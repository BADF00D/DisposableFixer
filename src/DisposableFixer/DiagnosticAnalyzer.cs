using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DisposableFixer.Configuration;
using DisposableFixer.Extensions;
using DisposableFixer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using IMethodSymbol = Microsoft.CodeAnalysis.IMethodSymbol;

namespace DisposableFixer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisposableFixerAnalyzer : DiagnosticAnalyzer
    {
        private static readonly IDetector Detector = new TrackingTypeDetector();
        private static readonly IConfiguration Configuration = ConfigurationManager.Instance;

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(
                Descriptor.AnonymousObjectFromObjectCreationDescriptor,
                Descriptor.AnonymousObjectFromMethodInvocationDescriptor,
                Descriptor.NotDisposedLocalVariableDescriptor,
                
                Descriptor.AssignmentFromObjectCreationToFieldNotDisposedDescriptor,
                Descriptor.AssignmentFromObjectCreationToPropertyNotDisposedDescriptor,
                Descriptor.AssignmentFromMethodInvocationToFieldNotDisposedDescriptor,
                Descriptor.AssignmentFromMethodInvocationToPropertyNotDisposedDescriptor
                );

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpressionStatement, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(AnalyzeObjectCreationExpressionStatement,
                SyntaxKind.ObjectCreationExpression);
        }

        private static void AnalyzeObjectCreationExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is ObjectCreationExpressionSyntax node)) return; //something went wrong

            var symbolInfo = context.SemanticModel.GetSymbolInfo(node);
            var t = (symbolInfo.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol;
            var ctx = CustomAnalysisContext.WithOriginalNode(context, DisposableSource.ObjectCreation, t, Detector, Configuration);
            if (!ctx.CouldDetectType()) { }
            else if (!ctx.IsDisposableOrImplementsDisposable()) return;
            else if (node.IsParentADisposeCallIgnoringParenthesis()) return; //(new MemoryStream()).Dispose()
            else if (Detector.IsIgnoredTypeOrImplementsIgnoredInterface(ctx.Type)) { } 
            else if (node.IsReturnedInProperty()) AnalyzeNodeInReturnStatementOfProperty(ctx);
            else if (node.IsPartOfReturnStatementInBlock()) { } // return new MemoryStream() or return Task.FromResult(new MemoryStream())
            else if (node.IsArrowExpressionClauseOfMethod()) { } // void Create()=>CreateMemoryStream()
            else if (node.IsReturnValueInLambdaExpression()) { }
            else if (node.IsReturnedLaterWithinMethod()) { }
            else if (node.IsReturnedLaterWithinParenthesizedLambdaExpression()) { }
            else if (node.IsPartOfMethodCall())
            {
                AnalyzePartOfMethodCall(ctx);
            }
            else if (node.IsMaybePartOfMethodChainUsingTrackingExtensionMethod())
            {
                var methodInvocation = node.Parent.Parent as InvocationExpressionSyntax;
                if (Detector.IsTrackingMethodCall(methodInvocation, context.SemanticModel)) return;
            }
            else if (node.IsArgumentInObjectCreation()) AnalyzeNodeInArgumentList(ctx);
            else if (node.IsPartIfArrayInitializerThatIsPartOfObjectCreation())
            {
                var objectCreation = node.Parent.Parent.Parent.Parent.Parent as ObjectCreationExpressionSyntax;
                CheckIfObjectCreationTracksNode(ctx, objectCreation);
            }
            else if (node.IsDescendantOfUsingHeader()) { }//this have to be checked after IsArgumentInObjectCreation
            else if (node.IsDescendantOfVariableDeclarator()) AnalyzeNodeWithinVariableDeclarator(ctx);
            else if (node.IsPartOfAssignmentExpression()) AnalyzeNodeInAssignmentExpression(ctx);
            else if (node.IsPartOfPropertyExpressionBody())  AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(ctx);
            else if (node.IsPartOfAutoProperty()) AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(ctx);
            else ctx.ReportNotDisposedAnonymousObject(); //new MemoryStream();
        }

        private static void CheckIfObjectCreationTracksNode(CustomAnalysisContext context, ObjectCreationExpressionSyntax objectCreation)
        {
            var t = context.SemanticModel.GetReturnTypeOf(objectCreation);
            if (t == null) return;//return type could not be determined
            if (Detector.IsTrackedType(t, objectCreation, context.SemanticModel)) return;

            context.ReportNotDisposedAnonymousObject();
        }

        private static void AnalyzeNodeInReturnStatementOfProperty(CustomAnalysisContext context)
        {
            var node = context.Node;
            if (!(node.Parent.Parent.Parent.Parent.Parent is PropertyDeclarationSyntax propertyDeclaration)) return; // should not happen => we cke this before
            
            if (node.IsDisposedInDisposingMethod(propertyDeclaration.Identifier.Text, Configuration, context.SemanticModel)) return;
            context.ReportNotDisposedProperty(propertyDeclaration.Identifier.Text);
        }

        private static void AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(CustomAnalysisContext context)
        {
            var node = context.Node;
            if (!(node.Parent.Parent is PropertyDeclarationSyntax propertyDeclaration)) return; // should not happen => we cke this before

            if (node.IsDisposedInDisposingMethod(propertyDeclaration.Identifier.Text, Configuration, context.SemanticModel)) return;
            context.ReportNotDisposedProperty(propertyDeclaration.Identifier.Text);
        }

        private static void AnalyzeNodeWithinVariableDeclarator(CustomAnalysisContext context)
        {
            var node = context.Node;
            var identifier = node.GetIdentifierIfIsPartOfVariableDeclarator();//getIdentifier
            if (identifier == null) return;
            if (node.IsLocalDeclaration()) //var m = new MemoryStream();
            {
                AnalyzeNodeWithinLocalDeclaration(context, identifier);
            }
            else if (node.IsFieldDeclaration()) //_field = new MemoryStream();
            {
                AnalyzeNodeInFieldDeclaration(context, identifier);
            }
        }

        private static void AnalyzeNodeWithinLocalDeclaration(CustomAnalysisContext context, string localVariableName)
        {
            if (!context.Node.TryFindParentScope(out var parentScope)) return;

            var localVariableInsideUsing = parentScope
                .DescendantNodes<UsingStatementSyntax>()
                .SelectMany(@using => @using.DescendantNodes<IdentifierNameSyntax>())
                .Where(id => localVariableName != null && (string) id.Identifier.Value == localVariableName)
                .ToArray();

            if (localVariableInsideUsing.Any())
            {
                if (localVariableInsideUsing.Any(id => id.Parent is UsingStatementSyntax)) //using(mem))
                {
                    return;
                }
                if (IsArgumentInConstructorOfTrackingTypeWithinUsing(context, localVariableInsideUsing)) return;

                context.ReportNotDisposedLocalVariable();
                return;
            }
            var invocationExpressions = parentScope.DescendantNodes<InvocationExpressionSyntax>().ToArray();
            if (invocationExpressions.Any(ie => ie.IsInterlockedExchangeAssignExpressionFor(localVariableName))) return;
            if (ExistsDisposeCall(localVariableName, invocationExpressions, context.SemanticModel)) return;
            if (IsArgumentInTrackingMethod(context, localVariableName, invocationExpressions)) return;
            if (IsArgumentInConstructorOfTrackingType(context, localVariableName, parentScope)) return;
            if (IsCallToMethodThatIsConsideredAsDisposeCall(invocationExpressions, context)) return;
            
            context.ReportNotDisposedLocalVariable();
        }

        private static bool IsCallToMethodThatIsConsideredAsDisposeCall(IEnumerable<InvocationExpressionSyntax> invocations,
            CustomAnalysisContext context)
        {
            var fullName = GetReturnOrReceivedType(context);
            return Configuration.DisposingMethodsAtSpecialClasses.TryGetValue(fullName, out var methodCalls) 
                   && methodCalls.Any(mc => invocations.Any(ies => ies.IsCallToMethod(mc)));
        }

        private static string GetReturnOrReceivedType(CustomAnalysisContext context)
        {
            var node = context.OriginalNode;
            var typeInfo = context.SemanticModel.GetSymbolInfo(node);
            switch (node)
            {
                case ObjectCreationExpressionSyntax _:
                    return ((typeInfo.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol).GetFullNamespace();
                case InvocationExpressionSyntax _:
                    return ((typeInfo.Symbol as IMethodSymbol)?.ReturnType as INamedTypeSymbol).GetFullNamespace();
                default:
                    throw new ArgumentException($"Unexpected Node Type: '{node.GetType()}'");
            }
        }

        private static bool IsArgumentInConstructorOfTrackingTypeWithinUsing(CustomAnalysisContext context, IEnumerable<IdentifierNameSyntax> localVariableInsideUsing)
        {
            return localVariableInsideUsing
                .Select(id => id.Parent?.Parent?.Parent)
                .Where(parent => parent is ObjectCreationExpressionSyntax)
                .Cast<ObjectCreationExpressionSyntax>()
                .Any(ocs =>
                {
                    var sym = context.SemanticModel.GetSymbolInfo(ocs);
                    var type2 = (sym.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol;

                    return Detector.IsTrackedType(type2, ocs, context.SemanticModel);
                });
        }

        private static bool ExistsDisposeCall(string localVariableName, IEnumerable<InvocationExpressionSyntax> invocationExpressions, SemanticModel semanticModel)
        {
            return invocationExpressions.Any(ies => localVariableName != null && ies.IsCallToDisposeFor(localVariableName, semanticModel, Configuration));
        }

        private static bool IsArgumentInTrackingMethod(CustomAnalysisContext context, string localVariableName, IEnumerable<InvocationExpressionSyntax> invocationExpressions)
        {
            return invocationExpressions.Any(ie => ie.UsesVariableInArguments(localVariableName) && Detector.IsTrackingMethodCall(ie, context.SemanticModel));
        }

        private static bool IsArgumentInConstructorOfTrackingType(CustomAnalysisContext context,
            string localVariableName, SyntaxNode parentScope)
        {
            return parentScope
                .DescendantNodes<ObjectCreationExpressionSyntax>()
                .Any(oce =>
                {
                    var argumentListSyntax = oce.ArgumentList;
                    if (argumentListSyntax == null) return false;
                    return argumentListSyntax.Arguments.Any(arg =>
                    {
                        var expression = arg.Expression as IdentifierNameSyntax;
                        var isPartOfObjectCreation = expression?.Identifier.Text == localVariableName;
                        if (!isPartOfObjectCreation) return false;

                        //check if is tracking instance
                        var sym = context.SemanticModel.GetSymbolInfo(oce);
                        return (sym.Symbol as IMethodSymbol)?.ReceiverType is INamedTypeSymbol type2 
                               && Detector.IsTrackedType(type2, oce, context.SemanticModel);
                    });
                });
        }

        private static void AnalyzeNodeInFieldDeclaration(CustomAnalysisContext context,
            string variableName)
        {
            if (context.Node.IsDisposedInDisposingMethod(variableName, Configuration, context.SemanticModel)) return;
            
            context.ReportNotDisposedField(variableName);
        }

        private static void AnalyzeNodeInAssignmentExpression(CustomAnalysisContext context)
        {
            var node = context.Node;
            //is local or global variable
            var assignmentExpressionSyntax = node.Parent as AssignmentExpressionSyntax;
            var variableName = (assignmentExpressionSyntax?.Left as IdentifierNameSyntax)?.Identifier.Text;

            if (node.TryFindContainingMethod(out var containingMethod))
            {
                if (containingMethod.ContainsDisposeCallFor(variableName, context.SemanticModel, Configuration)) return;

                if (containingMethod.HasDecendentVariableDeclaratorFor(variableName))
                {
                    //local declaration in method
                    if (containingMethod.Returns(variableName)) return;
                    if (node.IsDescendantOfUsingHeader()) return;
                    if (node.IsArgumentInObjectCreation())
                    {
                        AnalyzeNodeInArgumentList(context);
                        return;
                    }

                    if (containingMethod.HasInterlockedExchangeWith(variableName)) return;
                    //is part of tracking call
                    context.ReportNotDisposedLocalVariable();
                    return;
                }
                if (node.IsDisposedInDisposingMethod(variableName, Configuration, context.SemanticModel)) return;
                if (node.IsArgumentInObjectCreation())
                {
                    AnalyzeNodeInArgumentList(context);
                    return;
                }

                //assignment to field or property
                var containingClass = node.FindContainingClass();
                if (containingClass == null) return;
                if (containingClass.FindFieldNamed(variableName) != null)
                    context.ReportNotDisposedField(variableName);
                else
                    context.ReportNotDisposedProperty(variableName);

                return;
            }

            if (node.TryFindContainingCtor(out var ctor))
            {
                if (ctor.HasDecendentVariableDeclaratorFor(variableName))
                {
                    //local variable in ctor
                    if (ctor.HasInterlockedExchangeWith(variableName)) return;
                    if (node.IsDescendantOfUsingHeader()) return;
                    if (node.IsArgumentInObjectCreation())
                    {
                        AnalyzeNodeInArgumentList(context);
                        return;
                    }
                    if (ctor.ContainsDisposeCallFor(variableName, context.SemanticModel, Configuration)) return;
                    context.ReportNotDisposedLocalVariable();
                }
                else //field or property
                {
                    if (node.IsDisposedInDisposingMethod(variableName, Configuration, context.SemanticModel)) return;

                    if (node.IsAssignmentToProperty(variableName))
                    {
                        context.ReportNotDisposedProperty(variableName);
                    }
                    else
                    {
                        context.ReportNotDisposedField(variableName);
                    }
                    
                }
            }
        }
        
        private static void AnalyzeNodeInArgumentList(CustomAnalysisContext context)
        {
            var objectCreation = context.Node.Parent.Parent.Parent as ObjectCreationExpressionSyntax;
            var t = context.SemanticModel.GetReturnTypeOf(objectCreation);
            if (t == null) return;//return type could not be determined
            if (Detector.IsTrackedType(t, objectCreation, context.SemanticModel)) return;

            context.ReportNotDisposedAnonymousObject();
        }

        private static void AnalyzeInvocationExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is InvocationExpressionSyntax node)) return;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(node);
            var symbol = symbolInfo.Symbol as IMethodSymbol;

            var type = symbol?.ReturnType as INamedTypeSymbol;
            var ctx = CustomAnalysisContext.WithOriginalNode(context, DisposableSource.InvocationExpression, type, Detector, Configuration);
            if (!ctx.CouldDetectType()) { }
            else if (node.IsParentADisposeCallIgnoringParenthesis()) return; //(new object()).AsDisposable().Dispose()
            else if (node.IsPartOfAwaitExpression()) AnalyzeInvocationExpressionInsideAwaitExpression(ctx);
            else if (!ctx.IsDisposableOrImplementsDisposable()) return;
            else if (node.IsReturnedInProperty()) AnalyzeNodeInReturnStatementOfProperty(ctx);
            else if (ctx.IsTypeIgnoredOrImplementsIgnoredInterface()) { } //GetEnumerator()
            else if (Detector.IsTrackingMethodCall(node, context.SemanticModel)) { }//ignored extension methods
            else if (Detector.IsIgnoredFactoryMethod(node, context.SemanticModel)) return; //A.Fake<IDisposable>
            else if (node.IsMaybePartOfMethodChainUsingTrackingExtensionMethod())
            {
                //there maybe multiple method invocations within one chain
                var baseNode = node;
                while(baseNode?.Parent is MemberAccessExpressionSyntax && baseNode?.Parent?.Parent is InvocationExpressionSyntax parentIes)
                {
                    baseNode = parentIes;
                    if (Detector.IsTrackingMethodCall(baseNode, context.SemanticModel)) return;
                }
            }
            else if (node.IsPartOfMethodCall())
            {
                AnalyzePartOfMethodCall(ctx);
            }
            else if (node.IsPartOfReturnStatementInBlock()) { } // return new MemoryStream() or return Task.FromResult(new MemoryStream())
            else if (node.IsArrowExpressionClauseOfMethod()) { } // void Create()=>new MemoryStream()
            else if (node.IsReturnValueInLambdaExpression()) { } //e.g. ()=> new MemoryStream
            else if (node.IsReturnedLaterWithinMethod()) { }
            else if (node.IsReturnedLaterWithinParenthesizedLambdaExpression()) { }
            else if (node.IsArgumentInObjectCreation()) AnalyzeNodeInArgumentList(ctx);
            else if (node.IsPartIfArrayInitializerThatIsPartOfObjectCreation()) {
                var objectCreation = node.Parent.Parent.Parent.Parent.Parent as ObjectCreationExpressionSyntax;
                CheckIfObjectCreationTracksNode(ctx, objectCreation);
            } 
            else if (node.IsDescendantOfUsingHeader()) { } //using(memstream) or using(new MemoryStream())
            else if (node.IsDescendantOfVariableDeclarator()) AnalyzeNodeWithinVariableDeclarator(ctx);
            else if (node.IsPartOfAssignmentExpression()) AnalyzeNodeInAssignmentExpression(ctx);
            else if (node.IsPartOfAutoProperty()) AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(ctx);
            else if (node.IsPartOfPropertyExpressionBody()) AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(ctx);
            else ctx.ReportNotDisposedAnonymousObject(); //call to Create(): MemeoryStream
        }

        private static void AnalyzePartOfMethodCall(CustomAnalysisContext ctx)
        {
            var methodInvocation = ctx.Node.Parent.Parent.Parent as InvocationExpressionSyntax;
            if (Detector.IsTrackingMethodCall(methodInvocation, ctx.Context.SemanticModel)) return;

            if (methodInvocation.IsInterlockedExchangeExpression()) return;

            ctx.ReportNotDisposedAnonymousObject();
        }

        private static void AnalyzeInvocationExpressionInsideAwaitExpression(CustomAnalysisContext context)
        {
            var node = context.Node;
            var awaitExpression = node.Parent as AwaitExpressionSyntax;
            var awaitExpressionInfo = context.SemanticModel.GetAwaitExpressionInfo(awaitExpression);
            if (!(awaitExpressionInfo.GetResultMethod?.ReturnType is INamedTypeSymbol returnType)) return;
            if (!returnType.IsDisposableOrImplementsDisposable()) return;
            if (Detector.IsIgnoredTypeOrImplementsIgnoredInterface(returnType)) return;
            if (awaitExpression.IsDescendantOfUsingHeader()) return;
            if (awaitExpression.IsPartOfVariableDeclaratorInsideAUsingDeclaration()) return;
            if (awaitExpression.IsPartOfReturnStatementInMethod()) return;
            if (awaitExpression.IsReturnedLaterWithinMethod()) return;
            if (awaitExpression.IsDescendantOfVariableDeclarator())
            {
                AnalyzeNodeWithinVariableDeclarator(context.NewWith(awaitExpression));
            }else if (awaitExpression.IsDescendantOfAssignmentExpressionSyntax())
            {
                if (node.TryFindParentClass(out var @class))
                {
                    var assignment = awaitExpression?.Parent as AssignmentExpressionSyntax;
                    var member = (assignment?.Left as IdentifierNameSyntax)?.Identifier.Text;
                    var isDisposed = @class.ContainsDisposeCallFor(member, context.SemanticModel, Configuration);
                    if (isDisposed) return;
                }
                context.ReportNotDisposedLocalVariable();
            }
            else
            {
                context.ReportNotDisposedAnonymousObject();
            }
        }
    }
}
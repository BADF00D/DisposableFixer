using System;
using System.CodeDom;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using DisposableFixer.Configuration;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DisposableFixer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DisposableFixerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DisposableFixer";
        public const string Category = "Wrong Usage";

        private const string DisposeMethod = "Dispose";
        private const string DisposableInterface = "IDisposable";

	    private static readonly IDetector Detector = new TrackingTypeDetector();

	    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(
                SyntaxNodeAnalysisContextExtension.AnonymousObjectFromObjectCreationDescriptor,
                SyntaxNodeAnalysisContextExtension.AnonymousObjectFromMethodInvokationDescriptor,
                SyntaxNodeAnalysisContextExtension.NotDisposedDescriptor,
                SyntaxNodeAnalysisContextExtension.NotDisposedLocalVariableDescriptor,
                
                SyntaxNodeAnalysisContextExtension.AssignmendFromObjectCreationToFieldNotDisposedDescriptor,
                SyntaxNodeAnalysisContextExtension.AssignmendFromObjectCreationToPropertyNotDisposedDescriptor,
                SyntaxNodeAnalysisContextExtension.AssignmendFromMethodInvocationToFieldNotDisposedDescriptor,
                SyntaxNodeAnalysisContextExtension.AssignmendFromMethodInvocationToPropertyNotDisposedDescriptor
                );

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyseInvokationExpressionStatement, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(AnalyseObjectCreationExpressionStatement,
                SyntaxKind.ObjectCreationExpression);
        }

        private static void AnalyseObjectCreationExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as ObjectCreationExpressionSyntax;
            if (node == null) return; //something went wrong

            var symbolInfo = context.SemanticModel.GetSymbolInfo(node);
            var type = (symbolInfo.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol;
            if (type == null) { }
            else if (node.IsParentADisposeCallIgnoringParenthesis()) return; //(new MemoryStream()).Dispose()
            else if (IsIgnoredTypeOrImplementsIgnoredInterface(type)) { }
            else if (node.IsPartOfReturnStatement()) { }
            else if (node.IsReturnValueInLambdaExpression()) { }
            else if (node.IsReturnedLaterWithinMethod()) { }
            else if (!IsDisposeableOrImplementsDisposable(type)) { }
            else if (node.IsMaybePartOfMethodChainUsingTrackingExtensionMethod())
            {
                var methodInvokation = node.Parent.Parent as InvocationExpressionSyntax;
                if (Detector.IsTrackingMethodCall(methodInvokation, context.SemanticModel)) return;
            }
            else if (node.IsArgumentInObjectCreation()) AnalyseNodeInArgumentList(context, node, DisposableSource.ObjectCreation);
            else if (node.IsDescendantOfUsingHeader()) { }//this have to be checked after IsArgumentInObjectCreation
            else if (node.IsDescendantOfVariableDeclarator()) AnalyseNodeWithinVariableDeclarator(context, node, DisposableSource.ObjectCreation);
            else if (node.IsPartOfAssignmentExpression()) AnalyseNodeInAssignmentExpression(context, node, DisposableSource.ObjectCreation);
            else context.ReportNotDisposedAnonymousObject(DisposableSource.ObjectCreation); //new MemoryStream();
        }

        private static void AnalyseNodeWithinVariableDeclarator(SyntaxNodeAnalysisContext context,
            SyntaxNode node, DisposableSource source)
        {
            var identifier = node.GetIdentifierIfIsPartOfVariableDeclarator();//getIdentifier
            if (identifier == null) return;
            if (node.IsLocalDeclaration()) //var m = new MemoryStream();
            {
                AnalyseNodeWithinLocalDeclaration(context, node, identifier);
            }
            else if (node.IsFieldDeclaration()) //_field = new MemoryStream();
            {
                AnalyseNodeInFieldDeclaration(context, node, identifier, source);
            }
        }

        private static void AnalyseNodeWithinLocalDeclaration(SyntaxNodeAnalysisContext context,
            SyntaxNode node, string identifier)
        {
            SyntaxNode parentScope;//lamda or ctor or method 
            if (!node.TryFindParentScope(out parentScope)) return;

            var usings = parentScope.DescendantNodes<UsingStatementSyntax>()
                .SelectMany(@using => @using.DescendantNodes<IdentifierNameSyntax>())
                .Where(id => identifier != null && (string) id.Identifier.Value == identifier)
                .ToArray();

            if (usings.Any())
            {
                if (usings.Any(id => id.Parent is UsingStatementSyntax)) //using(mem))
                {
                    return;
                }
                var isTracked = usings
                    .Select(id => id.Parent?.Parent?.Parent)
                    .Where(parent => parent is ObjectCreationExpressionSyntax)
                    .Any(ocs =>
                    {
                        var sym = context.SemanticModel.GetSymbolInfo(ocs);
                        var type2 = (sym.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol;

                        return Detector.IsTrackedType(type2, ocs as ObjectCreationExpressionSyntax, context.SemanticModel);
                    });
                if (isTracked) return;

                context.ReportNotDisposedLocalDeclaration();
                return;
            }
            var invokationExpression = parentScope.DescendantNodes<InvocationExpressionSyntax>().ToArray();
            if (invokationExpression.Any(ies => identifier != null && ies.IsCallToDisposeFor(identifier))) return;

            if (parentScope.DescendantNodes<ObjectCreationExpressionSyntax>().Any(oce => {
                return oce.ArgumentList.Arguments.Any(arg => {
                    var expression = arg.Expression as IdentifierNameSyntax;
                    var isPartOfObjectcreation = expression?.Identifier.Text == identifier;
                    if (!isPartOfObjectcreation) return false;

                    //check if is tracking instance
                    var sym = context.SemanticModel.GetSymbolInfo(oce);
                    var type2 = (sym.Symbol as IMethodSymbol)?.ReceiverType as INamedTypeSymbol;

                    return Detector.IsTrackedType(type2, oce, context.SemanticModel);
                });
            })) {
                return;
            }
            
            context.ReportNotDisposedLocalDeclaration();
        }

        private static void AnalyseNodeInFieldDeclaration(SyntaxNodeAnalysisContext context,
            SyntaxNode node, string nameOfVariable, DisposableSource source)
        {
            if (node.IsDisposedInDisposedMethod(nameOfVariable)) return;

            context.ReportNotDisposedField(source);
        }

        private static void AnalyseNodeInAssignmentExpression(SyntaxNodeAnalysisContext context,
            SyntaxNode node, DisposableSource source)
        {
            //is local or global variable
            var assignmentExrepssion = node.Parent as AssignmentExpressionSyntax;
            var variableName = (assignmentExrepssion?.Left as IdentifierNameSyntax)?.Identifier.Text;
            
            MethodDeclarationSyntax containingMethod;
            if (node.TryFindContainingMethod(out containingMethod))
            {
                if (containingMethod.ContainsDisposeCallFor(variableName)) return;

                if (containingMethod.HasDecendentVariableDeclaratorFor(variableName))
                {
                    //local declaration in method
                    if (containingMethod.Returns(variableName)) return;
                    if (node.IsDescendantOfUsingHeader()) return;
                    if (node.IsArgumentInObjectCreation())
                    {
                        AnalyseNodeInArgumentList(context, node, source);
                        return;
                    }
                    //is part of tracking call
                    context.ReportNotDisposedLocalDeclaration();
                    return;
                }
                //field declaration
                if (node.IsDisposedInDisposedMethod(variableName)) return;
                if (node.IsArgumentInObjectCreation())
                {
                    AnalyseNodeInArgumentList(context, node, source);
                    return;
                }
                context.ReportNotDisposedField(source);
                return;
            }
            ConstructorDeclarationSyntax ctor;
            if (node.TryFindContainingCtor(out ctor))
            {
                if (ctor.HasDecendentVariableDeclaratorFor(variableName))
                {
                    //local variable in ctor
                    if (node.IsDescendantOfUsingHeader()) return;
                    if (node.IsArgumentInObjectCreation())
                    {
                        AnalyseNodeInArgumentList(context, node, source);
                        return;
                    }
                    if (ctor.ContainsDisposeCallFor(variableName)) return;
                    context.ReportNotDisposedLocalDeclaration();
                }
                else //field or property
                {
                    if (node.IsDisposedInDisposedMethod(variableName)) return;

                    if (node.IsAssignmentToProperty(variableName))
                    {
                        context.ReportNotDisposedProperty(source);
                    }
                    else
                    {
                        context.ReportNotDisposedField(source);
                    }
                    
                }
            }
        }
        
        private static void AnalyseNodeInArgumentList(SyntaxNodeAnalysisContext context,
            SyntaxNode node, DisposableSource source)
        {
            var objectCreation = node.Parent.Parent.Parent as ObjectCreationExpressionSyntax;
            var t = context.SemanticModel.GetReturnTypeOf(objectCreation);
            if (t == null) return;//return type could not be determind
            if (Detector.IsTrackedType(t, objectCreation, context.SemanticModel)) return;

            context.ReportNotDisposedAnonymousObject(source);
        }

        private static void AnalyseInvokationExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as InvocationExpressionSyntax;
            if (node == null) return;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(node);
            var symbol = symbolInfo.Symbol as IMethodSymbol;
            var type = symbol?.ReturnType as INamedTypeSymbol;

            if (type == null) { } 
            else if (node.IsParentADisposeCallIgnoringParenthesis()) return; //(new object()).AsDisposable().Dispose()
            else if (node.IsPartOfAwaitExpression()) AnalyseInvokationExpressionInsideAwaitExpression(context, node);
            else if (IsIgnoredTypeOrImplementsIgnoredInterface(type)) { } 
            else if (Detector.IsTrackingMethodCall(node, context.SemanticModel)) { }
            else if (node.IsMaybePartOfMethodChainUsingTrackingExtensionMethod())
            {
                //there maybe multiple method invocations within one chain
                var baseNode = node;
                while(baseNode?.Parent is MemberAccessExpressionSyntax && baseNode?.Parent?.Parent is InvocationExpressionSyntax)
                {
                    baseNode = baseNode.Parent.Parent as InvocationExpressionSyntax;
                    if (Detector.IsTrackingMethodCall(baseNode, context.SemanticModel)) return;
                }
            }
            else if (!IsDisposeableOrImplementsDisposable(type)) { } 
            else if (node.IsPartOfReturnStatement()) { } //return new StreamReader()
            else if (node.IsReturnValueInLambdaExpression()) { } //e.g. ()=> new MemoryStream
            else if (node.IsReturnedLaterWithinMethod()) { }
            else if (node.IsArgumentInObjectCreation()) AnalyseNodeInArgumentList(context, node, DisposableSource.InvokationExpression);
            else if (node.IsDescendantOfUsingHeader()) { } //using(memstream) or using(new MemoryStream())
            else if (node.IsDescendantOfVariableDeclarator()) AnalyseNodeWithinVariableDeclarator(context, node, DisposableSource.InvokationExpression);
            else if (node.IsPartOfAssignmentExpression()) AnalyseNodeInAssignmentExpression(context, node, DisposableSource.InvokationExpression);
            else context.ReportNotDisposedAnonymousObject(DisposableSource.InvokationExpression); //call to Create(): MemeoryStream
        }

        private static void AnalyseInvokationExpressionInsideAwaitExpression(SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax node)
        {
            var awaitExpression = node.Parent as AwaitExpressionSyntax;
            var awaitExpressionInfo = context.SemanticModel.GetAwaitExpressionInfo(awaitExpression);
            var returnType = awaitExpressionInfo.GetResultMethod.ReturnType as INamedTypeSymbol;
            if (!IsDisposeableOrImplementsDisposable(returnType)) return;
            if (IsIgnoredTypeOrImplementsIgnoredInterface(returnType)) return;
            if (awaitExpression.IsDescendantOfUsingHeader()) return;
            if (awaitExpression.IsPartOfVariableDeclaratorInsideAUsingDeclaration()) return;
            if (awaitExpression.IsPartOfReturnStatement()) return;
            if (awaitExpression.IsReturnedLaterWithinMethod()) return;
            if (awaitExpression.IsDescendantOfVariableDeclarator())
            {
                AnalyseNodeWithinVariableDeclarator(context, awaitExpression, DisposableSource.InvokationExpression);
            }
            else
            {
                context.ReportNotDisposedAnonymousObject(DisposableSource.InvokationExpression);
            }
        }

        private static bool IsIgnoredTypeOrImplementsIgnoredInterface(INamedTypeSymbol type)
        {
            if (Detector.IsIgnoredType(type)) return true;

            var inter = type.AllInterfaces.Select(ai => ai);
            return inter.Any(@if => Detector.IsIgnoredInterface(@if));
        }

        private static bool IsDisposeableOrImplementsDisposable(ITypeSymbol typeInfo)
        {
            return IsIDisposable(typeInfo) || ImplementsIDisposable(typeInfo);
        }

        private static bool IsIDisposable(ISymbol typeInfo)
        {
            return typeInfo.Name == DisposableInterface;
        }

        private static bool ImplementsIDisposable(ITypeSymbol typeInfo)
        {
            return typeInfo.AllInterfaces.Any(i => i.Name == DisposableInterface);
        }
    }
}
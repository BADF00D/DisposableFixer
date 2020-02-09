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
                NotDisposed.AnonymousObject.FromObjectCreationDescriptor,
                NotDisposed.AnonymousObject.FromMethodInvocationDescriptor,
                NotDisposed.LocalVariable.Descriptor,

                NotDisposed.Assignment.FromObjectCreation.ToField.OfSameTypeDescriptor,
                NotDisposed.Assignment.FromObjectCreation.ToProperty.OfSameTypeDescriptor,
                NotDisposed.Assignment.FromMethodInvocation.ToField.OfSameTypeDescriptor,
                NotDisposed.Assignment.FromMethodInvocation.ToProperty.OfSameTypeDescriptor,
                NotDisposed.Assignment.FromObjectCreation.ToStaticField.OfSameTypeDescriptor,
                NotDisposed.Assignment.FromObjectCreation.ToStaticProperty.OfSameTypeDescriptor,
                NotDisposed.Assignment.FromMethodInvocation.ToStaticField.OfSameTypeDescriptor,
                NotDisposed.Assignment.FromMethodInvocation.ToStaticProperty.OfSameTypeDescriptor,

                NotDisposed.Assignment.FromObjectCreation.ToField.OfAnotherTypeDescriptor,
                NotDisposed.Assignment.FromObjectCreation.ToProperty.OfAnotherTypeDescriptor,
                NotDisposed.Assignment.FromMethodInvocation.ToField.OfAnotherTypeDescriptor,
                NotDisposed.Assignment.FromMethodInvocation.ToProperty.OfAnotherTypeDescriptor,
                NotDisposed.Assignment.FromObjectCreation.ToStaticField.OfAnotherTypeDescriptor,
                NotDisposed.Assignment.FromObjectCreation.ToStaticProperty.OfAnotherTypeDescriptor,
                NotDisposed.Assignment.FromMethodInvocation.ToStaticField.OfAnotherTypeDescriptor,
                NotDisposed.Assignment.FromMethodInvocation.ToStaticProperty.OfAnotherTypeDescriptor,

                NotDisposed.FactoryProperty.Descriptor,
                NotDisposed.StaticFactoryProperty.Descriptor,

                Hidden.Disposable
                );

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
#if !DEBUG
            context.EnableConcurrentExecution();
#endif
            context.RegisterSyntaxNodeAction(
                AnalyzeInvocationExpressionStatement, 
                SyntaxKind.InvocationExpression);

            context.RegisterSyntaxNodeAction(
                AnalyzeObjectCreationExpressionStatement,
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
            else if (node.IsPartOfReturnStatementInBlock())
            {
                AnalyzeForHiddenDisposables(node, ctx);
            } // return new MemoryStream() or return Task.FromResult(new MemoryStream())
            else if (node.IsArrowExpressionClauseOfMethod())// void/IDisposable/object Create()=>CreateMemoryStream()
            {
                AnalyzeArrowExpressionOfMethod(ctx);
            }
            else if (node.IsArrowExpressionClauseOfLocalFunction())
            {
                AnalyzeArrowExpressionOfLocalFunction(ctx);
            }
            else if (node.IsPartOfLocalFunction(out var localFunctionStatement) && node.IsReturnedValue())
            {
                var returnTypeSyntax = localFunctionStatement.ReturnType;
                var returnType = ctx.SemanticModel.GetSymbolInfo(returnTypeSyntax);
            }
            else if (node.IsPartOfYieldReturnStatementInBlock()) { } //yield return new MemoryStream()
            
            else if (node.IsReturnValueInLambdaExpression()) { }
            else if (node.IsReturnedLaterWithinMethod())
            {
                AnalyzeForHiddenDisposables(node, ctx);
            }
            else if (node.IsReturnedLaterWithinParenthesizedLambdaExpression()) { }
            else if (node.IsPartOfMethodCall())
            {
                AnalyzePartOfMethodCall(ctx);
            }
            else if (node.IsMaybePartOfMethodChainUsingTrackingExtensionMethod() && Detector.IsTrackingMethodCall(node.Parent.Parent as InvocationExpressionSyntax, context.SemanticModel)) return;
            else if (node.IsArgumentInObjectCreation()) AnalyzeNodeInArgumentList(ctx);
            else if (node.IsPartIfArrayInitializerThatIsPartOfObjectCreation())
            {
                var objectCreation = node.Parent.Parent.Parent.Parent.Parent as ObjectCreationExpressionSyntax;
                CheckIfObjectCreationTracksNode(ctx, objectCreation);
            }
            else if (node.IsDescendantOfUsingHeader()) { }//this have to be checked after IsArgumentInObjectCreation
            else if (node.TryGetParentVariableDeclarator(out var vds)) AnalyzeNodeWithinVariableDeclarator(ctx, vds);
            else if (node.IsPartOfAssignmentExpression()) AnalyzeNodeInAssignmentExpression(ctx);
            else if (node.IsPartOfPropertyExpressionBody())  AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(ctx);
            else if (node.IsPartOfAutoProperty()) AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(ctx);
            else ctx.ReportNotDisposedAnonymousObject(); //new MemoryStream();
        }

        private static void AnalyzeArrowExpressionOfLocalFunction(CustomAnalysisContext context)
        {
            var localFunction = context.Node.Parent.Parent as LocalFunctionStatementSyntax;
            if (localFunction.ReturnsVoid())
            {
                context.ReportNotDisposedAnonymousObject();
            }
        }

        private static void AnalyzeArrowExpressionOfMethod(CustomAnalysisContext context)
        {
            var method = context.Node.Parent.Parent as MethodDeclarationSyntax;
            if (method.ReturnsVoid())
            {
                context.ReportNotDisposedAnonymousObject();
            }
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
            context.ReportNotDisposedPropertyFactory(propertyDeclaration.Identifier.Text, propertyDeclaration.IsStatic());
        }

        private static void AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(CustomAnalysisContext context)
        {
            var node = context.Node;
            if (!(node.Parent.Parent is PropertyDeclarationSyntax propertyDeclaration)) return; // should not happen => we cke this before

            if (node.IsDisposedInDisposingMethod(propertyDeclaration.Identifier.Text, Configuration, context.SemanticModel)) return;
            if (propertyDeclaration.ExpressionBody != null)
            {
                context.ReportNotDisposedPropertyFactory(propertyDeclaration.Identifier.Text, propertyDeclaration.IsStatic());
            }
            else
            {
                context.ReportNotDisposedProperty(propertyDeclaration.Identifier.Text, propertyDeclaration.IsStatic());
            }
        }

        private static void AnalyzeNodeWithinVariableDeclarator(CustomAnalysisContext context, VariableDeclaratorSyntax variableDeclarator)
        {
            var node = context.Node;
            var identifier = variableDeclarator.Identifier.Text;
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

                context.ReportNotDisposedLocalVariable(localVariableName);
                return;
            }
            var invocationExpressions = parentScope.DescendantNodes<InvocationExpressionSyntax>().ToArray();
            if (invocationExpressions.Any(ie => ie.IsInterlockedExchangeAssignExpressionFor(localVariableName))) return;
            if (ExistsDisposeCall(localVariableName, invocationExpressions, context.SemanticModel)) return;
            if (IsArgumentInTrackingMethod(context, localVariableName, invocationExpressions)) return;
            if (IsArgumentInConstructorOfTrackingType(context, localVariableName, parentScope)) return;
            if (IsCallToMethodThatIsConsideredAsDisposeCall(invocationExpressions, context)) return;
            if (IsDisposeDelegated(parentScope, localVariableName)) return;

            context.ReportNotDisposedLocalVariable(localVariableName);
        }

        /// <summary>
        /// Checks if Dispose call if don't via referenced via a Delegate. E.g. Observable.Do(variable.Dispose)
        /// </summary>
        /// <param name="parentScope">The scope that should contain the call.</param>
        /// <param name="localVariableName">The local variable that should be part of the MemberAccessExpression</param>
        /// <returns></returns>
        private static bool IsDisposeDelegated(SyntaxNode parentScope, string localVariableName) => parentScope
            .DescendantNodes<ArgumentSyntax>()
            .Any(argument => argument.Expression is MemberAccessExpressionSyntax maes
                             && maes.Expression is IdentifierNameSyntax variable
                             && variable.Identifier.Text == localVariableName
                             && maes.Name is IdentifierNameSyntax member
                             && member.Identifier.Text == Constants.Dispose);

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
            return invocationExpressions.Any(ie => ie.ArgumentList.HasArgumentWithName(localVariableName) && Detector.IsTrackingMethodCall(ie, context.SemanticModel));
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
            string fieldName)
        {
            if (context.Node.IsDisposedInDisposingMethod(fieldName, Configuration, context.SemanticModel)) return;

            var containeringClass = context.Node.FindContainingClass();
            var fieldDeclaration = containeringClass?.FindFieldNamed(fieldName);
            if (fieldDeclaration == null) return;
            context.ReportNotDisposedField(fieldName, fieldDeclaration.IsStatic());

        }

        private static void AnalyzeNodeInAssignmentExpression(CustomAnalysisContext context)
        {
            void AnalyzeMemberAccessInAssignmentExpression(MemberAccessExpressionSyntax maes, IdentifierNameSyntax otherInstance)
            {
                var memberName = maes.Name.Identifier.Text;
                var typeInfo = context.SemanticModel.GetTypeInfo(maes.Expression);
                var type = typeInfo.Type;
                var member = type?.GetMembers(maes.Name.Identifier.Text).FirstOrDefault();
                
                if (member == null) return;

                var isProperty = member is IPropertySymbol;
                var isField = member is IFieldSymbol;
                var isStatic = member.IsStatic;
                if (isProperty)
                {
                    if (Detector.IsTrackedSetter(type, memberName, TrackingMode.Always)) return;
                    context.ReportNotDisposedPropertyOfAnotherType(memberName, otherInstance.Identifier.Text, isStatic);
                }
                else if (isField)
                {
                    context.ReportNotDisposedFieldOfAnotherType(memberName, otherInstance.Identifier.Text, isStatic);
                }
            }

            var node = context.Node;
            //is local or global variable
            var assignmentExpressionSyntax = node.Parent as AssignmentExpressionSyntax;
            var variableName = (assignmentExpressionSyntax?.Left as IdentifierNameSyntax)?.Identifier.Text;
            if (assignmentExpressionSyntax?.Parent is InitializerExpressionSyntax initializerExpression &&
                assignmentExpressionSyntax.Parent?.Parent is ObjectCreationExpressionSyntax objectCreationExpression)
            {
                var typeInfo = context.SemanticModel.GetTypeInfo(objectCreationExpression);
                if (typeInfo.Type == null) return;
                if (context.Detector.IsTrackedSetter(typeInfo.Type, variableName)) return;
                context.ReportNotDisposedPropertyOfAnotherType(variableName);
                return;
            }

            if (node.TryFindContainingMethod(out var containingMethod))
            {
                if (containingMethod.ContainsDisposeCallFor(variableName, context.SemanticModel, Configuration)) return;

                if (containingMethod.HasDescendentVariableDeclaratorFor(variableName))
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
                    context.ReportNotDisposedLocalVariable(variableName);
                    return;
                }
                if (node.IsDisposedInDisposingMethod(variableName, Configuration, context.SemanticModel)) return;
                if (node.IsArgumentInObjectCreation())
                {
                    AnalyzeNodeInArgumentList(context);
                    return;
                }

                if (assignmentExpressionSyntax?.Left is MemberAccessExpressionSyntax maes &&
                    maes.Expression is IdentifierNameSyntax otherInstance)
                {
                    AnalyzeMemberAccessInAssignmentExpression(maes, otherInstance);
                    return;
                }

                if (node.IsTrackedViaTrackingMethod(context, containingMethod, variableName))
                {
                    return;
                }

                //assignment to field or property
                ReportStaticOrNonStaticNotDisposedMember(context, variableName);

                return;
            }

            if (node.TryFindContainingCtor(out var ctor))
            {
                if (ctor.HasDescendentVariableDeclaratorFor(variableName))
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
                    context.ReportNotDisposedLocalVariable(variableName);
                }
                else if (assignmentExpressionSyntax?.Left is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax otherInstance) 
                {
                    AnalyzeMemberAccessInAssignmentExpression(maes, otherInstance);
                }
                else //field or property //assignmentExpressionSyntax?.Left is a IdentifierExpression
                {
                    if (node.IsDisposedInDisposingMethod(variableName, Configuration, context.SemanticModel)) return;
                    if (node.IsTrackedViaTrackingMethod(context, ctor, variableName)) return;


                    if (node.IsAssignmentToProperty(variableName, out var isStatic))
                    {
                        context.ReportNotDisposedProperty(variableName, isStatic);
                    }
                    else
                    {
                        context.ReportNotDisposedField(variableName, node.IsAssignmentToStaticField(variableName));
                    }

                }
            }
        }

        private static void ReportStaticOrNonStaticNotDisposedMember(CustomAnalysisContext context, string variableName)
        {
            var node = context.Node;
            var containingClass = node.FindContainingClass();
            if (containingClass == null) return;
            var fieldDeclarationSyntax = containingClass.FindFieldNamed(variableName);
            if (fieldDeclarationSyntax != null)
            {
                context.ReportNotDisposedField(variableName, fieldDeclarationSyntax.IsStatic());
            }
            else
            {
                var prop = containingClass.FindPropertyNamed(variableName);
                if (prop == null) return; //this should never happen
                context.ReportNotDisposedProperty(variableName, prop.IsStatic());
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
            else if (node.IsMaybePartOfMethodChainUsingTrackingExtensionMethod() && IsTrackingMethod(node, ctx)) return;
            else if (node.IsPartOfMethodCall())
            {
                AnalyzePartOfMethodCall(ctx);
            }
            else if (node.IsPartOfReturnStatementInBlock())
            {
                AnalyzeForHiddenDisposables(node, ctx);
            } // return new MemoryStream() or return Task.FromResult(new MemoryStream())
            else if (node.IsArrowExpressionClauseOfMethod())// void/IDisposable/object Create()=>CreateMemoryStream()
            {
                AnalyzeArrowExpressionOfMethod(ctx);
            }
            else if (node.IsArrowExpressionClauseOfLocalFunction())
            {
                AnalyzeArrowExpressionOfLocalFunction(ctx);
            }
            else if (node.IsPartOfLocalFunction(out var localFunctionStatement) && node.IsReturnedValue())
            {
                var returnTypeSyntax = localFunctionStatement.ReturnType;
                var returnType = ctx.SemanticModel.GetSymbolInfo(returnTypeSyntax);
            }
            else if (node.IsPartOfYieldReturnStatementInBlock()) { } //yield return CreateMemoryStream()
            else if (node.IsReturnValueInLambdaExpression()) { } //e.g. ()=> new MemoryStream
            else if (node.IsReturnedLaterWithinMethod())
            {
                AnalyzeForHiddenDisposables(node, ctx);
            }
            else if (node.IsReturnedLaterWithinParenthesizedLambdaExpression()) { }
            else if (node.IsArgumentInObjectCreation()) AnalyzeNodeInArgumentList(ctx);
            else if (node.IsPartIfArrayInitializerThatIsPartOfObjectCreation()) {
                var objectCreation = node.Parent.Parent.Parent.Parent.Parent as ObjectCreationExpressionSyntax;
                CheckIfObjectCreationTracksNode(ctx, objectCreation);
            }
            else if (node.IsDescendantOfUsingHeader()) { } //using(memstream) or using(new MemoryStream())
            else if (node.TryGetParentVariableDeclarator(out var vds)) AnalyzeNodeWithinVariableDeclarator(ctx, vds);
            else if (node.IsPartOfAssignmentExpression()) AnalyzeNodeInAssignmentExpression(ctx);
            else if (node.IsPartOfAutoProperty()) AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(ctx);
            else if (node.IsPartOfPropertyExpressionBody()) AnalyzeNodeInAutoPropertyOrPropertyExpressionBody(ctx);
            else ctx.ReportNotDisposedAnonymousObject(); //call to Create(): MemeoryStream
        }

        private static void AnalyzeForHiddenDisposables(SyntaxNode invocationExpressionSyntax, CustomAnalysisContext ctx)
        {
            if (!invocationExpressionSyntax.TryFindContainingBlock(out var block)) return;
            switch (block.Parent)
            {
                case MethodDeclarationSyntax mds:
                    AnalyzeReturnType(mds.ReturnType, mds.Identifier);
                    break;
                case LocalFunctionStatementSyntax lfds:
                    AnalyzeReturnType(lfds.ReturnType, lfds.Identifier);
                    break;
            }

            void AnalyzeReturnType(ExpressionSyntax returnTypeSyntax, SyntaxToken methodOrFuncIdentifier)
            {
                var returnTypeInfo = ctx.SemanticModel.GetSymbolInfo(returnTypeSyntax);
                if (!(returnTypeInfo.Symbol is INamedTypeSymbol nts)) return;
                if (nts.IsTask())
                {
                    if (nts.TypeArguments.Any(ts => ts.IsDisposableOrImplementsDisposable()))
                    {
                        return;
                    }
                    ctx.ReportHiddenDisposable(ctx.Type.Name, nts.Name, methodOrFuncIdentifier.Text);
                }
                if (nts.IsDisposableOrImplementsDisposable()) return;
                ctx.ReportHiddenDisposable(ctx.Type.Name, nts.Name, methodOrFuncIdentifier.Text);
            }
        }

        private static bool IsTrackingMethod(InvocationExpressionSyntax node, CustomAnalysisContext context)
        {
            var baseNode = node;
            while (baseNode?.Parent is MemberAccessExpressionSyntax && baseNode?.Parent?.Parent is InvocationExpressionSyntax parentIes)
            {
                baseNode = parentIes;
                if (Detector.IsTrackingMethodCall(baseNode, context.SemanticModel)) return true;
            }
            return false;
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
            if (awaitExpression.TryGetParentVariableDeclarator(out var variableDeclarator))
            {
                AnalyzeNodeWithinVariableDeclarator(context.NewWith(awaitExpression), variableDeclarator);
            }else if (awaitExpression.IsDescendantOfAssignmentExpressionSyntax())
            {
                if (node.TryFindParentClass(out var @class))
                {
                    var assignment = awaitExpression?.Parent as AssignmentExpressionSyntax;
                    var memberName = (assignment?.Left as IdentifierNameSyntax)?.Identifier.Text;
                    var isDisposed = @class.ContainsDisposeCallFor(memberName, context.SemanticModel, Configuration);
                    if (isDisposed) return;
                    var fieldDeclarations = @class
                        .DescendantNodes<FieldDeclarationSyntax>()
                        .FirstOrDefault(fds =>
                            fds.DescendantNodes<VariableDeclaratorSyntax>()
                                .Any(vds => vds.Identifier.Text == memberName));
                    if (fieldDeclarations != null)
                    {
                        context.ReportNotDisposedField(memberName, fieldDeclarations.IsStatic());
                        return;
                    }

                    var propertyDeclaration = @class
                        .DescendantNodes<PropertyDeclarationSyntax>()
                        .FirstOrDefault(pds => pds.Identifier.Text == memberName);
                    if (propertyDeclaration != null)
                    {
                        context.ReportNotDisposedProperty(memberName, propertyDeclaration.IsStatic());
                        return;
                    }
                }
                context.ReportNotDisposedLocalVariable("class not found");
            }
            else
            {
                context.ReportNotDisposedAnonymousObject();
            }
        }
    }
}
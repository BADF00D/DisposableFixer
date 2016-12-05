using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
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

        private const string Category = "Wrong Usage";

        private const string DisposeMethod = "Dispose";
        private const string DisposableInterface = "IDisposable";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(
            nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof (Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager,
                typeof (Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager,
                typeof (Resources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category,
            DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSyntaxNodeAction(AnalyseLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
            //context.RegisterSyntaxNodeAction(AnalyseFieldDeclaration, SyntaxKind.FieldDeclaration);
            //context.RegisterSyntaxNodeAction(SimpleAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyseInvokationExpressionStatement, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(AnalyseObjectCreationExpressionStatement,
                SyntaxKind.ObjectCreationExpression);
        }

        private static void AnalyseObjectCreationExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            try
            {
                var node = context.Node as ObjectCreationExpressionSyntax;
                if (node == null) return; //something went wrong

                var symbolInfo = context.SemanticModel.GetSymbolInfo(node);
                var symbol = symbolInfo.Symbol as IMethodSymbol;
                var type = symbol?.ReceiverType as INamedTypeSymbol;

                if (type != null && !IsDisposeableOrImplementsDisposable(type)) return;

                //check if instance is Disposed via Dispose() or by include it in using
                if (node.IsNodeWithinUsing()) return; //using(new MemoryStream()){}
                if (node.IsPartOfReturn()) return; //return new MemoryStream(),
                if (node.IsPartOfVariableDeclarator())
                {
                    var identifier = (node.Parent.Parent as VariableDeclaratorSyntax)?.Identifier;
                    if (identifier == null) return;
                    if (node.IsLocalDeclaration())
                    {
                        SyntaxNode ctorOrMethod;
                        if (!node.TryFindContainingConstructorOrMethod(out ctorOrMethod)) return;

                        if (ctorOrMethod.DescendantNodes<UsingStatementSyntax>()
                            .SelectMany(@using =>
                            {
                                var objectCreationExpressionSyntaxs = @using
                                    .DescendantNodes<IdentifierNameSyntax>()
                                    .ToArray();
                                return objectCreationExpressionSyntaxs;
                            })
                            .Any(id => id.Identifier.Value == identifier.Value.Value))
                        {
                            return;
                        }
                        if (ctorOrMethod.DescendantNodes<InvocationExpressionSyntax>().Any(ies =>
                        {
                            var expression = (ies.Expression as MemberAccessExpressionSyntax);
                            var ids = expression.Expression as IdentifierNameSyntax;
                            return ids.Identifier.Text == identifier.Value.Text
                                && expression.Name.Identifier.Text == DisposeMethod;
                        }))
                        {
                            return;
                        }

                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
                        return;
                    }

                    if (node.IsFieldDeclaration())
                    {
                        var disposeMethod = node.FindContainingClass().DescendantNodes<MethodDeclarationSyntax>()
                            .FirstOrDefault(method => method.Identifier.Text == DisposeMethod);
                        if (disposeMethod == null)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
                            return;
                        };
                        var isDisposed = disposeMethod.DescendantNodes<InvocationExpressionSyntax>()
                            .Select(invo => invo.Expression as MemberAccessExpressionSyntax)
                            .Any(invo =>
                            {
                                var id = invo.Expression as IdentifierNameSyntax;
                                var member = id.Identifier.Text == identifier.Value.Text;
                                var callToDispose = invo.Name.Identifier.Text == DisposeMethod;

                                return member && callToDispose;
                            });
                        if (isDisposed) return;
                        context.ReportDiagnostic(Diagnostic.Create(Rule, node.GetLocation()));
                        return;

                    }
                }


                var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private static void AnalyseInvokationExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            try
            {
                var node = context.Node;

                var symbolInfo = context.SemanticModel.GetSymbolInfo(node);
                var symbol = symbolInfo.Symbol as IMethodSymbol;
                var type = symbol?.ReturnType as INamedTypeSymbol;

                if (type != null && !IsDisposeableOrImplementsDisposable(type)) return;

                var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        //private static void AnalyseLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        //{
        //    try
        //    {
        //        var node = context.Node;
        //        var symanticModel = context.SemanticModel;
        //        var creation = node
        //            .DescendantNodes<VariableDeclaratorSyntax>()
        //            .FirstOrDefault(n => n?.DescendantNodes<ObjectCreationExpressionSyntax>().Any() ?? false);

        //        if (creation == null)
        //        {
        //            //here is not creation, but maybe a factory is called that delivers an IDisposable
        //            AnalyseLocalDeclarationStatementForFactoryCall(context);
        //            return;
        //        }
        //        var identifierNameSyntax = node.DescendantNodes<IdentifierNameSyntax>().FirstOrDefault();
        //        var typeInfo = symanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
        //        if (typeInfo == null) return;
        //        if (!typeInfo.AllInterfaces.Any(i => i.Name == DisposableInterface)) return;


        //        var location =
        //            creation.DescendantNodes<ObjectCreationExpressionSyntax>().FirstOrDefault().GetLocation();
        //        var name = creation.Identifier.Text;

        //        //is this instance wrapped into a using
        //        var method = creation.TryFindContainingMethod();
        //        if (IsCreationWithinMethod(method))
        //        {
        //            AnalyseCreationWithinMethod(method, context, name, location);
        //            return;
        //        }

        //        //sereach using in ctor
        //        var ctor = creation.FindContainingConstructor();
        //        if (IsCreationWithinCtor(ctor))
        //        {
        //            AnalyseCreationWithinMethod(ctor, context, name, location);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("Something went wrong: " + e);
        //    }
        //}

        private static void AnalyseLocalDeclarationStatementForFactoryCall(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;

            var simpleMemberExpression = node.DescendantNodes<IdentifierNameSyntax>().Skip(1).FirstOrDefault();
            if (simpleMemberExpression == null) return;
            //todo get semantic model of node => waht is the return type????
            var symanticModel = context.SemanticModel;

            var localSymbol = node
                .DescendantNodes<VariableDeclaratorSyntax>()
                .Select(descendantNode => symanticModel.GetDeclaredSymbol(descendantNode) as ILocalSymbol)
                .FirstOrDefault(m => m != null);

            var isDisposable = localSymbol.Type.AllInterfaces.Any(ints => ints.Name == DisposableInterface);

            if (!isDisposable) return;

            var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        private static void AnalyseCreationWithinMethod(SyntaxNode methodOrCtor, SyntaxNodeAnalysisContext context,
            string name, Location location)
        {
            if (methodOrCtor.ContainsUsingsOfVariableNamed(name)) return;

            var isDisposed = methodOrCtor.DescendantNodes<MemberAccessExpressionSyntax>()
                .Where(maes =>
                {
                    var ins = maes.Expression as IdentifierNameSyntax;
                    return ins?.Identifier.Text == name && maes.Name.Identifier.Text == DisposeMethod;
                });

            if (isDisposed.Any()) return;

            var diagnostic = Diagnostic.Create(Rule, location);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsCreationWithinCtor(ConstructorDeclarationSyntax ctor)
        {
            return ctor != null;
        }

        private static bool IsCreationWithinMethod(MethodDeclarationSyntax method)
        {
            return method != null;
        }

        private static void AnalyseFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node;
            var symanticModel = context.SemanticModel;
            var creation = node
                .DescendantNodes<VariableDeclaratorSyntax>()
                .FirstOrDefault(n => n?.DescendantNodes<ObjectCreationExpressionSyntax>().Any() ?? false);

            if (creation == null) return; //nothing to analyse
            var identifierNameSyntax = node.DescendantNodes<IdentifierNameSyntax>().FirstOrDefault();
            var typeInfo = symanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
            if (typeInfo == null) return;
            if (!IsDisposeableOrImplementsDisposable(typeInfo)) return;


            var location =
                creation.DescendantNodes<ObjectCreationExpressionSyntax>().FirstOrDefault().GetLocation();
            var name = creation.Identifier.Text;

            //this is a field => go to containing class and find dispose
            var classDeclaration = creation.FindContainingClass();
            var isDisposed2 = (classDeclaration?
                .DescendantNodes<MemberAccessExpressionSyntax>())
                .Any(
                    maes =>
                        (maes.Expression as IdentifierNameSyntax)?.Identifier.Text == name &&
                        maes.Name.Identifier.Text == DisposeMethod);

            if (isDisposed2) return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, location));
        }

        private static void SimpleAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            var symanticModel = context.SemanticModel;

            var creationSyntax = context.Node
                .DescendantNodes<ObjectCreationExpressionSyntax>()
                .FirstOrDefault();

            if (creationSyntax == null) return;
            var identifierNameSyntax = context.Node.DescendantNodes<IdentifierNameSyntax>().FirstOrDefault();
            /*at this point we cant get the type of the variable, because its assignment to a field.
             * We have to find the field and get the name from that FieldDeclaration */
            var classDeclaration = context.Node.FindContainingClass();
            var fieldDeclaration = classDeclaration?.FindFieldNamed(identifierNameSyntax.Identifier.Text);
            if (fieldDeclaration == null) return;
            var identifierSyntax = fieldDeclaration
                .DescendantNodes<IdentifierNameSyntax>()
                .FirstOrDefault();

            var typeInfo = context.Node
                .DescendantNodes()
                .Select(n => symanticModel.GetSymbolInfo(n).Symbol as INamedTypeSymbol)
                .FirstOrDefault(nts => nts != null);

            if (!IsDisposeableOrImplementsDisposable(typeInfo)) return;

            var name = identifierSyntax.Identifier.Text;
            var location = creationSyntax.GetLocation();

            var access = context.SemanticModel.SyntaxTree.GetRoot()
                .DescendantNodes<MemberAccessExpressionSyntax>();


            var dispose = access
                .Where(a =>
                {
                    var id = a.Expression as IdentifierNameSyntax;
                    return id?.Identifier.Text == name && a.Name.Identifier.Text == DisposeMethod;
                });

            if (dispose.Any()) return;

            var diagnostic = Diagnostic.Create(Rule, location);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsDisposeableOrImplementsDisposable(INamedTypeSymbol typeInfo)
        {
            return IsIDisposable(typeInfo) || ImplementsIDisposable(typeInfo);
        }

        private static bool IsIDisposable(INamedTypeSymbol typeInfo)
        {
            return typeInfo.Name == DisposableInterface;
        }

        private static bool ImplementsIDisposable(INamedTypeSymbol typeInfo)
        {
            return typeInfo.AllInterfaces.Any(i => i.Name == DisposableInterface);
        }
    }
}
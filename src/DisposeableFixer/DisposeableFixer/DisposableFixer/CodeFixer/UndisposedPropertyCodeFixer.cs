using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Text;

namespace DisposableFixer.CodeFixer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UndisposedPropertyCodeFixer)), Shared]
    public class UndisposedPropertyCodeFixer : CodeFixProvider {
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            context.RegisterCodeFix(CodeAction.Create("Dispose property in Dispose() method", c => DisposeProperty(context, c)), context.Diagnostics);

            return Task.FromResult(1);
        }

        public static async Task<Document> DisposeProperty(CodeFixContext context, CancellationToken cancel)
        {
            var oldRoot = await context.Document.GetSyntaxRootAsync(cancel);
            var node = oldRoot.FindNode(context.Span);
            var variableName = context.Diagnostics.First().Properties[Constants.Variablename];

            ClassDeclarationSyntax oldClass;
            if (node.TryFindParentClass(out oldClass))
            {
                var model = await context.Document.GetSemanticModelAsync(cancel);
                var @classtype = model.GetEnclosingSymbol(context.Span.Start, cancel).ContainingSymbol as INamedTypeSymbol;
                if (@classtype == null) return context.Document;



                var disposeMethods = oldClass
                        .DescendantNodes<MethodDeclarationSyntax>()
                        .Where(mds => mds.Identifier.Text == "Dispose" && mds.ParameterList.Parameters.Count == 0)
                        .ToArray();
                SyntaxNode newRoot;
                if (disposeMethods.Any()) {
                    var oldDisposeMethod = disposeMethods.First();
                    var disposeCall = SyntaxFactory
                        .ExpressionStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(variableName),
                                    SyntaxFactory.IdentifierName("Dispose"))))
                        .NormalizeWhitespace();
                    var newdisposeMethod = oldDisposeMethod.AddBodyStatements(disposeCall);

                    var implementsIDisposable = @classtype.AllInterfaces.Any(i => i.GetFullNamespace() == "System.IDisposable");
                    ClassDeclarationSyntax newClass;
                    if (implementsIDisposable) {
                        newClass = oldClass;
                    } else if (oldClass.BaseList == null) {
                        newClass = SyntaxFactory
                            .ClassDeclaration(oldClass.Identifier.Text)
                            .WithModifiers(oldClass.Modifiers)
                            .WithBaseList(
                                SyntaxFactory.BaseList(
                                    SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.IdentifierName("IDisposable")))))
                            .WithMembers(oldClass.Members)
                            .NormalizeWhitespace();
                    } else {
                        var newBaseList = oldClass.BaseList.AddTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("IDisposable")).NormalizeWhitespace());
                        newClass = oldClass.ReplaceNode(oldClass.BaseList, newBaseList);
                    }

                    newClass = newClass.ReplaceNode(oldDisposeMethod, newdisposeMethod);
                    newRoot = oldRoot.ReplaceNode(oldClass, newClass);
                    
                } else {
                    var disposeMethod = SyntaxFactory
                        .MethodDeclaration(
                            SyntaxFactory.PredefinedType(
                                SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                            SyntaxFactory.Identifier("Dispose"))
                            .WithModifiers(
                                SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                            .WithBody(
                                SyntaxFactory.Block(
                                    SyntaxFactory.SingletonList<StatementSyntax>(
                                        SyntaxFactory.ExpressionStatement(
                                            SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.IdentifierName(variableName),
                                                    SyntaxFactory.IdentifierName("Dispose")))))))
                        .NormalizeWhitespace();
                    
                    var implementsIDisposable = @classtype.AllInterfaces.Any(i => i.GetFullNamespace() == "System.IDisposable");
                    ClassDeclarationSyntax newClass;
                    if (implementsIDisposable)
                    {
                        newClass = oldClass;
                    } else if (oldClass.BaseList == null) {
                        newClass = SyntaxFactory
                            .ClassDeclaration(oldClass.Identifier.Text)
                            .WithModifiers(oldClass.Modifiers)
                            .WithBaseList(
                                SyntaxFactory.BaseList(
                                    SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.IdentifierName("IDisposable")))))
                            .WithMembers(oldClass.Members)
                            .NormalizeWhitespace();
                    } else {
                        var newBaseList = oldClass.BaseList.AddTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("IDisposable")).NormalizeWhitespace());
                        newClass = oldClass.ReplaceNode(oldClass.BaseList, newBaseList);
                    }
                    
                    newRoot = oldRoot.ReplaceNode(oldClass, newClass.AddMembers(disposeMethod));
                }
                
                return context.Document.WithSyntaxRoot(newRoot);
            }
            
            return context.Document;
        }
        

        public override ImmutableArray<string> FixableDiagnosticIds { get; } = 
            ImmutableArray.Create(
                SyntaxNodeAnalysisContextExtension.IdForAssignmendFromMethodInvocationToPropertyNotDisposed,
                SyntaxNodeAnalysisContextExtension.IdForAssignmendFromObjectCreationToPropertyNotDisposed);
    }
}
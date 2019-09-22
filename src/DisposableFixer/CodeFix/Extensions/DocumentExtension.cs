using System;
using System.Linq;
using DisposableFixer.Extensions;
using DisposableFixer.Misc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DisposableFixer.CodeFix.Extensions
{
    public static class DocumentExtension
    {
        public static void AddImportIfNeeded(this DocumentEditor editor, string importedNamespace)
        {
            if(importedNamespace.Contains("."))
                throw new NotSupportedException("Only adding of toplevel namespace is supported");

            var root = editor.OriginalRoot;
            var usings = root.DescendantNodes<UsingDirectiveSyntax>()
                .ToArray();

            var existingImport = usings
                .SelectMany(u => u.DescendantNodes<IdentifierNameSyntax>())
                .Where(ins => ins.Parent is UsingDirectiveSyntax)
                .FirstOrDefault(ins => ins.Identifier.Text == importedNamespace);

            if (existingImport != null) return;

            var newSystemImport = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(Constants.System));
            editor.InsertAfter(usings.Last(), new[] { newSystemImport });
        }

        public static void AddInterfaceIfNeeded(this DocumentEditor editor, ClassDeclarationSyntax oldClass,
            IdentifierNameSyntax baseClass)
        {
            if (oldClass.BaseList != null && oldClass.BaseList.Types.Any(bts =>
                    bts.DescendantNodes<IdentifierNameSyntax>()
                        .Any(ins => ins.Identifier.Text == baseClass.Identifier.Text)))
                return;
            var model = editor.SemanticModel;
            var oc = model.GetDeclaredSymbol(oldClass);

            if (oc.AllInterfaces.Any(i => i.Name == baseClass.Identifier.Text))
            {
                return;
            }

            editor.AddInterfaceType(oldClass, baseClass);
        }

        public static void AddDisposeCallToMemberInDisposeMethod(this DocumentEditor editor,
            MethodDeclarationSyntax oldDisposeMethod, string memberName, bool castToIDisposable)
        {
            var oldStatements = oldDisposeMethod.Body.Statements;
            var disposeStatement = CreateDisposeCall(memberName, castToIDisposable);
            var newStatements = oldStatements.Add(disposeStatement);
            var newDisposeMethod = SyntaxFactory
                .MethodDeclaration(
                    SyntaxFactory.PredefinedType(
                        SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    SyntaxFactory.Identifier(Constants.Dispose))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBody(SyntaxFactory.Block(newStatements))
                .WithoutAnnotations(Formatter.Annotation);
            editor.ReplaceNode(oldDisposeMethod, newDisposeMethod);
        }

        private static ExpressionStatementSyntax CreateDisposeCall(string memberName, bool castToIDisposable)
        {
            if (castToIDisposable)
                return SyntaxFactory.ExpressionStatement(
                                    SyntaxFactory.ConditionalAccessExpression(
                                            SyntaxFactory.ParenthesizedExpression(
                                                    SyntaxFactory.BinaryExpression(
                                                            SyntaxKind.AsExpression,
                                                            SyntaxFactory.IdentifierName(memberName),
                                                            SyntaxFactory.IdentifierName(Constants.IDisposable))
                                                        .WithOperatorToken(
                                                            SyntaxFactory.Token(SyntaxKind.AsKeyword)))
                                                .WithOpenParenToken(
                                                    SyntaxFactory.Token(SyntaxKind.OpenParenToken))
                                                .WithCloseParenToken(
                                                    SyntaxFactory.Token(SyntaxKind.CloseParenToken)),
                                            SyntaxFactory.InvocationExpression(
                                                    SyntaxFactory.MemberBindingExpression(
                                                            SyntaxFactory.IdentifierName(Constants.Dispose))
                                                        .WithOperatorToken(
                                                            SyntaxFactory.Token(SyntaxKind.DotToken)))
                                                .WithArgumentList(
                                                    SyntaxFactory.ArgumentList()
                                                        .WithOpenParenToken(
                                                            SyntaxFactory.Token(SyntaxKind.OpenParenToken))
                                                        .WithCloseParenToken(
                                                            SyntaxFactory.Token(SyntaxKind.CloseParenToken))))
                                        .WithOperatorToken(
                                            SyntaxFactory.Token(SyntaxKind.QuestionToken)))
                                .WithSemicolonToken(
                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return SyntaxCreator.CreateConditionalAccessDisposeCallFor(memberName);
        }

        public static void AddDisposeMethodAndDisposeCallToMember(this DocumentEditor editor,
            ClassDeclarationSyntax oldClass, string memberName, bool castToDisposable)
        {
            var disposeMethod = SyntaxFactory
                .MethodDeclaration(
                    SyntaxFactory.PredefinedType(
                        SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    SyntaxFactory.Identifier(Constants.Dispose))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBody(
                    SyntaxFactory.Block(
                        SyntaxFactory.SingletonList<StatementSyntax>(
                            CreateDisposeCall(memberName, castToDisposable))))
                .WithoutAnnotations(Formatter.Annotation);
            editor.AddMember(oldClass, disposeMethod);
        }

        public static void AddUninitializedFieldNamed(this DocumentEditor editor, ClassDeclarationSyntax oldClass, string name, string typeName)
        {
            var fieldDeclaration = FieldDeclaration(
                                VariableDeclaration(
                                    IdentifierName(typeName))
                                .WithVariables(
                                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        VariableDeclarator(
                                            Identifier(name)))))
                            .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PrivateKeyword)))
                            .WithAdditionalAnnotations(Formatter.Annotation);
            editor.AddMember(oldClass, fieldDeclaration);
        }
    }
}
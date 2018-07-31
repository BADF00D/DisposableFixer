using System;
using System.Linq;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;

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

        public static void AddBaseTypeIfNeeded(this DocumentEditor editor, ClassDeclarationSyntax oldClass,
            IdentifierNameSyntax baseClass)
        {
            if (oldClass.BaseList != null && oldClass.BaseList.Types.Any(bts =>
                    bts.DescendantNodes<IdentifierNameSyntax>()
                        .Any(ins => ins.Identifier.Text == baseClass.Identifier.Text)))
                return;
            editor.AddBaseType(oldClass, baseClass);
        }

        public static void AddDisposeCallToMemberInDisposeMethod(this DocumentEditor editor,
            MethodDeclarationSyntax oldDisposeMethod, string memberName)
        {
            var oldStatements = oldDisposeMethod.Body.Statements;
            var disposeStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.ConditionalAccessExpression(
                    SyntaxFactory.IdentifierName(memberName),
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberBindingExpression(
                            SyntaxFactory.IdentifierName(Constants.Dispose)))));
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

        public static void AddDisposeMethodAndDisposeCallToMember(this DocumentEditor editor,
            ClassDeclarationSyntax oldClass, string memberName)
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
                            SyntaxFactory.ExpressionStatement(
                                SyntaxFactory.ConditionalAccessExpression(
                                    SyntaxFactory.IdentifierName(memberName),
                                    SyntaxFactory.InvocationExpression(SyntaxFactory.MemberBindingExpression(SyntaxFactory.IdentifierName(Constants.Dispose))))))))
                .WithoutAnnotations(Formatter.Annotation);
            editor.AddMember(oldClass, disposeMethod);
        }

        public static void AddUninitializedFieldNamed(this DocumentEditor editor, ClassDeclarationSyntax oldClass, string name, string typeName)
        {
            var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.IdentifierName(typeName)
                        )
                        .WithVariables(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(name)
                                )
                            )
                        )
                )
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PrivateKeyword)
                    )
                ).WithoutAnnotations(Formatter.Annotation);
            editor.AddMember(oldClass, fieldDeclaration);
        }
    }
}
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DisposableFixer.CodeFix.Extensions
{
    public static class ExpressionStatementSyntaxExtensions
    {
        public static LocalDeclarationStatementSyntax WrapInLocalDeclarationStatement(this ExpressionStatementSyntax expressionStatement, string variableName)
        {
            return LocalDeclarationStatement(
                VariableDeclaration(
                        IdentifierName("var"))
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(
                                    Identifier(variableName))
                                .WithInitializer(
                                    EqualsValueClause(expressionStatement.Expression)))));
        }
    }
}
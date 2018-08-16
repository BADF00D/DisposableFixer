using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Misc
{
    internal static class SyntaxCreator
    {
        public static ExpressionStatementSyntax CreateConditionalAccessDisposeCallFor(string memberName)
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.ConditionalAccessExpression(
                    SyntaxFactory.IdentifierName(memberName),
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberBindingExpression(
                            SyntaxFactory.IdentifierName(Constants.Dispose)))));
        }

        public static ExpressionStatementSyntax CreateDisposeCallFor(string memberName)
        {
            return SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(memberName),
                        SyntaxFactory.IdentifierName(Constants.Dispose))));
        }
    }
}
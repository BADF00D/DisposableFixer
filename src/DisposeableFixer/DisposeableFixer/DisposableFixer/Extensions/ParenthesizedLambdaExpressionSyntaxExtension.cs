using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class ParenthesizedLambdaExpressionSyntaxExtension
    {
        public static bool Returns(this ParenthesizedLambdaExpressionSyntax method, string name) {
            if (name == null) return false;
            return method.DescendantNodes<ReturnStatementSyntax>()
                .Select(rss => rss.DescendantNodes<IdentifierNameSyntax>())
                .Any(inss => inss.Any(ins => ins.Identifier.Text == name));
        }
    }
}
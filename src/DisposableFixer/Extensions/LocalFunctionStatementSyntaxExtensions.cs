using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class LocalFunctionStatementSyntaxExtensions
    {
        public static bool ReturnsVoid(this LocalFunctionStatementSyntax localFunction)
        {
            return localFunction?.ReturnType.IsVoid() ?? false;
        }
    }
}
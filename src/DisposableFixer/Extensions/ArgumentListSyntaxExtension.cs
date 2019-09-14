using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class ArgumentListSyntaxExtension
    {
        public static bool HasArgumentWithName(this ArgumentListSyntax als, string variableName)
        {
            return als.Arguments.Any(arg =>
                arg.Expression is SimpleNameSyntax sns && sns.Identifier.Text == variableName);
        }
    }
}
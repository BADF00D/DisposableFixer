using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Extensions
{
    public static class BlockSyntaxExtension
    {
        public static bool TryFindLastStatementThatUsesVariableWithName(this BlockSyntax block, string variableName, out StatementSyntax lastUsageStatement)
        {
            var lastUsage = block
                .DescendantNodes()
                .Last(sn =>
                {
                    switch (sn)
                    {
                        case ObjectCreationExpressionSyntax oces:
                            return oces.HasArgumentWithName(variableName);
                        case InvocationExpressionSyntax ies when ies.IsInvocationExpressionSyntaxOn(variableName):
                            return true;
                        case InvocationExpressionSyntax ies when ies.IsMemberAccessExpressionTo(variableName):
                            return true;
                        case InvocationExpressionSyntax ies when ies.ArgumentList.HasArgumentWithName(variableName):
                            return true;
                    }

                    return sn.IsVariableDeclaratorSyntaxFor(variableName);
                });
            return lastUsage.TryFindParent<StatementSyntax>(block, out lastUsageStatement);
        }
    }
}
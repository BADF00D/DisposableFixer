using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class MethodDeclarationSyntaxExtension
{
    public static bool IsDisposeMethod(this MethodDeclarationSyntax method)
    {
        return method.Identifier.Text == "Dispose"
               && method.ParameterList.Parameters.Count == 0;
    }
}
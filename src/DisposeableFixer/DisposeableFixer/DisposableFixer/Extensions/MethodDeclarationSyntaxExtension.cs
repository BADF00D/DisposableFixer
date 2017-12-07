using System.Collections.Generic;
using System.Linq;
using DisposableFixer.Configuration;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MethodCall = DisposableFixer.Configuration.MethodCall;

internal static class MethodDeclarationSyntaxExtension
{
    public static bool IsDisposeMethod(this MethodDeclarationSyntax method,
        IConfiguration configuration, SemanticModel semanticModel)
    {
        IReadOnlyCollection<MethodCall> methods;
        if (configuration.DisposingMethods.TryGetValue(method.Identifier.Text, out methods))
        {
            if (methods.Any(mc =>
            {
                var partlyEquals = mc.IsStatic == method.IsMissing
                                   && mc.Parameter.Length == method.ParameterList.Parameters.Count;
                if (!partlyEquals) return false;


                //todo optimize this, this should only be excecuted once.
                var parameterTypes = method.ParameterList.Parameters
                    .Select(p => (semanticModel.GetSymbolInfo(p.Type).Symbol as INamedTypeSymbol).GetFullNamespace())
                    .ToArray();

                return parameterTypes.SequenceEqual(mc.Parameter);
            }))
            {
                return true;
            } 
        }
        return method.AttributeLists
                    .SelectMany(als => als.Attributes)
                    .Select(a => semanticModel.GetTypeInfo(a).Type)
                    .Any(attribute => configuration.DisposingAttributes.Contains(attribute.GetFullNamespace()));
    }

    public static bool IsStatic(this MethodDeclarationSyntax method)
    {
        return method.Modifiers.Any(SyntaxKind.StaticKeyword);
    }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DisposableFixer.Configuration {
	internal interface IDetector {
		bool IsIgnoredInterface(INamedTypeSymbol named_type);
		bool IsIgnoredType(INamedTypeSymbol namedType);
		bool IsTrackedType(INamedTypeSymbol namedType, ObjectCreationExpressionSyntax node, SemanticModel semanticModel);
	}
}
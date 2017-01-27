using Microsoft.CodeAnalysis;

namespace DisposableFixer.Configuration {
	internal interface IDetector {
		bool IsIgnoredInterface(INamedTypeSymbol named_type);
		bool IsIgnoredType(INamedTypeSymbol named_type);
		bool IsTrackedType(INamedTypeSymbol named_type);
	}
}
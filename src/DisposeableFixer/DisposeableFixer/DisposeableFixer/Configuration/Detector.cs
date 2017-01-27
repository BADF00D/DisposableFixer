using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis;

namespace DisposableFixer.Configuration {
	internal sealed class Detector : IDetector {
		private readonly IConfiguration _configuration = ConfigurationManager.Instance;
		public bool IsIgnoredInterface(INamedTypeSymbol named_type) {
			var name = named_type.GetFullNamespace();
			return _configuration.IgnoredInterfaces.Contains(name);
		}

		public bool IsIgnoredType(INamedTypeSymbol named_type) {
			var name = named_type.GetFullNamespace();
			return _configuration.IgnoredTypes.Contains(name);
		}

		public bool IsTrackedType(INamedTypeSymbol named_type) {
			var name = named_type.GetFullNamespace();
			return _configuration.TrackingTypes.Contains(name);
		}
	}
}
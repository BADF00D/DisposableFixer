using System.Collections.Generic;

namespace DisposableFixer.Configuration {
	internal class DefaultConfiguration : IConfiguration {
		public DefaultConfiguration() {
			TrackingTypes = new HashSet<string> {
				"System.IO.StreamReader",
				"System.IO.StreamWriter",
				"System.IO.BinaryReader",
				"System.IO.BinaryWriter",
				"System.IO.BufferedStream",
				"System.Security.Cryptography.CryptoStream",
				"System.Resources.ResourceReader",
				"System.Resources.ResourceSet",
				"System.Resources.ResourceWriter"
			};
			IgnoredInterfaces = new HashSet<string> {
				"System.Collections.Generic.IEnumerator"
			};
			IgnoredTypes = new HashSet<string> {
				"System.Threading.Tasks.Task"
			};
		}

		public HashSet<string> IgnoredTypes { get; }
		public HashSet<string> IgnoredInterfaces { get; }
		public HashSet<string> TrackingTypes { get; }
	}
}
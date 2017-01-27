using System.Collections.Generic;

namespace DisposableFixer.Configuration {
	internal interface IConfiguration {
		HashSet<string> IgnoredTypes { get; }
		HashSet<string> IgnoredInterfaces { get; }
		HashSet<string> TrackingTypes { get; }
	}
}
using System.Collections.Generic;

namespace DisposableFixer.Configuration {
	/// <summary>
	/// Represent the current configuration. If the current configuration is changed in settings menu, this
	/// instance is changed internally to.
	/// </summary>
	internal interface IConfiguration {
		HashSet<string> IgnoredTypes { get; }
		HashSet<string> IgnoredInterfaces { get; }
		HashSet<string> TrackingTypes { get; }
		Dictionary<string, IReadOnlyCollection<CtorCall>> IgnoredTrackingTypeCtorCalls { get; }
	}
}
namespace DisposableFixer.Configuration {
	internal static class ConfigurationManager {
		public static IConfiguration Instance { get; } = new DefaultConfiguration();
	}
}
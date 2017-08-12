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
				"System.Resources.ResourceWriter",
                "Newtonsoft.Json.JsonTextWriter",
                "Newtonsoft.Json.Bson.BsonWriter",
                "Newtonsoft.Json.Bson.BsonWriter"
            };
			IgnoredInterfaces = new HashSet<string> {
				"System.Collections.Generic.IEnumerator",
                "Microsoft.Extensions.Logging.ILoggerFactory"
            };
			IgnoredTypes = new HashSet<string> {
				"System.Threading.Tasks.Task",
            };
			IgnoredTrackingTypeCtorCalls = new Dictionary<string, IReadOnlyCollection<CtorCall>> {
					["System.IO.BinaryReader"] =  new [] {
						new CtorCall(new [] {"Stream","Encoding","Boolean"}, 2, true)
                    },
					["System.IO.BinaryWriter"] = new[] {
						new CtorCall(new [] {"Stream","Encoding","Boolean"}, 2, true)
					},
					["System.IO.StreamReader"] = new[] {
						new CtorCall(new [] {"Stream", "Encoding", "Boolean", "Int32", "Boolean"},4, true)
					},
					["System.IO.StreamWriter"] = new[] {
						new CtorCall(new [] {"Stream","Encoding", "Int32","Boolean"},3, true)
					}
            };
            TrackingMethods = new Dictionary<string, IReadOnlyCollection<MethodCall>>
            {
                ["Reactive.Bindings.Extensions.IDisposableExtensions"] = new List<MethodCall>
                {
                    new MethodCall("AddTo", new [] {"T", "System.Collections.Generic.ICollection<IDisposable>"}, true)
                },
                //["TestProject.Reactive.Bindings.Extensions.IDisposableExtensions"] = new List<MethodCall>
                //{
                //    new MethodCall("AddTo", new [] {"T", "System.Collections.Generic.ICollection<IDisposable>"}, true)
                //}
            };

		    DisposingMethods = new HashSet<string>
		    {
                "Cleanup"
		    };
		}

		public HashSet<string> IgnoredTypes { get; }
		public HashSet<string> IgnoredInterfaces { get; }
		public HashSet<string> TrackingTypes { get; }
	    public HashSet<string> DisposingMethods { get; }
	    public Dictionary<string,IReadOnlyCollection<CtorCall>> IgnoredTrackingTypeCtorCalls { get; }
	    public Dictionary<string, IReadOnlyCollection<MethodCall>> TrackingMethods { get; }
	}
}
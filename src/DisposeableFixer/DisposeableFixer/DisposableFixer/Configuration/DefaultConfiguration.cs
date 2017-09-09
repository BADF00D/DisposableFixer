using System.Collections.Generic;
using DisposableFixer.Misc;

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
                "Newtonsoft.Json.Bson.BsonWriter",
                "System.Reactive.Disposables.CompositeDisposable"
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
                    new MethodCall("AddTo", new [] {"T", "System.Collections.Generic.ICollection<IDisposable>"}, true),
                },
                ["System.Reactive.Disposables.CompositeDisposable"] = new List<MethodCall>
                {
                    new MethodCall("Add", new [] {"System.IDisposable"}, false)
                },
                ["System.Diagnostics.Process"] = new List<MethodCall>
                {
                    new MethodCall("GetCurrentProcess", new string[0] , true)
                },
                ["FakeItEasy.ArgumentConstraintManagerExtensions"] = new List<MethodCall>()
                {
                    new MethodCall("Matches", new [] {"FakeItEasy.IArgumentConstraintManager<T>", "System.Linq.Expressions.Expression<System.Func<T, bool>>"}, true),
                },
                ["FakeItEasy.IArgumentConstraintManager"] = new List<MethodCall>()
                {
                    new MethodCall("Matches", new [] {"FakeItEasy.IArgumentConstraintManager<T>", "System.Linq.Expressions.Expression<System.Func<T, bool>>", "System.Action<FakeItEasy.IOutputWriter>"}, true)
                }
            };
            TrackingFactoryMethods = new Dictionary<string, IReadOnlyCollection<MethodCall>>
            {
                ["FakeItEasy.A"] = new List<MethodCall>
                {
                    new MethodCall("Fake", Empty.Array<string>(), true),
                    new MethodCall("Fake", new [] {"System.Action<FakeItEasy.Creation.IFakeOptions<T>>"}, true),
                    new MethodCall("Dummy", Empty.Array<string>(), true),
                    new MethodCall("CollectionOfFake", new [] {"Int32"}, true),
                    new MethodCall("CollectionOfFake", new [] {"Int32", "System.Action<FakeItEasy.Creation.IFakeOptions<T>>"}, true),
                    new MethodCall("CollectionOfDummy", new [] {"Int32"}, true),
                },
            };

		    DisposingMethods = new HashSet<string>
		    {
                "Cleanup"
		    };
		    DisposingAttributes = new HashSet<string>
		    {
                "NUnit.Framework.TearDownAttribute"
            };
		}
        public HashSet<string> IgnoredTypes { get; }
		public HashSet<string> IgnoredInterfaces { get; }
		public HashSet<string> TrackingTypes { get; }
	    public HashSet<string> DisposingMethods { get; }
	    public HashSet<string> DisposingAttributes { get; }
	    public Dictionary<string,IReadOnlyCollection<CtorCall>> IgnoredTrackingTypeCtorCalls { get; }
	    public Dictionary<string, IReadOnlyCollection<MethodCall>> TrackingMethods { get; }
	    public Dictionary<string, IReadOnlyCollection<MethodCall>> TrackingFactoryMethods { get; }
	}
}
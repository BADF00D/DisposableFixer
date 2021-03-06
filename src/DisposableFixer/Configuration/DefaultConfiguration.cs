﻿using System.Collections.Generic;
using DisposableFixer.Misc;
using Microsoft.CodeAnalysis;

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

                //Newtonsoft
                "Newtonsoft.Json.JsonTextWriter",
                "Newtonsoft.Json.JsonTextReader",
                "Newtonsoft.Json.Bson.BsonWriter",
                "Newtonsoft.Json.Bson.BsonReader",
                "LumenWorks.Framework.IO.Csv.CsvReader",
                "System.Xml.XmlTextReader",
                "System.Xml.XmlTextWriter",

                //Rx
                "System.Reactive.Disposables.CompositeDisposable",
                "System.Reactive.Disposables.ContextDisposable",
                "System.Reactive.Disposables.RefCountDisposable",
                "System.Reactive.Disposables.ScheduledDisposable"
            };
			IgnoredInterfaces = new HashSet<string> {
				"System.Collections.Generic.IEnumerator",
                "Microsoft.Extensions.Logging.ILoggerFactory"
            };
			IgnoredTypes = new HashSet<string> {
                "System.Threading.Tasks.Task",
                "System.Data.DataColumn",
                "System.Data.DataSet",
                "System.Data.DataTable",
                "System.Data.DataViewManager",
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
                ["System.Reactive.Disposables.DisposableMixins"] = new List<MethodCall>
                {
                    new MethodCall("DisposeWith", new [] {"T", "System.Reactive.Disposables"}, true),
                },
                ["System.Diagnostics.Process"] = new List<MethodCall>
                {
                    new MethodCall("GetCurrentProcess", new string[0] , true)
                },
                ["System.Reactive.Linq.Observable"] = new List<MethodCall>
                {
                    new MethodCall("Multicast", new []
                    {
                        "System.IObservable<T>", "System.Reactive.Subjects.ISubject<TSource, TResult>"
                    } , true)
                },
                ["FakeItEasy.ArgumentConstraintManagerExtensions"] = new List<MethodCall>()
                {
                    new MethodCall("Matches", new [] {"FakeItEasy.IArgumentConstraintManager<T>", "System.Linq.Expressions.Expression<System.Func<T, bool>>"}, true),
                },
                ["FakeItEasy.IArgumentConstraintManager"] = new List<MethodCall>()
                {
                    new MethodCall("Matches", new [] {"FakeItEasy.IArgumentConstraintManager<T>", "System.Linq.Expressions.Expression<System.Func<T, bool>>", "System.Action<FakeItEasy.IOutputWriter>"}, true)
                },
                ["Microsoft.AspNetCore.Http.HttpResponse"] = new List<MethodCall>
                {
                    new MethodCall("RegisterForDispose", new [] {"System.IDisposable"}, false)
                },
                ["Microsoft.Extensions.Logging"] = new List<MethodCall>
                {
                    new MethodCall("AddConsole", Empty.Array<string>(), true)
                }
                ,
                ["Reactive.Binding.ReactiveProperty"] = new List<MethodCall>
                {
                    new MethodCall("SetValidateNotifyError", new []{"System.Func<T,System.String>"}, false),
                    new MethodCall("SetValidateNotifyError", new []{"System.Func<T,System.Threading.Tasks.Task<System.String>>"}, false),
                    new MethodCall("SetValidateNotifyError", new []{"System.Func<T,System.Threading.Tasks.Task<System.Collections.IEnumerable>>"}, false),
                    new MethodCall("SetValidateNotifyError", new []{"System.Func<T,System.Collections.IEnumerable>"}, false),
                    new MethodCall("SetValidateNotifyError", new []{"System.Func<System.IObservable<T>,System.IObservable<System.String>>"}, false),
                    new MethodCall("SetValidateNotifyError", new []{"System.Func<System.IObservable<T>,System.IObservable<System.Collections.IEnumerable>>"}, false)
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
                ["Castle.Windsor.IWindsorContainer"] = new List<MethodCall>
                {
                    new MethodCall("AddFacility", new [] {"Castle.MicroKernel.IFacility"}, false),
                    new MethodCall("AddFacility", Empty.Array<string>(), false),
                    new MethodCall("AddFacility", new [] {"System.Action<T>"}, false),
                    new MethodCall("Install", new [] {"Castle.MicroKernel.Registration.IWindsorInstaller[]"}, false),
                    new MethodCall("Register", new [] {"Castle.MicroKernel.Registration.IRegistration[]"}, false),
                }
            };

		    DisposingMethods = new Dictionary<string, IReadOnlyCollection<MethodCall>>
		    {
                ["Cleanup"] = new List<MethodCall>
                {
                    new MethodCall("Cleanup",Empty.Array<string>(), false),
                },
                ["Dispose"] = new List<MethodCall>
                {
                    new MethodCall("Dispose", Empty.Array<string>(), false),
                    new MethodCall("Dispose", new[] {"System.Boolean"}, false),
                }
		    };
		    DisposingAttributes = new HashSet<string>
		    {
                "NUnit.Framework.TearDownAttribute"
            };
		    DisposingMethodsAtSpecialClasses = new Dictionary<string, IReadOnlyCollection<MethodCall>>
		    {
		        ["System.IO.Ports.SerialPort"] = new[]
		        {
		            new MethodCall("Close", Empty.Array<string>(), false)
		        },
                ["System.Data.SqlClient.DataTableReader"] = new[]
                {
                    new MethodCall("Close", Empty.Array<string>(), false)
                },
                ["System.Data.SqlClient.OdbcDataReader"] = new[]
                {
                    new MethodCall("Close", Empty.Array<string>(), false)
                },
                ["System.Data.SqlClient.OleDbDataReader"] = new[]
                {
                    new MethodCall("Close", Empty.Array<string>(), false)
                },
                ["System.Data.SqlClient.OracleDataReader"] = new[]
                {
                    new MethodCall("Close", Empty.Array<string>(), false)
                },
                ["System.Data.SqlClient.SqlDataReader"] = new[]
                {
                    new MethodCall("Close", Empty.Array<string>(), false)
                },
                ["System.Data.SqlClient.SqlDataReaderSmi"] = new[]
                {
                    new MethodCall("Close", Empty.Array<string>(), false)
                },
                ["System.Threading.WaitHandle"] = new []
                {
                    new MethodCall("Close", Empty.Array<string>(), false), 
                }
            };

            TrackedSet = new Dictionary<string, TrackingMode>
            {
                // mscorlib
                ["System.Threading.WaitHandle.SafeWaitHandle"] = TrackingMode.Once,
                
                // Assembly System.Net.Http
                ["System.Net.Http.HttpResponseMessage.Content"] = TrackingMode.Once,
                ["System.Net.Http.HttpResponseMessage.RequestMessage"] = TrackingMode.Once,
                ["System.Net.Http.HttpRequestMessage.Content"] = TrackingMode.Once,
                ["System.Net.Http.DelegatingHandler.InnerHandler"] = TrackingMode.Once,

                ["System.Reactive.Disposables.MultipleAssignmentDisposable.Disposable"] = TrackingMode.Always,
                ["System.Reactive.Disposables.SerialDisposable.Disposable"] = TrackingMode.Always,
                ["System.Reactive.Disposables.SingleAssignmentDisposable.Disposable"] =TrackingMode.Always,
            };
		}
        public HashSet<string> IgnoredTypes { get; }
		public HashSet<string> IgnoredInterfaces { get; }
		public HashSet<string> TrackingTypes { get; }
        public Dictionary<string, DiagnosticSeverity> TypeWithCustomSeverity { get; } = new Dictionary<string, DiagnosticSeverity>
        {
            ["System.Threading.CancellationTokenRegistration"] = DiagnosticSeverity.Info
        };
        public Dictionary<string, IReadOnlyCollection<MethodCall>> DisposingMethods { get; }
	    public Dictionary<string, IReadOnlyCollection<MethodCall>> DisposingMethodsAtSpecialClasses { get; }
	    public HashSet<string> DisposingAttributes { get; }
	    public Dictionary<string,IReadOnlyCollection<CtorCall>> IgnoredTrackingTypeCtorCalls { get; }
	    public Dictionary<string, IReadOnlyCollection<MethodCall>> TrackingMethods { get; }
	    public Dictionary<string, IReadOnlyCollection<MethodCall>> TrackingFactoryMethods { get; }
        public Dictionary<string, TrackingMode> TrackedSet { get; }
    }
}
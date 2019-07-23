using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace DisposableFixer.Configuration {
	/// <summary>
	/// Represent the current configuration. If the current configuration is changed in settings menu, this
	/// instance is changed internally to.
	/// </summary>
	internal interface IConfiguration {
		HashSet<string> IgnoredTypes { get; }
		HashSet<string> IgnoredInterfaces { get; }
		HashSet<string> TrackingTypes { get; }

        /// <summary>
        /// Types that should be disposed, but should not be marked as Error.
        /// </summary>
        Dictionary<string, DiagnosticSeverity> TypeWithCustomSeverity { get; }
		/// <summary>
        /// List of method names that are evaluated when trying to detect whether fields/properties get disposed.
        /// A call to Dispose() of fields is treated just as if there where made in Dispose method.
        /// </summary>
        Dictionary<string, IReadOnlyCollection<MethodCall>> DisposingMethods { get; }
        
        /// <summary>
        /// These are ordanary methods of arbitary clases that are handled as Dispose call. 
        /// E.g SerialPort.Close() internally calls Dispose method.
        /// The key is the full qualified class name, the MethodCall the method.
        /// </summary>
        Dictionary<string, IReadOnlyCollection<MethodCall>> DisposingMethodsAtSpecialClasses { get; }

            /// <summary>
        /// A methods marked with on of this attributes, is treated just as if this is the dispose method. 
        /// </summary>
        /// <remarks>Name of attributes should contains its namespace.</remarks>
        HashSet<string> DisposingAttributes { get; }

        /// <summary>
        /// Collection of contructors of tracking types, that dont track given disposables.
        /// </summary>
        Dictionary<string, IReadOnlyCollection<CtorCall>> IgnoredTrackingTypeCtorCalls { get; }
        /// <summary>
        /// IDisposables given to this methods are consired as automatically disposed.
        /// </summary>
        Dictionary<string, IReadOnlyCollection<MethodCall>> TrackingMethods { get; } 

        /// <summary>
        /// IDisposables delivered by on of these methods can be ignored.
        /// </summary>
        Dictionary<string, IReadOnlyCollection<MethodCall>> TrackingFactoryMethods { get; } 
	}
}
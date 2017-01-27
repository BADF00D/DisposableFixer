using System.IO;
using System.Resources;
using System.Security.Cryptography;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked {
	internal class TrackingClasses {
		public TrackingClasses() {
			using(var bla = new BinaryReader(new MemoryStream())) { }
			using (var bla = new BinaryWriter(new MemoryStream())) { }
			using (var bla = new BufferedStream(new MemoryStream())) { }
			//using (var bla = new BufferedStream(new MemoryStream())) { }//filestream
			using (var bla = new StreamReader(new MemoryStream())) { }
			using (var bla = new StreamWriter(new MemoryStream())) { }
			using (var bla = new CryptoStream(new MemoryStream(), null, CryptoStreamMode.Read)) { }
			using (var bla = new ResourceSet(new MemoryStream())) { }
			using (var bla = new ResourceReader(new MemoryStream())) { }
			using (var bla = new ResourceWriter(new MemoryStream())) { }
		}
	}

	internal class TrackingClasses2 {
		public TrackingClasses2() {
			using (var bla = new BinaryReader(new MemoryStream())) { }
		}
	}
}

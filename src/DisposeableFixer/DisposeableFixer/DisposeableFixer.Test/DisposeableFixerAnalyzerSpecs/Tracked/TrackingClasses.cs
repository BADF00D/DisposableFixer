using System.IO;
using System.Resources;
using System.Security.Cryptography;
using System.Text;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked {
	internal class TrackingClasses {
		public TrackingClasses() {
			using(var bla = new BinaryReader(new MemoryStream())) { }
			using (var bla = new BinaryWriter(new MemoryStream())) { }
			using (var bla = new BufferedStream(new MemoryStream())) { }
			//using (var bla = new BufferedStream(new MemoryStream())) { }
			using (var bla = new StreamReader(new MemoryStream())) { }
			using (var bla = new StreamWriter(new MemoryStream())) { }
			using (var bla = new CryptoStream(new MemoryStream(), null, CryptoStreamMode.Read)) { }
			using (var bla = new ResourceSet(new MemoryStream())) { }
			using (var bla = new ResourceReader(new MemoryStream())) { }
			using (var bla = new ResourceWriter(new MemoryStream())) { }

            //not dispoed MemoryStream
			using (var bla = new BinaryReader(new MemoryStream(), Encoding.UTF8, true)) { } 
			using (var bla = new BinaryWriter(new MemoryStream(), Encoding.UTF8, true)) { } 
			using (var bla = new StreamWriter(new MemoryStream(), Encoding.UTF8, 1024, true)) { } 
			using (var bla = new StreamReader(new MemoryStream(), Encoding.UTF8, true, 1024, true)) { }

            //no diagnostics
            using (var bla = new BinaryReader(new MemoryStream(), Encoding.UTF8, false)) { }
            using (var bla = new BinaryWriter(new MemoryStream(), Encoding.UTF8, false)) { }
            using (var bla = new StreamWriter(new MemoryStream(), Encoding.UTF8, 1024, false)) { }
            using (var bla = new StreamReader(new MemoryStream(), Encoding.UTF8, true, 1024, false)) { }
        }

		
	}

	internal class TrackingClasses2 {
		public TrackingClasses2() {
			using (var bla = new BinaryReader(new MemoryStream())) { }
		}
	}
}

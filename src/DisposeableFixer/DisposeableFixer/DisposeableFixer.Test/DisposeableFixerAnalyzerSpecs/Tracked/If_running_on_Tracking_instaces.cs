using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    internal class If_running_on_Tracking_instaces : DisposeableFixerAnalyzerSpec
    {



        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(CodeWith("BinaryReader"), 0)
					.SetName("mscorelib BinaryReader");
				yield return new TestCaseData(CodeWith("BinaryWriter"), 0)
					.SetName("mscorelib BinaryWriter");
				yield return new TestCaseData(CodeWith("BufferedStream"), 0)
					.SetName("mscorelib BufferedStream");
				yield return new TestCaseData(CodeWith("StreamReader"), 0)
					.SetName("mscorelib StreamReader");
				yield return new TestCaseData(CodeWith("StreamWriter"), 0)
					.SetName("mscorelib StreamWriter");
				yield return new TestCaseData(CodeWith("ResourceSet"), 0)
					.SetName("mscorelib ResourceSet");
				yield return new TestCaseData(CodeWith("ResourceReader"), 0)
					.SetName("mscorelib ResourceReader");
				yield return new TestCaseData(CodeWith("ResourceWriter"), 0)
					.SetName("mscorelib ResourceWriter");
				yield return new TestCaseData(CodeForCryptoStream(), 0)
					.SetName("mscorelib CryptoStream");//filestream is missing
			}
        }

	    private const string _CODE_1 = @"
using System.IO;
using System.Resources;
using System.Security.Cryptography;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked {
		internal class TrackingClasses2 {
			public TrackingClasses() {
				using (var bla = new ";

		private const string _CODE_2 = @"(new MemoryStream())) { }
			}
		}
	}
";

		private const string _CODE_For_CryptoStream = @"CryptoStream(new MemoryStream(), null, CryptoStreamMode.Read)) { }
			}
		}
	}
";

		private static string CodeWith(string className) {
		    return _CODE_1
					+ className
					+ _CODE_2;
	    }

		private static string CodeForCryptoStream() {
			return _CODE_1+ _CODE_For_CryptoStream;
		}


		[Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_n_Diagnostics(string code, int numberOfDisgnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
			Debug.WriteLine(code);
			diagnostics.Length.Should().Be(numberOfDisgnostics);

			
        }
    }
}
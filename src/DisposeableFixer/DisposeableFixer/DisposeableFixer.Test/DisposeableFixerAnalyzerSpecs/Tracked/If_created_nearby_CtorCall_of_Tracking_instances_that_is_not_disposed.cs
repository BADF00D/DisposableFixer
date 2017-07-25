using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    internal class If_created_nearby_CtorCall_of_Tracking_instances_that_is_not_disposed : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(CodeWith("BinaryReader"), 1)
                    .SetName("mscorelib BinaryReader");
                yield return new TestCaseData(CodeWith("BinaryWriter"), 1)
                    .SetName("mscorelib BinaryWriter");
                yield return new TestCaseData(CodeWith("BufferedStream"), 1)
                    .SetName("mscorelib BufferedStream");
                yield return new TestCaseData(CodeWith("StreamReader"), 1)
                    .SetName("mscorelib StreamReader");
                yield return new TestCaseData(CodeWith("StreamWriter"), 1)
                    .SetName("mscorelib StreamWriter");
                yield return new TestCaseData(CodeWith("ResourceSet"), 1)
                    .SetName("mscorelib ResourceSet");
                yield return new TestCaseData(CodeWith("ResourceReader"), 1)
                    .SetName("mscorelib ResourceReader");
                yield return new TestCaseData(CodeWith("ResourceWriter"), 1)
                    .SetName("mscorelib ResourceWriter");
                yield return new TestCaseData(CodeForCryptoStream(), 1)
                    .SetName("mscorelib CryptoStream"); //todo filestream is missing
            }
        }

        private const string _CODE_1 = @"
using System.IO;
using System.Resources;
using System.Security.Cryptography;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked {
		internal class TrackingClasses2 {
			public TrackingClasses() {
                var memStream = new MemoryStream();
				var bla = new ";

        private const string _CODE_2 = @"(memStream) ;
			}
		}
	}
";

        private const string _CODE_For_CryptoStream = @"CryptoStream(memStream, null, CryptoStreamMode.Read)) { }
			}
		}
	}
";

        private static string CodeWith(string className)
        {
            return _CODE_1 + className + _CODE_2;
        }

        private static string CodeForCryptoStream()
        {
            return _CODE_1 + _CODE_For_CryptoStream;
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
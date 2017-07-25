using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    internal class If_running_on_Tracking_instaces_with_optout : DisposeableFixerAnalyzerSpec
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(CodeWith("new BinaryReader(new MemoryStream(), Encoding.UTF8, true)"), 1)
                    .SetName("mscorelib BinaryReader leaveOpen=true");
                yield return new TestCaseData(CodeWith("new BinaryWriter(new MemoryStream(), Encoding.UTF8, true)"), 1)
                    .SetName("mscorelib BinaryWriter leaveOpen=true");
                yield return
                    new TestCaseData(CodeWith("new StreamWriter(new MemoryStream(), Encoding.UTF8, 1024, true)"), 1)
                        .SetName("mscorelib StreamWriter leaveOpen=true");
                yield return
                    new TestCaseData(CodeWith("new StreamReader(new MemoryStream(), Encoding.UTF8, true, 1024, true)"),
                        1)
                        .SetName("mscorelib StreamReader leaveOpen=true");

                yield return new TestCaseData(CodeWith("new BinaryReader(new MemoryStream(), Encoding.UTF8, false)"), 0)
                    .SetName("mscorelib BinaryReader leaveOpen=false");
                yield return new TestCaseData(CodeWith("new BinaryWriter(new MemoryStream(), Encoding.UTF8, false)"), 0)
                    .SetName("mscorelib BinaryWriter leaveOpen=false");
                yield return
                    new TestCaseData(CodeWith("new StreamWriter(new MemoryStream(), Encoding.UTF8, 1024, false)"), 0)
                        .SetName("mscorelib StreamWriter leaveOpen=false");
                yield return
                    new TestCaseData(
                        CodeWith("new StreamReader(new MemoryStream(), Encoding.UTF8, true, 1024, false)"), 0)
                        .SetName("mscorelib StreamReader leaveOpen=false");
            }
        }

        private const string _CODE_1 = @"
using System.IO;
using System.Resources;
using System.Security.Cryptography;
using System.Text;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked {
		internal class TrackingClasses2 {
			public TrackingClasses() {
				using (var bla = ";

        private const string _CODE_2 = @"){ }
			}
		}
	}
";

        private static string CodeWith(string className)
        {
            return _CODE_1
                   + className
                   + _CODE_2;
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
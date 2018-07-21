using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    internal class If_created_nearby_CtorCall_of_Tracking_instances_that_is_disposed : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
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
                    .SetName("mscorelib CryptoStream"); //todo filestream is missing
            }
        }

        private const string _CODE_1 = @"
using System.IO;
using System.Resources;
using System.Security.Cryptography;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked {
	internal class TrackingClasses2 {
		public TrackingClasses2() {
            var memStream = new MemoryStream();
			using (var bla = new ";

        private const string _CODE_2 = @"(memStream)) { }
		}
	}
}";
private const string _CODE_3 = @"namespace System.Resources
{
    public class ResourceSet
    {
        public ResourceSet(Stream stream) { }
    }
    public class ResourceReader : IDisposable
    {
        public ResourceReader(Stream stream) { }
        public void Dispose() { }
    }
    public class ResourceWriter : IDisposable
    {
        public ResourceWriter(Stream stream) { }
        public void Dispose() { }
    }
}
namespace System.IO
{
    public class Stream : IDisposable
    {
        public void Dispose() { }
    }

    public class MemoryStream : Stream
    {
        public void Dispose() { }
    }
    public class BinaryReader : IDisposable
    {
        public BinaryReader(Stream stream) { }
        public void Dispose() { }
    }
    public class BinaryWriter : IDisposable
    {
        public BinaryWriter(Stream stream) { }
        public void Dispose() { }
    }
    public class BufferedStream : IDisposable
    {
        public BufferedStream(Stream stream) { }
        public void Dispose() { }
    }
    public class StreamReader : IDisposable
    {
        public StreamReader(Stream stream) { }
        public void Dispose() { }
    }
    public class StreamWriter : IDisposable
    {
        public StreamWriter(Stream stream) { }
        public void Dispose() { }
    }   
}
namespace System.Security.Cryptography
{
    /// <summary>Specifies the mode of a cryptographic stream.</summary>
    public enum CryptoStreamMode
    {
        Read,
        Write,
    }
    public class CryptoStream : Stream
    {
        public CryptoStream(Stream stream, object x, CryptoStreamMode mode)
        {

        }
        public void Dispose(){}
    }
    public interface ICryptoTransform : IDisposable { }
}
";

        private const string _CODE_For_CryptoStream = @"CryptoStream(memStream, null, CryptoStreamMode.Read)) { }
			}
		}
	}
";

        private static string CodeWith(string className)
        {
            return _CODE_1 + className + _CODE_2 + _CODE_3;
        }

        private static string CodeForCryptoStream()
        {
            return _CODE_1 + _CODE_For_CryptoStream + _CODE_3;
        }


        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_n_Diagnostics(string code, int numberOfDisgnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDisgnostics);
        }
    }
}
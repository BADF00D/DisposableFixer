using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposeableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal class If_Analyser_runs_on_method_that_returns_a_MemoryStream : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest
{
    public class MethodWithMemoryStreamAsReturnValue
    {
        public MemoryStream Create(){
            return new MemoryStream();
        }
    }
}
";

        [Test]
        public void Then_there_should_be_no_Diagnostics() {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
namespace DisFixerTest.UsingBlock {
    public class ClassWithMemoryStreamDeclaredInUsingBlock {
        public MemoryStream Create() {
            return new MemoryStream();
        }
    }
}
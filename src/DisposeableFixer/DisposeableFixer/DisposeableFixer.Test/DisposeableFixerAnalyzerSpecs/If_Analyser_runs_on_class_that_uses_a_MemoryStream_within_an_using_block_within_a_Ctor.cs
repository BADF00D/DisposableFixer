using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposeableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal class If_Analyser_runs_on_class_that_uses_a_MemoryStream_within_an_using_block_within_a_Ctor : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest.UsingBlock
{
    public class ClassThatUsedMemoryStreamWithinUsingBlock
    {
        public ClassThatUsedMemoryStreamWithinUsingBlock()
        {
            var memstream = new MemoryStream();
            using (memstream) { }
        } 
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics() {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
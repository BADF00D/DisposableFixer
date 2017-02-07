using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    [TestFixture]
    internal class If_a_stream_is_created_and_stored_local_and_given_to_ctor_of_tracking_class_that_is_not_disposed :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest.UsingBlock {
    public class ClassThatUsedMemoryStreamWithinUsingBlock {
        public ClassThatUsedMemoryStreamWithinUsingBlock() {
            var memstream = new MemoryStream()
            var reader = new StreamReader(memstream));
        }
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    [TestFixture]
    internal class If_a_stream_delivered_by_a_factory_is_stored_local_and_given_to_ctor_of_tracking_class_that_id_not_disposed :
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
            var memstream = Get();
            var reader = new StreamReader(memstream));
        }
        private static Stream Get(){
            return new MemoryStream();
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
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.LocalDeclaration
{
    [TestFixture]
    internal class If_Analyser_runs_on_a_local_declared_variable_that_gets_disposed :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
using System.Threading.Tasks;
namespace DisFixerTest.Async
{
    class AsyncFileStream
    {
        public async Task<MemoryStream> Test(){
            var m = new MemoryStream();
            m.Dispose();
            return null;
        }
    }
}
";

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
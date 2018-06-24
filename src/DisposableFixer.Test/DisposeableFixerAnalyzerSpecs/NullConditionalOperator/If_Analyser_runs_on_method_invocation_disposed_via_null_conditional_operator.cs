using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.NullConditionalOperator
{
    [TestFixture]
    internal class If_Analyser_runs_on_method_invocation_disposed_via_null_conditional_operator :
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
    internal class MyClass : IDisposable
    {
        public MyClass()
        {
            Create()?.Dispose();
        }

        public IDisposable Create()
        {
            return new System.IO.MemoryStream();
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
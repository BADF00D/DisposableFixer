using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal class If_Analyser_runs_on_MethodInvokation_that_returns_a_MemoryStream_stored_in_a_field_that_gets_disposed :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest {
    public class ClassThatUsesFactoyInCtor : IDisposable {
        private readonly IDisposable _mem;
        public ClassThatUsesFactoyInCtor() {
            var factory = new Factory();

            _mem = factory.Create();
        }
        public void Dispose() {
            _mem.Dispose();
        }
    }
    class Factory {
        public MemoryStream Create() {
            return new MemoryStream();
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
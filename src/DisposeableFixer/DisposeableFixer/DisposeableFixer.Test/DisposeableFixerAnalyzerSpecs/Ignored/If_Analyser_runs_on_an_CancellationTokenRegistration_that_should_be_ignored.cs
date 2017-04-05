using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Ignored
{
    [TestFixture]
    internal class If_Analyser_runs_on_an_CancellationTokenRegistration_that_should_be_ignored :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.Threading;
namespace DisFixerTest.Ignored {
    class IgnoreCancellationtokenRegistration {
        public IgnoreCancellationtokenRegistration() {
            var token = new CancellationToken();

            var registration_not_marked = token.Register(() => { });
        }
    }
}";

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
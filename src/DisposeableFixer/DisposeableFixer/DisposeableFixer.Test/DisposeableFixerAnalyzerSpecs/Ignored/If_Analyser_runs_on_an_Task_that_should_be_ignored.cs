using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Ignored
{
    [TestFixture]
    internal class If_Analyser_runs_on_an_Task_that_should_be_ignored :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System;
using System.Threading.Tasks;
namespace DisFixerTest.Ignored {
    class IgnoreTask {
        public IgnoreTask() {
            Task.Delay(TimeSpan.FromSeconds(1));
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
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Ignored
{
    [TestFixture]
    internal class If_Analyser_runs_on_an_Enumerator_that_should_be_ignored :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.Collections.Generic;
namespace DisFixerTest.Ignored {
    class IgnoreEnumerator {
        private List<int> _list;
        public IgnoreEnumerator() {
            _list.GetEnumerator();
        } 
    }
}
";

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);

            var list = new List<int>();
            var enu = list.GetEnumerator();
        }
    }
}
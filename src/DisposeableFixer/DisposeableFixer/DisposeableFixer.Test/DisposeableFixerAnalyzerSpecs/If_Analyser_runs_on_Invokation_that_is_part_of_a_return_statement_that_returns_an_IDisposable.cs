using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal class If_Analyser_runs_on_Invokation_that_is_part_of_a_return_statement_that_returns_an_IDisposable : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.Collections;
using System.Collections.Generic;

namespace DisFixerTest.Misc {
    class EnumeratorYieldsDiagnostics : IEnumerable<int> {
        private readonly List<int> _list = new List<int>();

        public IEnumerator<int> GetEnumerator() {
            return ((IEnumerable<int>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<int>)_list).GetEnumerator();
        }
    }
}";

        [Test]
        public void Then_there_should_be_no_Diagnostics() {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
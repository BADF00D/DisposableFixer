using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_12 : IssueSpec
    {
        private readonly string _code = @"
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

        private Diagnostic[] _diagnostics;


        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(_code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
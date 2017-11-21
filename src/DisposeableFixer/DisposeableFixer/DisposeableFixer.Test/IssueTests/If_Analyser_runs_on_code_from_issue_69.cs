using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_69 : IssueSpec
    {
        private const string Code = @"
using System.Data;

namespace bla
{
    internal class SomeClass
    {
        public SomeClass()
        {
            var ds = new DataSet();
            var dt = new DataTable();
            var dv = new DataView(); //diagnostics needed
            var dc = new DataColumn();
            var dmv = new DataViewManager();
        }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_one_Diagnostic()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
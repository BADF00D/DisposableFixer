using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_11 : IssueSpec
    {
        private readonly string _code = @"
        namespace DisFixerTest.SystemIO {
            class Issue11 {
                public Issue11() {
                    var dirs = System.IO.Directory.GetDirectories(Environment.CurrentDirectory);
                    var files = System.IO.Directory.GetFiles(Environment.CurrentDirectory);
                    const string tmp = nameof(Issue11);
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
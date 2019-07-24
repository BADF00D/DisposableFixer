using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_26 : IssueSpec
    {
        private const string Code = @"
using System.IO;
namespace DisFixerTest.MethodCall {
    class MethodCallWithoutSavingReturnValue {
        public MethodCallWithoutSavingReturnValue() {
            Create();
        }
        private MemoryStream Create() {
            return new MemoryStream();
        }
    }

}";

        private Diagnostic[] _diagnostics;


        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
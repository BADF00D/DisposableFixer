using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_55 : IssueSpec
    {
        private const string Code = @"
namespace SomeNamespace
    public class SomeSpec : Spec{
        private System.IDisposable _memstream;

        public void Create(){
            _memstream = new System.IO.MemoryStream();
        }        

        public void Cleanup(){
            _memstream?.Dispose();
        }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostic()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
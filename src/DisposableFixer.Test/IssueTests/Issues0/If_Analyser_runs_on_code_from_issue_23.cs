using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_23 : IssueSpec
    {
        private const string Code = @"
using System.IO;
namespace GivenToNonDisposedTrackingInstance {
	internal class Program {
		private static void Main(string[] args) {
			var memoryStream = new MemoryStream();
			var reader = new StreamReader(memoryStream);
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
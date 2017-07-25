using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_43 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;
namespace Disposeables {
	class Program {
		static void Main(string[] args) {
			Func<MemoryStream> openStream = () => new MemoryStream();
		}
	}
}
";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private MemoryStream CD()
        {
            return new MemoryStream();
        }

        private MemoryStream CD(int i)
        {
            return new MemoryStream();
        }

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
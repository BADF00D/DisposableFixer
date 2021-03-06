using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_44 : IssueSpec
    {
        private const string Code = @"
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Disposeables {
	class Program {
		static void Main(string[] args) {
			Func<MemoryStream> openStream = () => new MemoryStream();
			Action action = () => {
				using (var stream = CreateDisposable()) {}
			};
			Action action2 = () => {
				using (var stream2 = new MemoryStream()) {}
			};
			Action action3 = () => {
				using (new MemoryStream()) { }
			};
			Action action4 = () => {
				var stream4 = CreateDisposable();
				stream4.Dispose();
			};
		}

		private static MemoryStream CreateDisposable() {
			return new MemoryStream();
		}
	}
}
";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
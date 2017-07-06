using System;
using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_45 : IssueSpec
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
				var stream = CreateDisposable();//this is not disposed
			};
			Action action2 = () => {
				var stream = CreateDisposable();
				stream.Dispose();
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
        public void Then_there_should_be_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }

    internal class Program2
    {
        static void Main(string[] args)
        {
            Func<MemoryStream> openStream = () => new MemoryStream();
            Action action = () => {
                var stream = CreateDisposable();//this is not disposed
            };
            Action action2 = () => {
                var stream = CreateDisposable();
                stream.Dispose();
            };
        }

        private static MemoryStream CreateDisposable()
        {
            return new MemoryStream();
        }
    }
}
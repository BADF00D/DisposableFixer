using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_24 : IssueSpec
    {
        private const string Code = @"
using System.IO;
namespace GivenToNonDisposedTrackingInstance {
	internal class Program {

            public IDisposable SomeMethod()
            {
                var reader = Create();//this was marked as not disposed
                return reader;
            }

            private static StreamReader Create()
            {
                var memoryStream = new MemoryStream();
                return new StreamReader(memoryStream);
            }
        }
    }
}";

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
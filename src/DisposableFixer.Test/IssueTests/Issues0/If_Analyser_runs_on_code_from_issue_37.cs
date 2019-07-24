using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_37 : IssueSpec
    {
        private const string Code = @"
using System.IO;
using System.Threading.Tasks;
namespace DisFixerTest.Async
{
    class AsyncFileStream
    {
        public Task<MemoryStream> Data()
        {
            return Task.FromResult(new MemoryStream());
        }
        public async void Test(){
            using (var s = await Data()) { }
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
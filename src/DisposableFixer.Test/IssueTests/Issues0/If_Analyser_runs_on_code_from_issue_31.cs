using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_31 : IssueSpec
    {
        private const string Code = @"
using System.IO;
using System.Threading.Tasks;
namespace DisFixerTest.Async
{
    class AsyncFileStream
    {
        public async Task<bool> Data()
        {
            var stream = await FileAsync.OpenReadAsync();

            return false;
        }
        public class FileAsync
        {
            public static async Task<MemoryStream> OpenReadAsync()
            {
                var memStream = new MemoryStream();

                return memStream;
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
        public void Then_there_should_be_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
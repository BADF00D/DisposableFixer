using System;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_86 : IssueSpec
    {
        private const string Code = @"
using System.IO;
using System.Threading.Tasks;

namespace DisFixerTest.Issue
{
    internal class Issue83
    {
        public async void Test()
        {
            MemoryStream mem;
            mem = await Create();
        }

        private Task<MemoryStream> Create()
        {
            return Task.FromResult(new MemoryStream());
        }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            Console.WriteLine(Code);

            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_one_Diagnostic_with_correct_ID()
        {
            _diagnostics.Length.Should().Be(1);
            _diagnostics[0].Id.Should().Be(Id.ForNotDisposedLocalVariable);
        }
    }
}
using System;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_88 : IssueSpec
    {
        private const string Code = @"
using System.IO;
using System.Threading.Tasks;

namespace DisFixerTest
{
    internal class SomeClass
    {
        public async void SomeMethod()
        {
            MemoryStream memoryStream;
            memoryStream = await Create();
            memoryStream.Dispose();
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
        public void Then_there_should_be_no_Diagnostic()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
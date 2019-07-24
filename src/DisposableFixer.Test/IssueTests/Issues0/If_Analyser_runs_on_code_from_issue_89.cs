using System;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_89 : IssueSpec
    {
        private const string Code = @"
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DisFixerTest
{
    public class Dummy : IDisposable
    {
        public ManualResetEvent MRE { get; } = new ManualResetEvent(false);

        public void Dispose()
        {
            this.MRE?.Dispose();
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
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_59 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace DisposeTests {
    public class TestClass {
        public TestClass()
        {
            Func<IDisposable> func = () =>
            {
                var memstream = new MemoryStream();

                return memstream;
            };
        }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostic()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
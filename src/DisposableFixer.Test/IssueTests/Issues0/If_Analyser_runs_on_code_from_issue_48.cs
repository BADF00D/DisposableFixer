using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_48 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace MyNamespace
{
    internal class MyClass : IDisposable
    {
        private readonly IDisposable _exampleDisposable;

        public MyClass()
        {
            _exampleDisposable = new MemoryStream();
        }

        public void Dispose()
        {
            _exampleDisposable?.Dispose();
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
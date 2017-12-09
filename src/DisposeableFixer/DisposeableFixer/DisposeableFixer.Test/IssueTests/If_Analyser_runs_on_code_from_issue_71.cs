
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_71 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace StoreObjectAsNonDispsable
{
    internal class AndUseAsIDisposableWithNUllPropagationToDisposeIt : IDisposable
    {
        private object _sp {get; set;}
        public AndUseAsIDisposableWithNUllPropagationToDisposeIt()
        {
            _sp = new MemoryStream();
        }

        public void Dispose()
        {
            (_sp as IDisposable)?.Dispose();
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
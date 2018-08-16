using System;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_85 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace SomeNamespace
{
    class SomeClass
    {
        public IDisposable Bla() => new MemoryStream();
        public IDisposable Bla2() => Bla();
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

using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_74 : IssueSpec
    {
        private const string Code = @"
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public MemoryStream Stream { get; private set; }
        public void SomeMethod()
        {
            Stream = new MemoryStream();
        }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_one_Diagnostic_with_correct_ID()
        {
            _diagnostics.Length.Should().Be(1);

            _diagnostics[0].Id.Should().Be(SyntaxNodeAnalysisContextExtension
                .IdForAssignmendFromObjectCreationToPropertyNotDisposed);
        }
    }
}
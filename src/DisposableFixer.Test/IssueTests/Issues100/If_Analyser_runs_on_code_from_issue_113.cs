using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_113 : IssueSpec
    {

        private const string Code = @"
using System.IO;
using System.Threading;

namespace SomeNamespace
{
    internal class SomeClass
    {
        public void Run(CancellationToken cancel)
        {
            var stream = new MemoryStream();

            var registration = cancel.Register(() => stream.Dispose());
        }
    }
}";
        [Test]
        public void Then_there_should_be_one_Diagnostics_with_Severity_Info()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Should()
                .Match<Diagnostic>(s => s.Severity == DiagnosticSeverity.Info && s.Id == Id.ForNotDisposedLocalVariable);
        }
    }
}
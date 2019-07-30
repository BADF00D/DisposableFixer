using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_93 : DisposableAnalyserCodeFixVerifierSpec
    {

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DisposableFixer.CodeFix.DisposeLocalVariableAfterLastUsageCodeFixProvider();
        }

        private const string Code = @"
using System.IO;

namespace SelectManyTest
{
    internal class LocalVariable
    {
        public LocalVariable()
        {
            var stream = new MemoryStream();
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToFix(Code);
            MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == NotDisposed.LocalVariable.ForNotDisposedLocalVariable, "this should be fixed");
            ApplyCSharpCodeFix(Code);
        }
    }
}
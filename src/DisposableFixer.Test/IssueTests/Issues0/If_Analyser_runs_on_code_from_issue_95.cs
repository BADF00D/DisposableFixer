using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_95 : DisposableAnalyserCodeFixVerifierSpec
    {

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DisposableFixer.CodeFix.UndisposedPropertyCodeFixProvider();
        }

        private const string Code = @"
using System;
using System.IO;

namespace SelectManyTest {
    internal class Issue95 : Object {
        public IDisposable MemoryStream { get; }

        public Issue95()
        {
            MemoryStream = new MemoryStream();
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToFix(Code);
            MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == Id.ForAssignment.FromObjectCreation.ToProperty.OfSameType, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we dont want to introduce bugs");

            var diagostics = MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer());
            diagostics.Should().HaveCount(0);
        }
    }
}
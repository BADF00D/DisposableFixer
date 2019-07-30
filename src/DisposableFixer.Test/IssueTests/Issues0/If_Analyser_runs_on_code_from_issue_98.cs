using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_98 : DisposableAnalyserCodeFixVerifierSpec
    {

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DisposableFixer.CodeFix.UndisposedPropertyCodeFixProvider();
        }

        private const string Code = @"
using System;
using System.IO;

namespace ExtensionMethodYieldsNotDisposed
{
    internal class Class1
    {
        public IDisposable MemoryStream { get; }

        public Class1()
        {
            MemoryStream = new MemoryStream();
        }

        private class InnerClassWithDisposeMethod
        {
            public void Dispose(){}
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToFix(Code);
            var beforeCodefixDiagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            beforeCodefixDiagnostics
                .Should().Contain(d => d.Id == Id.ForAssignmentFromObjectCreationToPropertyNotDisposed, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we don't want to introduce bugs");

            var diagnostics = MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}
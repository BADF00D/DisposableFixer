using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_110 : DisposableAnalyserCodeFixVerifierSpec
    {

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DisposableFixer.CodeFix.IntroduceFieldAndDisposeInDisposeMethodCodeFixProvider();
        }

        private const string Code = @"
using System;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public void Exchange()
        {
            var x = Create().SomeProperty;
        }

        public MyDisposable Create()
        {
            return null;
        }
    }

    public class MyDisposable : IDisposable
    {
        public int SomeProperty { get; set; }

        public void Dispose()
        {
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToFix(Code);

            var cSharpCompilerDiagnosticsBefore = GetCSharpCompilerErrors(Code);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnosticsBefore);
            cSharpCompilerDiagnosticsBefore
                .Should().HaveCount(0, "have no bug before fixing");

            var beforeCodefixDiagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            beforeCodefixDiagnostics
                .Should().Contain(d => d.Id == Id.ForAnonymousObjectFromMethodInvocation, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFixTo(Code, d => d.Id == Id.ForAnonymousObjectFromMethodInvocation);
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
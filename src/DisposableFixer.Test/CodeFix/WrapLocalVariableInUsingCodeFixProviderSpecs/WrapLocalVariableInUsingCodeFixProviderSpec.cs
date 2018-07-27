using System.Collections.Generic;
using DisposableFixer.CodeFix;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.CodeFix.WrapLocalVariableInUsingCodeFixProviderSpecs
{
    [TestFixture]
    internal class WrapLocalVariableInUsingCodeFixProviderSpec : DisposableAnalyserCodeFixVerifierSpec
    {
        private const string CodeWithUndisposedLocalVariableAndModeCode = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            var variable = Create();
            Console.WriteLine();
        }

        private static IDisposable Create()
        {
            return new MemoryStream();
        }
    }
}
";

        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return new TestCaseData(CodeWithUndisposedLocalVariableAndModeCode,
                        SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable)
                    .SetName("Local varibale and more code");
            }
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new WrapLocalVariableInUsingCodeFixProvider();
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void Should_the_applied_CodeFix_solve_the_diagnostic(string code, string preFixDiagnisticId)
        {
            PrintCodeToFix(code);
            MyHelper.RunAnalyser(code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == preFixDiagnisticId, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().NotContain(d => d.Id == preFixDiagnisticId, "this should have been fixed");
        }
    }
}
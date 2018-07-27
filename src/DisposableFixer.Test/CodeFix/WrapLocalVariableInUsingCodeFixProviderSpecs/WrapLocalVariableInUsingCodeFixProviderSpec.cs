using System.Collections.Generic;
using System.Linq;
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
            var x = 3;
        }

        private static IDisposable Create()
        {
            return new MemoryStream();
        }
    }
}
";
        private const string CodeWithUndisposedLocalVariableAndAMethodInvocation = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            var stream= new MemoryStream();
            stream.WriteByte(0);
        }
    }
}
";

        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return new TestCaseData(CodeWithUndisposedLocalVariableAndModeCode,
                        SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable)
                    .SetName("Local variable and more code");
                yield return new TestCaseData(CodeWithUndisposedLocalVariableAndAMethodInvocation,
                        SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable)
                    .SetName("Local variable and and a MethodInvocation");
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
            //arrange
            PrintCodeToFix(code);
            MyHelper.RunAnalyser(code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == preFixDiagnisticId, "this should be fixed");

            //act
            var fixedCode = ApplyCSharpCodeFix(code);
            PrintFixedCode(fixedCode);

            //assert
            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we dont want to introduce bugs");

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().NotContain(d => d.Id == preFixDiagnisticId, "this should have been fixed");
        }
    }
}
using System;
using System.Collections.Generic;
using DisposableFixer.CodeFix;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;
using TestHelper;

namespace DisposableFixer.Test.CodeFix.WrapInUsingCodeFixProviderSpecs
{
    public class WrapInUsingCodeFixProviderSpec : DisposableAnalyserCodeFixVerifierSpec
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new WrapInUsingCodeFixProvider();
        }
        
        [Test, TestCaseSource(nameof(TestCases))]
        public void test(string code, string preFixDiagnisticId)
        {
            PrintCodeToFix(code);
            MyHelper.RunAnalyser(code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == preFixDiagnisticId, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().NotContain(d => d.Id == preFixDiagnisticId, "this should have been fixed");
        }

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(codeToFix, SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation);
            }
        }

        private const string codeToFix = @"
using System.IO;

namespace SomeNamespace
{
    public class SomeClass{
        public SomeClass()
        {
            new MemoryStream();
        }
    }
}
";
    }
}
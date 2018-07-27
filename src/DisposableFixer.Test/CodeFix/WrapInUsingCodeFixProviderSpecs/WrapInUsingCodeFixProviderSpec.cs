using System.Collections.Generic;
using DisposableFixer.CodeFix;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.CodeFix.WrapInUsingCodeFixProviderSpecs
{
    public class WrapInUsingCodeFixProviderSpec : DisposableAnalyserCodeFixVerifierSpec
    {
        private const string CodeWithAnonymousObjectCreation = @"
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
        private const string CodeWithAnonymousObjectCreationAndOtherCode = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            new MemoryStream();
            Console.WriteLine();
        }
    }
}
";

        private const string CodeWithMethodInvocationAndOtherCode = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            Create();
            Console.WriteLine();
        }

        private static IDisposable Create()
        {
            return new MemoryStream();
        }
    }
}
";

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(CodeWithAnonymousObjectCreation,
                        SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                    .SetName("Anonymous ObjectCreation");
                yield return new TestCaseData(CodeWithAnonymousObjectCreationAndOtherCode,
                        SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                    .SetName("Anonymous ObjectCreation and other code");
                yield return new TestCaseData(CodeWithMethodInvocationAndOtherCode,
                        SyntaxNodeAnalysisContextExtension.IdForAnonymousMethodInvocation)
                    .SetName("Anonymous MethodInvocation and other code");
            }
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new WrapInUsingCodeFixProvider();
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
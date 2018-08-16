using System.Collections.Generic;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.CodeFix.DisposeLocalVariableAfterLastUsageCodeFixProviderSpecs
{
    [TestFixture]
    internal class DisposeLocalVariableAfterLastUsageCodeFixProvider : DisposableAnalyserCodeFixVerifierSpec
    {
        private CodeFixProvider _sut;

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DisposableFixer.CodeFix.DisposeLocalVariableAfterLastUsageCodeFixProvider();
        }

        protected override void BecauseOf()
        {
            _sut = GetCSharpCodeFixProvider();
        }

        [Test]
        public void Should_the_CodeFixProvider_be_responsible_for_undispose_local_fields()
        {
            _sut.FixableDiagnosticIds.Should()
                .Contain(SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable);
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void Should_CodeFix_work_correkt(string code, string preFixDiagnosticId)
        {
            PrintCodeToFix(code);
            MyHelper.RunAnalyser(code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == preFixDiagnosticId, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().BeEmpty();
        }

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(AnonymousObjectCreation, SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                    .SetName("Undisposed local Variable");
            }
        }

        private const string AnonymousObjectCreation = @"
using System.IO;

namespace SomeNamespace {
    internal class SomeClass {
        public void SomeMethod() {
            var memoryStream = new MemoryStream();
            var x = 0;
            var y = 1;
            memoryStream.Seek(0, SeekOrigin.Begin);
            var z = 2;
        }
    }
}
";
    }
}
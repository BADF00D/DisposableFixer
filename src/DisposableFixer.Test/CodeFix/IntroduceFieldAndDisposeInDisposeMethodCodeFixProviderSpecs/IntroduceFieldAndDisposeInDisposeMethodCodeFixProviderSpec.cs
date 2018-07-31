using System.Collections.Generic;
using DisposableFixer.CodeFix;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace DisposableFixer.Test.CodeFix.IntroduceFieldAndDisposeInDisposeMethodCodeFixProviderSpecs
{
    [TestFixture]
    internal class IntroduceFieldAndDisposeInDisposeMethodCodeFixProviderSpec : DisposableAnalyserCodeFixVerifierSpec
    {
        private CodeFixProvider _sut;

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IntroduceFieldAndDisposeInDisposeMethodCodeFixProvider();
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

        [Test]
        public void Should_the_CodeFixProvider_be_responsible_for_anonymous_objects_from_ObjectCreations()
        {
            _sut.FixableDiagnosticIds.Should()
                .Contain(SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation);
        }

        [Test]
        public void Should_the_CodeFixProvider_be_responsible_for_anonymous_objects_from_MethodInvocations()
        {
            _sut.FixableDiagnosticIds.Should()
                .Contain(SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation);
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
                    .SetName("Undisposed anonymous ObjectCreation");
                yield return new TestCaseData(AnonymousMethodInvoation, SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation)
                    .SetName("Undisposed anonymous MethodInvocation");
                yield return new TestCaseData(LocalVariable, SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable)
                    .SetName("Undisposed local variable");
                yield return new TestCaseData(AnonymousObjectCreationThatIsAArgument, SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                    .SetName("Undisposed Anonymous variable that is an argument");
            }
        }

        private const string AnonymousObjectCreation = @"
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public void MyMethod()
        {
            new MemoryStream();
        }
    }

}
";
        private const string AnonymousMethodInvoation = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public void MyMethod()
        {
            Create();
        }

        private IDisposable Create()
        {
            return new MemoryStream();
        }
    }

}
";

        private const string LocalVariable = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public void MyMethod()
        {
            var localVariable = Create();
        }

        private IDisposable Create()
        {
            return new MemoryStream();
        }
    }
}
";
        private const string AnonymousObjectCreationThatIsAArgument = @"
using System.IO;
using System.Text;

namespace Demo
{
    internal class Program
    {
        public Program()
        {
            var y = 0;
            using (var reader = new StreamReader(new MemoryStream(), Encoding.ASCII, true, 1024, true))
            {
                var x = 1;
            }
        }
    }
}
";
    }
}
using DisposableFixer.CodeFix;
using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue147_CodeFix_for_using_declaration : DisposableAnalyserCodeFixVerifierSpec
    {

        private const string Code = @"
using System;
using System.IO;
namespace SomeNamespace
{
    internal class TestClass
    {
        public void SomeMethod()
        {
            var localvariable = new MemoryStream();
        }
    }
}
";
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new WrapLocalVariableInUsingDeclarationCodeFix();
        }

        [Test]
        public void Then_there_should_be_no_diagnostic()
        {
            PrintCodeToFix(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Id.Should().Be(Id.ForNotDisposedLocalVariable, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().BeEmpty();
        }
    }
}
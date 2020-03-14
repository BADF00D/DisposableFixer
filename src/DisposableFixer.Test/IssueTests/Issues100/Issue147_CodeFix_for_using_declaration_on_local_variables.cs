using DisposableFixer.CodeFix;
using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue147_CodeFix_for_using_declaration_on_local_variables : DisposableAnalyserCodeFixVerifierSpec
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
            diagnostics[0].Id.Should().Be(Id.ForLocal.Variable, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().BeEmpty();
        }
    }

    [TestFixture]
    internal class Issue147_CodeFix_for_using_declaration_on_anonymous_objects_of_ObjectCreation : DisposableAnalyserCodeFixVerifierSpec
    {

        private const string Code = @"
using System.IO;
namespace SomeNamespace
{
    internal class TestClass
    {
        int y = 2;
        int z {get;}
        public void SomeMethod()
        {
            var memoryStream = 1;
            new MemoryStream();
        }
    }
}
";
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IntroduceLocalVariableAndUseUsingDeclaration();
        }

        [Test]
        public void Then_there_should_be_no_diagnostic()
        {
            PrintCodeToFix(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Id.Should().Be(Id.ForAnonymousObjectFromObjectCreation, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().BeEmpty();
        }
    }

    [TestFixture]
    internal class Issue147_CodeFix_for_using_declaration_on_anonymous_objects_of_MethodInvocation : DisposableAnalyserCodeFixVerifierSpec
    {

        private const string Code = @"
using System;
using System.IO;
namespace SomeNamespace
{
    internal class TestClass
    {
        int y = 2;
        int z {get;}
        public void SomeMethod()
        {
            var stream = 1;
            Stream Create () =>new MemoryStream();
            Create();
        }
    }
}
";
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IntroduceLocalVariableAndUseUsingDeclaration();
        }

        [Test]
        public void Then_there_should_be_no_diagnostic()
        {
            PrintCodeToFix(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Id.Should().Be(Id.ForAnonymousObjectFromMethodInvocation, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().BeEmpty();
        }
    }
}
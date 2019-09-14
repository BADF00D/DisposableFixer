using System.Collections.Generic;
using DisposableFixer.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
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
                .Contain(Id.ForNotDisposedLocalVariable);
        }

        [Test]
        public void Should_the_CodeFixProvider_be_responsible_for_anonymous_objects_from_ObjectCreations()
        {
            _sut.FixableDiagnosticIds.Should()
                .Contain(Id.ForAnonymousObjectFromObjectCreation);
        }

        [Test]
        public void Should_the_CodeFixProvider_be_responsible_for_anonymous_objects_from_MethodInvocations()
        {
            _sut.FixableDiagnosticIds.Should()
                .Contain(Id.ForAnonymousObjectFromMethodInvocation);
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
                yield return AnonymousObjectCreation();
                yield return AnonymousMethodInvoation();
                yield return LocalVariable();
                yield return AnonymousObjectCreationThatIsAArgument();
                yield return UndisposedAnoymousMethodInvocationWithMemberAccess();
            }
        }

        private static TestCaseData AnonymousObjectCreation()
        {
            const string code = @"
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
            return new TestCaseData(code, Id.ForAnonymousObjectFromObjectCreation)
                .SetName("Undisposed anonymous ObjectCreation");
        }


        private static TestCaseData AnonymousMethodInvoation()
        {
            const string code = @"
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
            return new TestCaseData(code, Id.ForAnonymousObjectFromMethodInvocation)
                .SetName("Undisposed anonymous MethodInvocation");
        }

        private static TestCaseData LocalVariable()
        {
            const string code = @"
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
            return new TestCaseData(code, Id.ForNotDisposedLocalVariable)
                .SetName("Undisposed local variable");
        }

        private static TestCaseData AnonymousObjectCreationThatIsAArgument()
        {
            const string code = @"
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
            return new TestCaseData(code, Id.ForAnonymousObjectFromObjectCreation)
                .SetName("Undisposed Anonymous variable that is an argument");
        }

        private static TestCaseData UndisposedAnoymousMethodInvocationWithMemberAccess()
        {
            const string code = @"
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
            return new TestCaseData(code, Id.ForAnonymousObjectFromMethodInvocation)
                .SetName("Undisposed anonymous MethodInvocation with MemberAccess");
        }
    }
}
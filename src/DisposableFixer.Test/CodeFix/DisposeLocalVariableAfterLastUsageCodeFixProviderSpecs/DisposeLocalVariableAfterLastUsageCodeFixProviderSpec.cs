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
                .Contain(Id.ForNotDisposedLocalVariable);
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
                yield return LocalVariableThatIsPartOfVariableDeclarator();
                yield return LocalVariablesThatIsPartOfAssignment();
                yield return LocalVariableInAwaitThatIsPartOfVariableDeclarator();
                yield return LocalVariableInAwaitThatIsPartOfAssignment();
            }
        }

        private static TestCaseData LocalVariableThatIsPartOfVariableDeclarator()
        {
            const string code = @"
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
}";
            return new TestCaseData(code, Id.ForNotDisposedLocalVariable)
                .SetName("Undisposed local Variable in VariableDeclarator");
        }

        private static TestCaseData LocalVariablesThatIsPartOfAssignment()
        {
            const string LocalVariablesThatIsPartOfAssignment = @"
using System.IO;

namespace SomeNamespace
{
    internal class SomeClass
    {
        public async void SomeMethod()
        {
            MemoryStream memoryStream;
            memoryStream = new MemoryStream();
            var x = 0;
            var y = 1;
            memoryStream.Seek(0, SeekOrigin.Begin);
            var z = 2;
        }
    }
}";
            return new TestCaseData(LocalVariablesThatIsPartOfAssignment, Id.ForNotDisposedLocalVariable)
                .SetName("Undisposed local Variable in Assigment");
        }

        private static TestCaseData LocalVariableInAwaitThatIsPartOfVariableDeclarator()
        {
            const string code = @"
using System.IO;
using System.Threading.Tasks;

namespace SomeNamespace
{
    internal class SomeClass
    {
        public async void SomeMethod()
        {
            var memoryStream = await Create();
            var x = 0;
            var y = 1;
            memoryStream.Seek(0, SeekOrigin.Begin);
            var z = 2;
        }

        private Task<MemoryStream> Create()
        {
            return Task.FromResult(new MemoryStream());
        }
    }
}";
            return new TestCaseData(code, Id.ForNotDisposedLocalVariable)
                .SetName("Undisposed local Variable in await in VariableDeclarator");
        }

        private static TestCaseData LocalVariableInAwaitThatIsPartOfAssignment()
        {
            const string code = @"
using System.IO;
using System.Threading.Tasks;

namespace SomeNamespace
{
    internal class SomeClass
    {
        public async void SomeMethod()
        {
            MemoryStream memoryStream;
            memoryStream = await Create();
            var x = 0;
            var y = 1;
            memoryStream.Seek(0, SeekOrigin.Begin);
            var z = 2;
        }

        private Task<MemoryStream> Create()
        {
            return Task.FromResult(new MemoryStream());
        }
    }
}";
            return new TestCaseData(code, Id.ForNotDisposedLocalVariable)
                .SetName("Undisposed local Variable in await in Assigment");
        }
    }
}
using DisposableFixer.CodeFix;
using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue143_CodeFix_for_all_Dispose_methods : DisposableAnalyserCodeFixVerifierSpec
    {

        private const string Code = @"
using System;
using System.IO;
namespace SomeNamespace
{
    internal class Spec { protected virtual void Cleanup() { } }
    internal class Spec2 : Spec { }
    internal class TestClass : Spec2
    {
        public IDisposable Mem = new MemoryStream();
        public TestClass(IObservable<int> source){}
    }
}";
        [Test]
        public void Then_there_should_be_no_diagnostic()
        {
            PrintCodeToFix(Code);

            var fixedCode = ApplyCSharpCodeFix(Code, 1);

            PrintFixedCode(fixedCode);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UndisposedFieldCodeFixProvider();
        }
    }
}
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue127_HiddenDisposable_as_argument_in_MethodInvocation : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;

namespace DisposableFixer.Test
{
    internal class MyClass
    {
        public bool Create()
        {
            using (var mem = new MemoryStream())
            {
                return Do(mem);
            }
        }

        private static bool Do(IDisposable disposable)
        {
            return true;
        }

        
    }
}";
        [Test]
        public void Then_there_should_be_no()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}
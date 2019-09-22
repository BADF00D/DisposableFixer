using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue127_HiddenDisposable_as_argument_in_CtorCall : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;

namespace DisposableFixer.Test
{
    internal class MyClass
    {
        public X Create()
        {
            using (var mem = new MemoryStream())
            {
                return new X(mem);
            }
        }

        public class X
        {
            public X(IDisposable x)
            {
            }
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
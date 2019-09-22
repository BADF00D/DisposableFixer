using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue127_HiddenDisposable_reading_Property : IssueSpec
    {

        private const string Code = @"
using System.IO;

namespace DisFixerTest.Async
{
    internal class MyClass
    {
        public bool Create()
        {
            using (var mem = new MemoryStream())
            {
                return mem.CanRead;
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
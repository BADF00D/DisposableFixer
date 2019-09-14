using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue125_not_ignored_result_of_local_function : IssueSpec
    {

        private const string Code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public MemoryStream CreateDisposable()
        {
            MemoryStream Create() => new MemoryStream();
            return Create();
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
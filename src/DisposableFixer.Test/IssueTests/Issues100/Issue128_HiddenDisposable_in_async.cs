using DisposableFixer.Test.Attributes;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    [HiddenDisposable]
    internal class Issue128_HiddenDisposable_in_async : IssueSpec
    {

        private const string Code = @"
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DisposableFixer.Test
{
    internal class MyClass
    {
        public async Task<object> Create(CancellationToken cancel)
        {
            var mem = new MemoryStream();
            await Task.Delay(1, cancel);
            return mem;
        }
    }
}";
        [Test]
        public void Then_there_should_be_no()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Id.Should().Be(Id.ForHiddenIDisposable);
        }
    }
}
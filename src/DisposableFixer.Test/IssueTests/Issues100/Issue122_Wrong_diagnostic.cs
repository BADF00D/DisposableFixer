using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue122_Wrong_diagnosticcs : IssueSpec
    {

        private const string Code = @"
using System.IO;
using System.Threading.Tasks;

namespace SomeNamespace
{
    public class SomeClass
    {
        private MemoryStream _mem;

        public async Task Do()
        {
            _mem = await Create();
        }
        public Task<MemoryStream> Create()
        {
            return Task.FromResult(new MemoryStream());
        }
    }
}";
        [Test]
        public void Then_there_should_be_one_Diagnostics_with_Severity_Info()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Should().Be(NotDisposed.Assignment.FromMethodInvocation.ToFieldNotDisposedDescriptor);
        }
    }
}
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_100 : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyNamespace
{
    class MyClass : IDisposable
    {
        private MemoryStream Field;
        private MemoryStream Property { get; set; }

        public async Task MyMethod()
        {
            Field = await CreateMemoryStream();
            Property = await CreateMemoryStream(); 
        }

        private Task<MemoryStream> CreateMemoryStream()
        {
            return Task.FromResult(new MemoryStream());
        }

        public void Dispose()
        {
            Field?.Dispose();
            Property?.Dispose();
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());

            diagnostics.Should().BeEmpty();
        }
    }
}
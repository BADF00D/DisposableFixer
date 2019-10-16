using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue133_LocalVariableHasName : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyNamespace
{
    class MyClass
    {
        private static async Task Method()
        {
            var mem = await Create();
        }
        private static Task<MemoryStream> Create ()=> throw new NotImplementedException();
    }
}
";

        [Test]
        public void Then_there_should_one_diagnostic_with_correct_message()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].GetMessage().Should().Be("Local variable 'mem' is not disposed");
        }
    }
}
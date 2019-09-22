using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue127_HiddenDisposable_with_wrong_message : IssueSpec
    {

        private const string Code = @"
using System.IO;

namespace DisposableFixer.Test
{
    internal class MyClass
    {
        public object Create()
        {
            return new MemoryStream();
        }
    }
}";
        [Test]
        public void Then_there_should_be_no()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Descriptor.MessageFormat.ToString().Should().NotContain("{0}");
            diagnostics[0].Descriptor.MessageFormat.ToString().Should().NotContain("{1}");
            diagnostics[0].Descriptor.MessageFormat.ToString().Should().NotContain("{2}");
        }
    }
}
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue127_HiddenDisposable_reading_Field: IssueSpec
    {

        private const string Code = @"
using System.IO;

namespace DisposableFixer.Test
{
    internal class MyClass
    {
        public bool Create()
        {
            using (var mem = new MemoryStream())
            {
                return new X(mem).Field;
            }
        }

        private class X
        {
            public readonly bool Field;

            public X(Stream x)
            {
                Field = x.CanRead;
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
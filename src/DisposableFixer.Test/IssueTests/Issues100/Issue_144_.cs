using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue_144 : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;

namespace SomeNamespace
{
    internal class TestClass
    {
        public TestClass()
        {
            var mem2 = new MemoryStream();
            new object()
                .DoSomething(mem2.Dispose);
        }
    }

    public static class Extensions{
        public static void DoSomething(this object @object, Action action)
        {
            throw new NotImplementedException();
        }
    }
}
";
        [Test]
        public void Then_there_should_be_no()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}
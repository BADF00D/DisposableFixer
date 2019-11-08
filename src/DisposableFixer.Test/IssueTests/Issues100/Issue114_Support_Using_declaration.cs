using DisposableFixer.Test.Attributes;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    [UsingDeclarationStatement]
    internal class Issue114_Support_Using_declaration : IssueSpec
    {

        private const string Code = @"
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            using var mem = new MemoryStream();
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
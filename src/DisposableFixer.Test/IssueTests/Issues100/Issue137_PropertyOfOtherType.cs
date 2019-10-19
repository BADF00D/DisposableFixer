using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue137_PropertyOfOtherType : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class ClassWithPublicSetableProperty : IDisposable
    {

        public IDisposable MemoryStream { get; set; }

        public void Dispose()
        {
            MemoryStream?.Dispose();
        }
    }

    class MyClass
    {
        public MyClass()
        {
            using (var instance = new ClassWithPublicSetableProperty())
            {
                instance.MemoryStream = new MemoryStream();//should generate a warning about not disposed property of other object
            }
        }
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
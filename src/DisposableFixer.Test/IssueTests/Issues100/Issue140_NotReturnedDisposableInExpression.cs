using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue140_NotReturnedDisposableInExpression : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;
using Microsoft.CodeAnalysis.Operations;

namespace SomeNamespace
{
    internal class HttpResponseMessageSpec
    {
        public void MethodWithExpressionBody1() => new MemoryStream();

        public void MethodWithStatementBody1()
        {
            void InnerMethod() => new MemoryStream();
        }

        public void MethodWithExpressionBody2() => Factory();

        public void MethodWithStatementBody2()
        {
            void InnerMethod() => Factory();
        }

        private IDisposable Factory()=> new MemoryStream();
    }
}
";

        [Test]
        public void Then_there_should_be_four_diagnostics_with_correct_message()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(4);
            //diagnostics[0].Descriptor.Should().Be(NotDisposed.Assignment.FromObjectCreation.ToProperty.OfAnotherTypeDescriptor);
        }
    }
}
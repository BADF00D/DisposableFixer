using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue108_anonymous_variable : IssueSpec
    {

        private const string Code = @"
using System;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        private IDisposable _field;

        public void Exchange() {
            using (var disposable = new SomeDisposable().CreateDisposable()) {
            }
        }
    }

    internal class SomeDisposable : IDisposable
    {
        public void Dispose()
        {
        }

        public SomeDisposable CreateDisposable()
        {
            return new SomeDisposable();
        }
    }
}";
        [Test]
        public void Then_there_should_be_one_Diagnostics_with_Severity_Info()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
        }
    }
}
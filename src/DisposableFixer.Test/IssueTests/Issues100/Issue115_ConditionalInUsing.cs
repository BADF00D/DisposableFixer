using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue115_ConditionalInUsing : IssueSpec
    {

        private const string Code = @"
using System;

internal class SomeTestNamspace
{
    Func<IDisposable> f = () => null;
    public void disposableTest(bool flag)
    {
        using (flag ? f() : f()) // whichever one we used will be disposed, but we get a warning on both
        { }
    }
}";
        [Test]
        public void Then_there_should_be_one_Diagnostics_with_Severity_Info()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}
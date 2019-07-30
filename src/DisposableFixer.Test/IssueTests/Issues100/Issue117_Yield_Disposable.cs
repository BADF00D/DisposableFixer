using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue117_Yield_Disposable : IssueSpec
    {

        private const string Code = @"
using System;
using System.Collections.Generic;
using System.Linq;

internal class SomeTestNamspace
{
    private IEnumerable<IDisposable> CreateDisposables()
    {
        foreach (var r in Enumerable.Range(1, 10))
        {
            yield return CreateDisposable();
        }
    }

    private IDisposable CreateDisposable()
    {
        return default(IDisposable);
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
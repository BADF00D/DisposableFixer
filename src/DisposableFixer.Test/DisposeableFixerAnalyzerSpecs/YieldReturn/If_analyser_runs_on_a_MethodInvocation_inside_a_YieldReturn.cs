using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.YieldReturn
{
    [TestFixture]
    internal class If_analyser_runs_on_a_MethodInvocation_inside_a_YieldReturn : DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
using System;
using System.Collections.Generic;
using System.IO;
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

    private static IDisposable CreateDisposable()
    {
        return new MemoryStream();
    }
}";

        private Diagnostic[] _diagnostics;


        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
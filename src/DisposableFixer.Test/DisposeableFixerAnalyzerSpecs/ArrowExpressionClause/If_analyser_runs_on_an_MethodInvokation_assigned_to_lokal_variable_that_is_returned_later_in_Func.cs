using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ArrowExpressionClause
{
    [TestFixture]
    internal class If_analyser_runs_on_ArrowExpressionClause_that_is : DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace SomeNamespace
{
    class SomeClass
    {
        public IDisposable Bla() => new MemoryStream();
        public IDisposable Bla2() => Bla();
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
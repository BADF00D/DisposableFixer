using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.FuncAndActions
{
    [TestFixture]
    internal class If_analyser_runs_on_an_ObjectCreation_in_LocalFunction : DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public System.IDisposable CreateDisposable()
        {
            MemoryStream Create() => new MemoryStream();
            return Create();
        }
    }
}";

        private Diagnostic[] _diagnostics;


        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(_code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            PrintCodeToFix(_code);
            _diagnostics.Length.Should().Be(0);
        }
    }
}
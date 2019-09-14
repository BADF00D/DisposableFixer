using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.FuncAndActions
{
    [TestFixture]
    internal class If_analyser_runs_on_an_MethodInvocation_in_LocalFunction : DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            MemoryStream Create() => CreateDisposableInternal();
            return Create();
        }

        private static MemoryStream CreateDisposableInternal() => new MemoryStream();
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
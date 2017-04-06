using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.InvokationExpression
{
    [TestFixture]
    internal class If_analyser_runs_on_an_disposable_created_by_factory_method_but_is_not_stored : DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
using System.IO;
namespace DisFixerTest.MethodCall {
    class MethodCallWithoutSavingReturnValue {
        public MethodCallWithoutSavingReturnValue() {
            Create();
        }
        private MemoryStream Create() {
            return new MemoryStream();
        }
    }

}";

        private Diagnostic[] _diagnostics;


        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(_code, Sut);
        }

        [Test]
        public void Then_there_should_be_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
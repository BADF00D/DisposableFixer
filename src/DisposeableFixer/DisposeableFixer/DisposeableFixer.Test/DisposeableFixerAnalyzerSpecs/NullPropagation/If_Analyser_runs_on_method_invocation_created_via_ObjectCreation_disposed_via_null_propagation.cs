using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.NullPropagation
{
    [TestFixture]
    internal class If_Analyser_runs_on_method_invocation_created_via_ObjectCreation_disposed_via_null_propagation :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System;
using System.IO;

namespace StoreObjectAsNonDispsable {
    internal class AndUseAsIDisposableWithNUllPropagationToDisposeIt {
        public AndUseAsIDisposableWithNUllPropagationToDisposeIt()
        {
            (new MemoryStream() as IDisposable)?.Dispose();
        }
    }
}
";

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
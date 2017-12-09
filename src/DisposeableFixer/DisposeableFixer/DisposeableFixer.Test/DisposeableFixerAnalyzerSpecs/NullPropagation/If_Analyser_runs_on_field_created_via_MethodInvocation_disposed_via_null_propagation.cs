using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.NullPropagation
{
    [TestFixture]
    internal class If_Analyser_runs_on_field_created_via_MethodInvocation_disposed_via_null_propagation :
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
    internal class AndUseAsIDisposableWithNUllPropagationToDisposeIt : IDisposable {
        private object _sp;
        public AndUseAsIDisposableWithNUllPropagationToDisposeIt() {
            _sp = Create();
        }

        private IDisposable Create()
        {
            return new MemoryStream();
        }

        public void Dispose() {
            (_sp as IDisposable)?.Dispose();
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
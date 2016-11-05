
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposeableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal class If_Analyser_runs_on_class_with_a_local_initialized_MemoryStream_IDisposable_field_that_is_not_disposed : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System;
using System.IO;

namespace DisFixerTest {
    internal class ClassWithUndisposedMemoryStreamAsField {
        private readonly IDisposable _memoryStream = new MemoryStream();
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics() {
            _diagnostics.Length.Should().Be(1);
        }
    }

    
}
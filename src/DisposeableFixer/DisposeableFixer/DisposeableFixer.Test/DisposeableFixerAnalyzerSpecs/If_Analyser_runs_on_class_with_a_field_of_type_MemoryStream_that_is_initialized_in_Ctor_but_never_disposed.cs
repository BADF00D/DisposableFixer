using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposeableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal class If_Analyser_runs_on_class_with_a_field_of_type_MemoryStream_that_is_initialized_in_Ctor_but_never_disposed : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest {
    class ClassWithUndisposedVariableInCtor {
        private readonly MemoryStream _memStream;
        public ClassWithUndisposedVariableInCtor() {
            _memStream = new MemoryStream();
        }
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics() {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
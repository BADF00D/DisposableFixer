using System;
using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal class
        If_Analyser_runs_on_class_with_a_field_of_type_IDisposable_that_is_initialized_in_a_method_but_never_disposed :
            DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest {
    class ClassWithUndisposedVariableInCtor {
        private IDisposable _memStream = new MemoryStream();
        public void Method() {
            _memStream = new MemoryStream();
        }
    }
}
";

        [Test]
        public void Then_there_should_be_two_Diagnostics()
        {
            _diagnostics.Length.Should().Be(2);
        }
    }
}
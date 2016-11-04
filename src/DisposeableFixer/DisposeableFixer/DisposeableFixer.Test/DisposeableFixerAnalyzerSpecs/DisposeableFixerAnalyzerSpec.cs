using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposeableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal abstract class DisposeableFixerAnalyzerSpec : Spec
    {
        protected readonly DisposeableFixerAnalyzer Sut;

        protected DisposeableFixerAnalyzerSpec()
        {
            Sut = new DisposeableFixerAnalyzer();
        }

        
    }

    [TestFixture]
    internal class If_Analyser_runs_on_class_with_no_Disposables : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.Text;

namespace DisFixerTest
{
    public class ClassWithoutDisposables
    {
        private readonly int _field = 1;

        public ClassWithoutDisposables()
        {
            Property = new object();
            var builder = new StringBuilder();
        }

        private object Property { get; }
    }
}
";

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }

    [TestFixture]
    internal class If_Analyser_runs_on_class_with_a_local_initialized_MemoryStream_field_that_is_not_disposed : DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;

namespace DisFixerTest {
    internal class ClassWithUndisposedMemoryStreamAsField {
        private readonly MemoryStream _memoryStream = new MemoryStream();
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }

    [TestFixture]
    internal class If_Analyser_runs_on_class_with_a_MemoryStream_as_field_initialized_in_ctor_that_is_not_disposed : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest {
    class ClassWithUndisposedMemoryStreamAsFieldThatIsInitializedInCtor {
        private readonly MemoryStream _memStream;
        public ClassWithUndisposedMemoryStreamAsFieldThatIsInitializedInCtor() {
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
using System.IO;
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
    internal class If_Analyser_runs_on_class_with_a_local_initialized_MemoryStream_field_that_is_not_disposed : DisposeableFixerAnalyzerSpec {
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
    [TestFixture]
    internal class If_Analyser_runs_on_class_with_a_local_initialized_MemoryStream_in_Ctor_Variable_that_is_not_disposed : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest {
    class ClassWithUndisposedVariableInCtor {
        public ClassWithUndisposedVariableInCtor() {
            var mem = new MemoryStream();
        }
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics() {
            _diagnostics.Length.Should().Be(1);
        }
    }

    [TestFixture]
    internal class If_Analyser_runs_on_class_with_that_declares_a_MemoryStream_within_an_using_block    : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest.UsingBlock
{
    public class ClassWithMemoryStreamDeclaredInUsingBlock
    {
        public ClassWithMemoryStreamDeclaredInUsingBlock()
        {
            using (var mem = new MemoryStream()){}
        } 
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics() {
            _diagnostics.Length.Should().Be(0);
        }
    }

    [TestFixture]
    internal class If_Analyser_runs_on_class_that_uses_a_MemoryStream_within_an_using_block_within_a_Ctor : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest.UsingBlock
{
    public class ClassThatUsedMemoryStreamWithinUsingBlock
    {
        public ClassThatUsedMemoryStreamWithinUsingBlock()
        {
            var memstream = new MemoryStream();
            using (memstream) { }
        } 
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics() {
            _diagnostics.Length.Should().Be(0);
        }
    }

    [TestFixture]
    internal class If_Analyser_runs_on_class_that_uses_a_MemoryStream_within_an_using_block_within_a_method : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest.UsingBlock {
    public class ClassThatUsedMemoryStreamWithinUsingBlock {
        public void SomeMethod() {
            var memstream = new MemoryStream();
            using (memstream) { }
        }
    }
}
";

        [Test]
        public void Then_there_should_be_no_Diagnostics() {
            _diagnostics.Length.Should().Be(0);
        }
    }

    [TestFixture]
    internal class If_Analyser_runs_on_class_that_has_a_using_block_in_its_method_that_is_not_used_for_the_MemoryStream : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest.UsingBlock {
    public class ClassThatUsedMemoryStreamWithinUsingBlock {
        public void SomeMethod() {
            var memstream = new MemoryStream();
            var memstream2 = new MemoryStream();
            using (memstream) { }
        }
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics() {
            _diagnostics.Length.Should().Be(1);
        }
    }

    [TestFixture]
    internal class If_Analyser_runs_on_class_that_has_a_using_block_in_its_ctor_that_is_not_used_for_the_MemoryStream : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
namespace DisFixerTest.UsingBlock {
    public class ClassThatUsedMemoryStreamWithinUsingBlock {
        public ClassThatUsedMemoryStreamWithinUsingBlock() {
            var memstream = new MemoryStream();
            var memstream2 = new MemoryStream();
            using (memstream) { }
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

namespace DisFixerTest.UsingBlock {
    public class ClassThatUsedMemoryStreamWithinUsingBlock {
        public ClassThatUsedMemoryStreamWithinUsingBlock() {
            var memstream = new MemoryStream();
            var memstream2 = new MemoryStream();
            using (memstream) { }
        }
    }
}
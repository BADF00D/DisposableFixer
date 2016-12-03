using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposeableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    internal class If_Analyser_runs_on_class_that_uses_DisposableFactory_in_Ctor : DisposeableFixerAnalyzerSpec {
        private const string Code = @"
using System.IO;
namespace DisFixerTest {
    public class ClassThatUsesFactoyInCtor {
        public ClassThatUsesFactoyInCtor() {
            var factory = new Factory();

            var mem = factory.Create();
        }
    }
    class Factory {
        public MemoryStream Create() {
            return new MemoryStream();
        }
    }
}
";
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }


        [Test]
        public void Then_there_should_be_eine_Diagnostics() {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    internal class If_Analyser_runs_on_class_that_uses_MemoryStreamFactory_used_within_an_using :
        DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
using System.IO;
namespace DisFixerTest {
    public class ClassThatUsesFactoyInCtor {
        public ClassThatUsesFactoyInCtor() {
            var factory = new Factory();

            using(var mem = factory.Create()){}
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

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }


        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
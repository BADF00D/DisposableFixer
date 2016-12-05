using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal class If_Analyser_runs_on_class_with_a_field_disposed_in_DisposeMethod :
        DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;

namespace DisFixerTest.Duplicates {
        public class OneClassWithTwoInstancesWithSameName : IDisposable{
            private readonly MemoryStream _memoryStream = new MemoryStream();
            public void Dispose()
            {
                _memoryStream.Dispose();
            }
        }
    }
";


        [Test]
        public void Then_result_should_contain_no_Diagnostics() {
            _diagnostics.Length.Should().Be(0);
        }
        }
}
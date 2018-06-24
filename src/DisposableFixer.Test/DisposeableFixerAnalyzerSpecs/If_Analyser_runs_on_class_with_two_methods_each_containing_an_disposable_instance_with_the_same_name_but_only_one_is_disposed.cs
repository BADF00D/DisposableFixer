using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal class
        If_Analyser_runs_on_class_with_two_methods_each_containing_an_disposable_instance_with_the_same_name_but_only_one_is_disposed :
            DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;

namespace DisFixerTest.Duplicates
{
    public class OneClassWithTwoInstancesWithSameName
    {
        private void Method1()
        {
            var mem = new MemoryStream();
        }
        private void Method2() {
            var mem = new MemoryStream();
            mem.Dispose();
        }
    }
}
";


        [Test]
        public void Then_result_should_contain_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
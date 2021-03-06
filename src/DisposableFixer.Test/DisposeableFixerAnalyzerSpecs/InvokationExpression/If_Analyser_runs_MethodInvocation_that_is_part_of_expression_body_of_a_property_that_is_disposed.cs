using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.InvokationExpression
{
    [TestFixture]
    internal class If_Analyser_runs_MethodInvocation_that_is_part_of_expression_body_of_a_property_that_is_disposed :
        DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
namespace SomeNamespace
     public class SomeCode : System.IDisposable{
        public System.IDisposable Property => Create();
        private static System.IDisposable Create() {
            return new System.IO.MemoryStream();
        }
        public void Dispose()
        {
            Property.Dispose();
        }
    }
}";

        private Diagnostic[] _diagnostics;


        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_one_Diagnostics_for_undisposed_property_from_objectcreation()
        {
            _diagnostics.Should().BeEmpty();
        }
    }
}
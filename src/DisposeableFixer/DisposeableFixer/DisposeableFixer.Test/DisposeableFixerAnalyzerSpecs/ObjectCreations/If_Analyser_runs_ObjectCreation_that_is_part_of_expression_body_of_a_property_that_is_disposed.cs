using System.Linq;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ObjectCreations
{
    [TestFixture]
    internal class If_Analyser_runs_ObjectCreation_that_is_part_of_expression_body_of_a_property_that_is_disposed : DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
namespace DisFixerTest.ObjectCreationAssignedToField {
    internal class EnumeratorYieldsDiagnostics : System.IDisposable {
        private System.IDisposable Subscrioption => new System.IO.MemoryStream();
        public void Dispose()
        {
            Subscrioption.Dispose();
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
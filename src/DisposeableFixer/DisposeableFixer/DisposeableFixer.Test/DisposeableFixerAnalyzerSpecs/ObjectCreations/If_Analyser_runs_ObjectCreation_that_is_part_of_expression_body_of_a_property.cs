using System.Linq;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ObjectCreations
{
    [TestFixture]
    internal class If_Analyser_runs_ObjectCreation_that_is_part_of_expression_body_of_a_property :
        DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
namespace DisFixerTest.ObjectCreationAssignedToField {
    internal class EnumeratorYieldsDiagnostics  {
        private System.IDisposable Subscrioption => new System.IO.MemoryStream();
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(_code, Sut);
        }

        [Test]
        public void Then_there_should_be_one_Diagnostics()
        {
            var diagnostic = _diagnostics.First();
            diagnostic.Descriptor.Should()
                .Be(SyntaxNodeAnalysisContextExtension.AssignmendFromObjectCreationToPropertyNotDisposedDescriptor);
        }
    }
}
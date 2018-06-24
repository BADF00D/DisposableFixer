using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ObjectCreations
{
    [TestFixture]
    internal class If_Analyser_runs_ObjectCreation_assigned_to_a_property : DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
using System;
using System.IO;

namespace DisFixerTest.ObjectCreationAssignedToField {
    internal class EnumeratorYieldsDiagnostics : IDisposable
    {
        private IDisposable _subscrioption {get; set;}

        public void Init() {
            _subscrioption = new MemoryStream();
        }
        
        public void Dispose()
        {
            _subscrioption.Dispose();
        }
    }
}";

        private Diagnostic[] _diagnostics;


        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(_code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
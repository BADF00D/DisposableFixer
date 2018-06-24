using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ObjectCreations
{
    [TestFixture]
    internal class If_Analyser_runs_ObjectCreation_that_is_disposed : DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
using System;
using System.IO;

namespace DisFixerTest.ObjectCreationAssignedToField {
    internal class EnumeratorYieldsDiagnostics
    {
        public void Init() {
            (new MemoryStream()).Dispose();    
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
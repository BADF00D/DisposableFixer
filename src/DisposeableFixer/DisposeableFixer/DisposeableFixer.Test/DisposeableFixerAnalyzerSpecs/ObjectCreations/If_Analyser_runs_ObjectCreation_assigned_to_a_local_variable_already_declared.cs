using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ObjectCreations
{
    [TestFixture]
    internal class If_Analyser_runs_ObjectCreation_assigned_to_a_local_variable_already_declared :
        DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
using System;
using System.IO;

namespace DisFixerTest.ObjectCreationAssignedToLocalVariable {
    class SeperateVariableDeclarationAndAssignment
    {
        public void DoSomething(bool check)
        {
            MemoryStream m;
            if(check){
                m = new MemoryStream();
            }else{
                m = new MemoryStream();
            }   
            m.Dispose();
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
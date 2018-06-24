using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.FuncAndActions
{
    [TestFixture]
    internal class If_analyser_runs_on_an_MethodInvokation_assigned_to_lokal_variable_that_is_returned_later_in_Func : DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace DisposeTests {
    public class TestClass {
        public TestClass() {
            Func<IDisposable> func = () =>
            {
                var memstream = Create();

                return memstream;
            };
        }

        private static IDisposable Create()
        {
            return new MemoryStream();
        } 
    }
}";

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
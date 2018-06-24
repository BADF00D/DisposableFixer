using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.InvokationExpression
{
    [TestFixture]
    internal class If_analyser_runs_on_an_method_invokation_that_assign_disposable_to_a_local_variable_declared_earlier :
        DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
using System;
using System.IO;

using System.IO;
namespace GivenToNonDisposedTrackingInstance {
	internal class Program {

            public void SomeMethod()
            {
                StreamReader sr;
                if(true){
                    sr = Create();
                }  
                else{
                    sr = Create();
                }
                sr.Dispose();
            }

            private static StreamReader Create()
            {
                var memoryStream = new MemoryStream();
                return new StreamReader(memoryStream);
            }
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
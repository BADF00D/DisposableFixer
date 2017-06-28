using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.FuncAndActions
{
    [TestFixture]
    internal class If_analyser_runs_on_an_MethodInvokation_in_ParenthesizedLambdaExpression : DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
using System;
using System.IO;

using System.IO;
namespace GivenToNonDisposedTrackingInstance {
	internal class Program {

            public void SomeMethod()
            {
                Func<MemoryStream> create = () => Create();
            }
            private MemoryStream Create(){
                return new MemoryStream();
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
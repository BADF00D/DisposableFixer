using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.InvokationExpression
{
    [TestFixture]
    internal class If_analyser_runs_on_an_method_invokation_that_assign_disposable_to_a_field :
        DisposeableFixerAnalyzerSpec
    {
        private readonly string _code = @"
using System;
using System.IO;
namespace GivenToNonDisposedTrackingInstance {
	internal class Program {
            private IDisposable _disposable;

            public void SomeMethod()
            {
                _disposable = Create();
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
        public void Then_there_should_be_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
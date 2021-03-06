using System;
using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.FuncAndActions
{
    [TestFixture]
    internal class
        If_analyser_runs_on_an_ObjectCreation_in_VariableDeclaration_in_UsingExpression_in_ParenthesizedLambdaExpression :
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
                Action create = () => {
                    using(var memStream = new MemoryStream()){}
                };
            }
        }
    }
}";

        private Diagnostic[] _diagnostics;

        public void SomeMethod()
        {
            Action create = () =>
            {
                using (var memStream = new MemoryStream())
                {
                }
            };
        }

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
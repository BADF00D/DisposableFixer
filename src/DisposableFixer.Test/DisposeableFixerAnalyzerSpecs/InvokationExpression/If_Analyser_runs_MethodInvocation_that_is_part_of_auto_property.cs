using System.Linq;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.InvokationExpression
{
    [TestFixture]
    internal class If_Analyser_runs_MethodInvocation_that_is_part_of_auto_property : DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
namespace SomeNamespace
     public class SomeCode {
        public System.IDisposable Property {get;} = Create();
        private static System.IDisposable Create() {
            return new System.IO.MemoryStream();
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
            var diagnostic = _diagnostics.First();
            diagnostic.Id.Should()
                .Be(Id.ForAssignment.FromMethodInvocation.ToProperty.OfSameType);
        }
    }
}
using System.Linq;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.InvokationExpression
{
    [TestFixture]
    internal class If_Analyser_runs_MethodInvocation_that_is_part_of_property : DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeCode
    {
        public IDisposable Property => Create();

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
            var diagnostic = _diagnostics.First();
            diagnostic.Id.Should()
                .Be(Id.ForNotDisposedFactoryProperty);
        }
    }
}
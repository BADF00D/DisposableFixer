using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Ignored
{
    [TestFixture]
    internal class If_Analyser_runs_on_field_that_is_assigned_via_ignored_static_method_call :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System;
using FakeItEasy;
namespace SomeNamespace
    public class SomeClass{
        private readonly System.IDisposable _fake = A.Fake<IDisposable>();
    }   
}

namespace FakeItEasy{
    public static class A {
        public static T Fake<T>()
        {
            throw new System.Exception();
        }   
    }
}";

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
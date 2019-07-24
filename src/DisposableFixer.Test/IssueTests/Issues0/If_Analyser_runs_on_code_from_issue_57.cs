using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_57 : IssueSpec
    {
        private const string Code = @"
using System;
using FakeItEasy;
using FakeItEasy.Creation;

namespace SomeNamespace {
    public class SomeClass {
        private readonly System.IDisposable _fake = A.Fake<IDisposable>(o => o.Strict());
    }
}

namespace FakeItEasy {
    public static class A {
        public static T Fake<T>(Action<IFakeOptions<T>> optionBuilder) {
            throw new System.Exception();
        }
    }

    public static class FakeOptionsExtensions
    {
        public static IFakeOptions<T> Strict<T>(this IFakeOptions<T> options) {
            throw new NotImplementedException();
        }    }
}

namespace FakeItEasy.Creation {
    public interface IFakeOptions<T> { }

}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostic()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
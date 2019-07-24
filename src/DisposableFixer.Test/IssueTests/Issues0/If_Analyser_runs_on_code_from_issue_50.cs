using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_50 : IssueSpec
    {
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
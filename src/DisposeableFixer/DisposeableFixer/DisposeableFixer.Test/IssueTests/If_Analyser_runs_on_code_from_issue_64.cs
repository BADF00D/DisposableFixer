using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_64 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;
namespace SomeNamespace{
    internal class Program {
        private static void Main(string[] args) {
            new Foo {Prop = memoryStream};//this causes the problem
        }
    }

    internal class Foo {
        public IDisposable Prop { get; set; }
        public Foo() { }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            /* Unfortunatelly this test is not able to reproduce the issue because currently 
             * MyHelper.RunAnalyser(Code, Sut); doesn not throw an exception. For some reason the generated
             * NullReferenceException is swallowed by Roslyn while running the analyser. 
             * If this changes in future, this test can reproduce the problem. */
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostic()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
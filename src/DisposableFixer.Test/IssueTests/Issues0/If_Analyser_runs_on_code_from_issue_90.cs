using System;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_90 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace MyNamespace
{
    public class Dummy
    {
        public object Create()
        {
            return Do(
                () => {
                    var fromObjectCreation = new System.IO.MemoryStream();
                    var fromMethodInvocation = CreateMemStream();

                    return 0;
                });
        }

        private IDisposable Do(Func<int> someFunc)
        {
            return new MemoryStream();
        }

        private IDisposable CreateMemStream()
        {
            return new MemoryStream();
        }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            Console.WriteLine(Code);

            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void There_should_be_two_undisposed_local_variables()
        {
            _diagnostics.Length.Should().Be(2);
        }

        [Test]
        public void Diagnostic_1_should_mark_an_undisposed_local_variable()
        {
            _diagnostics[0].Id.Should().Be(NotDisposed.LocalVariable.ForNotDisposedLocalVariable);
        }
        [Test]
        public void Diagnostic_2_should_mark_an_undisposed_local_variable()
        {
            _diagnostics[1].Id.Should().Be(NotDisposed.LocalVariable.ForNotDisposedLocalVariable);
        }
    }
}
using System.Xml;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_106 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;
using System.Threading;

namespace RxTimeoutTest
{
    internal class SomeClass : IDisposable
    {
        private IDisposable _field;

        public void Exchange()
        {
            var mem = new MemoryStream();

            Interlocked.Exchange(ref _field, mem)
                ?.Dispose();
        }

        public void Dispose()
        {
            _field?.Dispose();
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, Sut);
            diagnostics.Length.Should().Be(0);
        }
    }
}
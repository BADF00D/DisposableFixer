using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_68 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace UsingRegisterForDispose
{
    internal class SomeClass
    {
        public SomeClass()
        {
            var context = new MyHttpResponse();

            var stream = new MemoryStream();


            context.RegisterForDispose(stream);
        }

        private class MyHttpResponse : HttpResponse
        {
        }
    }
}

namespace Microsoft.AspNetCore.Http
{
    public abstract class HttpResponse
    {
        public virtual void RegisterForDispose(IDisposable disposable)
        {
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
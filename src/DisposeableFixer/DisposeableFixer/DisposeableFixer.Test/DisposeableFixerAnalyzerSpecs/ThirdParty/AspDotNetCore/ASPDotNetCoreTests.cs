using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.AspDotNetCore
{
    internal class ASPDotNetCoreTests : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return AFakeOfT();
            }
        }

       
        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }

        private static TestCaseData AFakeOfT()
        {
            const string code = @"using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace UsingRegisterForDispose
{
    internal class SomeClass
    {
        public SomeClass()
        {
            var response = new MyHttpResponse();
            var stream = new MemoryStream();
            response.RegisterForDispose(stream);
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



            return new TestCaseData(code, 0)
                .SetName("HttpResponseRegisterForDispose");
        }
    }
}
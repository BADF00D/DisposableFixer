using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_96 : DisposableAnalyserCodeFixVerifierSpec
    {

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DisposableFixer.CodeFix.IntroduceFieldAndDisposeInDisposeMethodCodeFixProvider();
        }

        private const string Code = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwaggerFileGenerator
{
    internal class Program
    {
        private async Task<(string route, string response)[]> GetRoutesFromServer(TestServer server)
        {
            var rootResponse = await server.CreateRequest(string.Empty).GetAsync();
            if (rootResponse.StatusCode != HttpStatusCode.MovedPermanently)
                throw new Exception(
                    ""Unable to retrieve Url to SwaggerUI. Expect root of server to contain redirect to swagger ui."");

            throw new NotImplementedException();
        }
    }

    public class TestServer : IDisposable
    {
        public void Dispose()
        {
        }

        public RequestBuilder CreateRequest(string path)
        {
            throw new NotImplementedException();
        }
    }

    public class RequestBuilder
    {
        public Task<HttpResponseMessage> GetAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class HttpResponseMessage : IDisposable
    {
        public HttpContent Content { get; set; }
        public HttpResponseHeaders Headers { get; }
        public HttpStatusCode StatusCode { get; set; }

        public void Dispose()
        {
        }
    }

    public sealed class HttpResponseHeaders : HttpHeaders
    {
    }

    public abstract class HttpHeaders : IEnumerable<KeyValuePair<string, IEnumerable<string>>>, IEnumerable
    {
        public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public enum HttpStatusCode
    {
        MovedPermanently = 301 // 0x0000012D
    }

    public abstract class HttpContent : IDisposable
    {
        public void Dispose()
        {
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToFix(Code);
            MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == NotDisposed.LocalVariable.ForNotDisposedLocalVariable, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we don't want to introduce bugs");

            var diagnostics = MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}
using System;
using DisposableFixer.Extensions;
using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
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

            var swaggerUiResponse = await server.CreateRequest(rootResponse.Headers.Location.ToString()).GetAsync();
            var swaggerUi = await swaggerUiResponse.Content.ReadAsStringAsync();

            var regex = new Regex(""discoveryPaths\\:\\sarrayFrom\\(\\\'(?<paths>([a-zA-Z\\/0-9]*))\\\'\\)."");
            var match = regex.Match(swaggerUi);
            if (!match.Success)
                throw new Exception(""Unable to retrieve routes from SwaggerUI. Maybe content changed."" + swaggerUi);

            var paths = match.Groups[""paths""].Value;

            var result = new List<(string route, string response)>();
            foreach (var path in paths.Split(','))
                using (var routeResponse = await server.CreateRequest(path).GetAsync())
                {
                    var response = await routeResponse.Content.ReadAsStringAsync();
                    result.Add((path, response));
                }

            return result.ToArray();
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
        public Uri Location { get; set; }
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

        public Task<string> ReadAsStringAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class Regex
    {
        public Regex(string pattern)
        {
        }

        public Match Match(string input)
        {
            throw new NotImplementedException();
        }
    }

    public class Match : Group
    {
        public bool Success { get; }
        public virtual GroupCollection Groups { get; }
    }

    public class Group
    {
        public string Value { get; }
    }

    /// <summary>Returns the set of captured groups in a single match.</summary>
    public class GroupCollection
    {
        public Group this[string groupname] => throw new NotImplementedException();
    }

    public class Uri
    {
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            //this bug seems to be fixed, but another bug occures at the fixed code
            PrintCodeToFix(Code);
            MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == SyntaxNodeAnalysisContextExtension.IdForNotDisposedLocalVariable, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we dont want to introduce bugs");

            var diagnostics = MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(0);
            //this bug seems to be fixed, but another bug occures at the fixed code
        }
    }
}
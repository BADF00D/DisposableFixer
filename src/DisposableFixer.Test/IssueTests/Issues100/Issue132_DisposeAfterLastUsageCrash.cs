using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using DisposableFixer.CodeFix;
using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue132_DisposeAfterLastUsageCrash : DisposableAnalyserCodeFixVerifierSpec
    {

        private const string Code = @"
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main2(string[] args)
    {
        HttpResponseMessage resp;

        using (var client = new HttpClient())
        {
            using (var stringContent = new StringContent(""data""))
            {
                resp = await client.PostAsync(""Url"", stringContent);
            }
        }
    }
}

namespace System.Net.Http
{
    public class HttpResponseMessage : IDisposable {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class HttpContent : IDisposable {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class StringContent : HttpContent
    {
        public StringContent(string data)
        {
            
        }
    }

    public class HttpClient : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            throw new NotImplementedException();
        }
    }

}
";

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DisposeLocalVariableAfterLastUsageCodeFixProvider();
        }

        [Test]
        public void Then_there_should_be_no_diagnostic()
        {
            PrintCodeToFix(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(1, "we expect one diagnostic");
            diagnostics[0].Id.Should().Be(Id.ForNotDisposedLocalVariable, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().BeEmpty();


            fixedCode.Should().Contain("resp?.Dispose();");
        }
    }
}
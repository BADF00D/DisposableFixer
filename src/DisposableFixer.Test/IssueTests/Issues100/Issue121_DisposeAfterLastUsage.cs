using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class Issue121_DisposeAfterLastUsage : DisposableAnalyserCodeFixVerifierSpec
    {

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DisposableFixer.CodeFix.DisposeLocalVariableAfterLastUsageCodeFixProvider();
        }

        private const string Code = @"
using System.IO;
using System.Threading.Tasks;

namespace Test
{
    public class DisposeAfterLastUsageDoesNotWork
    {
        public async Task Do()
        {
            var fac = new MemoryStream();
            var bytesRead = await fac.ReadAsync(new byte[10], 0, 10);
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToFix(Code);
            var beforeCodefixDiagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            beforeCodefixDiagnostics
                .Should().Contain(d => d.Id == Id.ForLocal.Variable, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we don't want to introduce bugs");

            var diagnostics = MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(0);

            var dispsoePosition = fixedCode.IndexOf("fac.Dispose");
            var lastUsagePosition = fixedCode.IndexOf("fac.");
            dispsoePosition.Should().BeGreaterThan(lastUsagePosition);
        }
    }
}
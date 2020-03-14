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
    internal class Issue130_CreateField_ConditionalCreation : DisposableAnalyserCodeFixVerifierSpec
    {

        private const string Code = @"
using System;
using System.IO;
using System.Threading;

namespace ConsoleApp
{
    class Program
    {
        private void ProcessResponses(object arg)
        {
            CancellationToken ct = (CancellationToken)arg;
            var _pipeServer = new MemoryStream();
            try
            {
                StreamWriter sw = _pipeServer != null ? new StreamWriter(_pipeServer) { AutoFlush = true } : null;
            }
            catch (Exception e)
            {
                Console.WriteLine(""Can't write to pipe or read from socket"");
            }
            finally
            {
                _pipeServer?.Dispose();
            }
        }
    }
}
";

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IntroduceFieldAndDisposeInDisposeMethodCodeFixProvider();
        }

        [Test]
        public void Then_there_should_be_no_diagnostic()
        {
            PrintCodeToFix(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Id.Should().Be(Id.ForLocal.Variable, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(Code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().BeEmpty();


            fixedCode.Should().Contain("private StreamWriter _sw;");
            var disposeFieldRegex = new Regex(@"public void Dispose\(\s*\)\s*{\s*_sw\?\.Dispose\(\);\s*}");
            disposeFieldRegex.IsMatch(fixedCode).Should().BeTrue();
        }
    }
}
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.AspDotNetCore
{
    internal partial class ASPDotNetCoreTests : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return HttpResponse_RegisterForDispose();
                yield return ILoggerFactory_AddConsole();
                yield return ILoggerFactory_AddConsole_Boolean();
                yield return ILoggerFactory_AddConsole_LogLevel();
                yield return ILoggerFactory_AddConsole_LogLevel_Boolean();
                yield return ILoggerFactory_AddConsole_Func_string_LogLevel_to_Boolean();
                yield return ILoggerFactory_AddConsole_IConsoleLoggerSettings();
                yield return ILoggerFactory_AddConsole_IConfiguration();
            }
        }

       
        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }   
    }
}
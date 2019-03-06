using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.InterlockedExcahnge
{
    [TestFixture]
    internal class If_Analyzer_runs_on_a_member_disposed_via_InterlockedExchange : DisposeableFixerAnalyzerSpec
    {
        private const string TemplateCode = @"
using System;
using System.IO;
using System.Threading;

namespace SomeNamespace
{
    internal class SomeClass : IDisposable
    {
        private IDisposable _field = new MemoryStream();
        
        public void Dispose()
        {
            #TEMPLATE#
        }
    }
}";


        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(TemplateCode.Replace("#TEMPLATE#", "Interlocked.Exchange(ref _field, null).Dispose()"))
                    .SetName("Dispose via Dispose");
                yield return new TestCaseData(TemplateCode.Replace("#TEMPLATE#", "Interlocked.Exchange(ref _field, null)?.Dispose()"))
                    .SetName("Dispose via conditional access Dispose");
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code)
        {
            PrintCodeToFix(code);
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Should().BeEmpty();
        }
    }
}
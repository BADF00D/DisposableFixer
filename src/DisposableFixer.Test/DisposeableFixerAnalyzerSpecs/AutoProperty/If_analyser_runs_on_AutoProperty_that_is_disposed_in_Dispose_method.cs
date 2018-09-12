using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.AutoProperty
{
    [TestFixture]
    internal class If_analyser_runs_on_AutoProperty_disposed_via : DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DisFixerTest
{
    public class Dummy : IDisposable
    {
        public ManualResetEvent MRE { get; } = new ManualResetEvent(false);
        public void Dispose()
        {
            ###DISPOSE###
        }
    }
}";

        [Test, TestCaseSource(nameof(TestCases))]
        public void Test(string code, int expectedTestCaseCount)
        {
            Console.WriteLine(code);
            var diagnostics = MyHelper.RunAnalyser(code, Sut);

            diagnostics.Should().HaveCount(expectedTestCaseCount);
        }

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return CodeWithDisposeBody("this.MRE.Dispose();");
                yield return CodeWithDisposeBody("this.MRE?.Dispose();");
                yield return CodeWithDisposeBody("MRE.Dispose();");
                yield return CodeWithDisposeBody("MRE?.Dispose();");
            }
        }

        private static TestCaseData CodeWithDisposeBody(string disposeCall)
        {
            var code = Code.Replace("###DISPOSE###", disposeCall);

            return new TestCaseData(code, 0).SetName(disposeCall);
        }
    }
}
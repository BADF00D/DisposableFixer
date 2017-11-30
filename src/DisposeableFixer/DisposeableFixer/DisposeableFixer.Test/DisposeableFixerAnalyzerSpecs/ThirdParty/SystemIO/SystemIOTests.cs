using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.SystemIO
{
    internal class SystemIOTests : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return CloseSerialPort();
            }
        }

        private const string Mock = @"
using System.Data;

namespace bla
{
    internal class SomeClass
    {
        public SomeClass()
        {
            ###SUT###
        }
    }
}
";

        [Test, TestCaseSource(nameof(TestCases))]
        public void The_number_of_Diagnostics_should_be_correct(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }

        private static TestCaseData CloseSerialPort()
        {
            const string code = @"var sp = new System.IO.Ports.SerialPort(); sp.Close();";

            return new TestCaseData(Mock.Replace("###SUT###", code), 0)
                .SetName("Close on SerialPort internally calls Dispose");
        }
    }
}
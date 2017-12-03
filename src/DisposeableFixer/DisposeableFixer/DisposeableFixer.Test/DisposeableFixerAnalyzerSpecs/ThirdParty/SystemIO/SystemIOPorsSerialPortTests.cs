using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.SystemIO
{
    internal class SystemIOPorsSerialPortTests : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return CloseSerialPortCreatedByObjectCreation();
                yield return CloseSerialPortOfFactoryMethod();
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void The_number_of_Diagnostics_should_be_correct(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }

        private static TestCaseData CloseSerialPortCreatedByObjectCreation()
        {
            const string code = @"
using System.Data;

namespace bla
{
    internal class SomeClass
    {
        public SomeClass()
        {
            var sp = new System.IO.Ports.SerialPort(); 
            sp.Close();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - ObjectCreation");
        }

        private static TestCaseData CloseSerialPortOfFactoryMethod() {
            const string code = @"
using System.Data;
using System.IO.Ports;

namespace bla
{
    internal class SomeClass
    {
        public SomeClass()
        {
            var sp = Create();
            sp.Close();
        }
        private SerialPort Create(){
            return new SerialPort();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - Factory method");
        }
    }
}
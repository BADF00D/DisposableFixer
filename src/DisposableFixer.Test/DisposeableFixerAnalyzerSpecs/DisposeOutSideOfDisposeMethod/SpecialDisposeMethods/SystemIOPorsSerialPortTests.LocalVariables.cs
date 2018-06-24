using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.DisposeOutSideOfDisposeMethod.SpecialDisposeMethods
{
    internal partial class SystemIOPorsSerialPortTests : DisposeableFixerAnalyzerSpec
    {
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

        private static TestCaseData CloseSerialPortOfFactoryMethod()
        {
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

        private static TestCaseData CloseSerialPortCreatedByObjectCreationWithConditionalAccess()
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
            sp?.Close();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - ObjectCreation and conditional access");
        }

        private static TestCaseData CloseSerialPortOfFactoryMethodWithConditionalAccess()
        {
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
            sp?.Close();
        }
        private SerialPort Create(){
            return new SerialPort();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - Factory method with conditional access");
        }
    }
}
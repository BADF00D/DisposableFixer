
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.DisposeOutSideOfDisposeMethod.SpecialDisposeMethods
{
    internal partial class SystemIOPorsSerialPortTests : DisposeableFixerAnalyzerSpec
	{
        private static TestCaseData CloseSerialPortOfFieldByFactoryMethod() {
            const string code = @"
using System.Data;
using System.IO.Ports;

namespace bla
{
    internal class SomeClass : IDisposable
    {
        private SerialPort _sp;
        public SomeClass()
        {
            _sp = Create();
        }
        private SerialPort Create(){
            return new SerialPort();
        }
        public void Dispose(bool heho)
        {
            _sp.Close();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - field assigned by factory method");
        }

        private static TestCaseData CloseSerialPortOfFieldByObjectCreation() {
            const string code = @"
using System.Data;
using System.IO.Ports;

namespace bla
{
    internal class SomeClass : IDisposable
    {
        private SerialPort _sp;
        public SomeClass()
        {
            _sp = new SerialPort();
        }
        public void Dispose(bool heho)
        {
            _sp.Close();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - field assigned by object creation");
        }

        private static TestCaseData CloseSerialPortOfFieldByFactoryMethodWithConditionalAccess() {
            const string code = @"
using System.Data;
using System.IO.Ports;

namespace bla
{
    internal class SomeClass : IDisposable
    {
        private SerialPort _sp;
        public SomeClass()
        {
            _sp = Create();
        }
        private SerialPort Create(){
            return new SerialPort();
        }
        public void Dispose(bool heho)
        {
            _sp?.Close();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - field assigned by factory method with conditional access");
        }

        private static TestCaseData CloseSerialPortOfFieldByObjectCreationWithConditionalAccess() {
            const string code = @"
using System.Data;
using System.IO.Ports;

namespace bla
{
    internal class SomeClass : IDisposable
    {
        private SerialPort _sp;
        public SomeClass()
        {
            _sp = new SerialPort();
        }
        public void Dispose(bool heho)
        {
            _sp?.Close();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - field assigned by object creation with conditional access");
        }
    }
}
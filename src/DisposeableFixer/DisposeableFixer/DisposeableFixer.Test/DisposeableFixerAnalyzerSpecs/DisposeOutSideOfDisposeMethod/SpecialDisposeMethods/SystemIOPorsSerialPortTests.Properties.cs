
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.DisposeOutSideOfDisposeMethod.SpecialDisposeMethods
{
    internal partial class SystemIOPorsSerialPortTests : DisposeableFixerAnalyzerSpec
	{
        private static TestCaseData CloseSerialPortOfProertyByFactoryMethod() {
            const string code = @"
using System.Data;
using System.IO.Ports;

namespace bla
{
    internal class SomeClass : IDisposable
    {
        private SerialPort _sp {get; set;}
        public SomeClass()
        {
            _sp = Create();
        }
        private SerialPort Create(){
            return new SerialPort();
        }
        public void Dispose()
        {
            _sp.Close();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - property assigned by factory method");
        }

        private static TestCaseData CloseSerialPortOfPropertyByObjectCreation() {
            const string code = @"
using System;
using System.IO.Ports;

namespace bla
{
    internal class SomeClass : IDisposable
    {
        private SerialPort _sp {get; set;}
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
                .SetName("Close on SerialPort internally calls Dispose - property assigned by object creation");
        }






        private static TestCaseData CloseSerialPortOfProertyByFactoryMethodWithConditionalAccess() {
            const string code = @"
using System.Data;
using System.IO.Ports;

namespace bla
{
    internal class SomeClass : IDisposable
    {
        private SerialPort _sp {get; set;}
        public SomeClass()
        {
            _sp = Create();
        }
        private SerialPort Create(){
            return new SerialPort();
        }
        public void Dispose()
        {
            _sp?.Close();
        }
    }
}
";
            return new TestCaseData(code, 0)
                .SetName("Close on SerialPort internally calls Dispose - property assigned by factory method with conditional access");
        }

        private static TestCaseData CloseSerialPortOfPropertyByObjectCreationWithConditionalAccess() {
            const string code = @"
using System;
using System.IO.Ports;

namespace bla
{
    internal class SomeClass : IDisposable
    {
        private SerialPort _sp {get; set;}
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
                .SetName("Close on SerialPort internally calls Dispose - property assigned by object creation with conditional access");
        }
    }
}
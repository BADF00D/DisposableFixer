using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.DisposeOutSideOfDisposeMethod.SpecialDisposeMethods
{
    internal class SystemIOPorsSerialPortTests : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return CloseSerialPortCreatedByObjectCreation();
                yield return CloseSerialPortOfFactoryMethod();
                yield return CloseSerialPortOfFieldByFactoryMethod();
                yield return CloseSerialPortOfFieldByObjectCreation();
                yield return CloseSerialPortOfPropertyByObjectCreation();
                yield return CloseSerialPortOfProertyByFactoryMethod();
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
    }
}
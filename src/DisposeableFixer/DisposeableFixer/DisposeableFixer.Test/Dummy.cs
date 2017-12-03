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

        public void Dispose()
        {
            _sp.Close();
        }
    }
}
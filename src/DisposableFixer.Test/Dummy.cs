using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public void MyMethod()
        {
            var localVariable = Create();
        }

        private IDisposable Create()
        {
            return new MemoryStream();
        }
    }

}
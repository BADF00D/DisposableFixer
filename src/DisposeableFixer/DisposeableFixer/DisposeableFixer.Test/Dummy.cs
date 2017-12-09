using System;
using System.IO;

namespace StoreObjectAsNonDispsable {
    internal class AndUseAsIDisposableWithNUllPropagationToDisposeIt {
        public AndUseAsIDisposableWithNUllPropagationToDisposeIt()
        {
            (new MemoryStream() as IDisposable)?.Dispose();
        }
    }
}
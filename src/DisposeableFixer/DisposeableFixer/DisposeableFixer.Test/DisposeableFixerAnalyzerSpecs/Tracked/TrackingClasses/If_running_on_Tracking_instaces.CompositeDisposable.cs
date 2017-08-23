using System.Collections.Generic;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked.TrackingClasses
{
    internal partial class If_running_on_Tracking_instaces
    {
        private static IEnumerable<TestCaseData> TestCasesForCompositeDisposable
        {
            get
            {
                yield return CompositeDisposableCtorThatTakesIEnumerableOfDisposablesFromObjectCreation();
                yield return CompositeDisposableCtorThatTakesParamsDisposablesFromObjectCreation();
                yield return CompositeDisposableAddToFromObjectCreation();
                yield return CompositeDisposableCtorThatTakesIEnumerableOfDisposablesFromMethodInvocation();
                yield return CompositeDisposableCtorThatTakesParamsDisposablesFromMethodInvocation();
                yield return CompositeDisposableAddToFromMethodInvocation();
                yield return CompositeDisposableCtorThatTakesIEnumerableOfDisposablesFromMultipleObjectCreation();
                yield return CompositeDisposableCtorThatTakesParamsDisposablesFromMultipleObjectCreation();
            }
        }

        private static TestCaseData CompositeDisposableAddToFromObjectCreation()
        {
            return new TestCaseData(@"
using System.IO;
using System.Reactive.Disposables;
namespace SomeNamespace
{
    public class SomeSpec
    {
        public SomeSpec()
        {
            var disposables = new CompositeDisposable();

            disposables.Add(new MemoryStream());
        }
    }
}

namespace System.Reactive.Disposables
{
    public class CompositeDisposable
    {
        public void Add(IDisposable item)
        {
        }
    }
}")
                .SetName("CompositeDisposable().AddTo(IDisposable) called with ObjectCreation");
        }

        private static TestCaseData CompositeDisposableCtorThatTakesIEnumerableOfDisposablesFromObjectCreation()
        {
            return new TestCaseData(@"
using System;
using System.IO;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace DisFixerTest.Tracking {
    class Tracking {
        public Tracking() {
            var disposables = new CompositeDisposable(new [] {new MemoryStream()});
            disposables.Dispose();
        }
    }
}
namespace System.Reactive.Disposables {
    public class CompositeDisposable : IDisposable {
        public CompositeDisposable(IEnumerable<IDisposable> values) { }
        public void Dispose() {
        }
    }
}
")
                .SetName("new CompositeDisposable(IEnumerable<IDisposable>) called with ObjectCreation");
        }

        private static TestCaseData CompositeDisposableCtorThatTakesParamsDisposablesFromObjectCreation()
        {
            return new TestCaseData(@"
using System;
using System.IO;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace DisFixerTest.Tracking {
    class Tracking {
        public Tracking() {
            var disposables = new CompositeDisposable(new MemoryStream());
            disposables.Dispose();
        }
    }
}
namespace System.Reactive.Disposables {
    public class CompositeDisposable : IDisposable {
        public CompositeDisposable(params IDisposable[] values) { }
        public void Dispose() {
        }
    }
}
")
                .SetName("new CompositeDisposable(params IDisposable[]) called with ObjectCreation");
        }


        private static TestCaseData CompositeDisposableAddToFromMethodInvocation()
        {
            return new TestCaseData(@"
using System;
using System.IO;
using System.Reactive.Disposables;
namespace SomeNamespace
{
    public class SomeSpec
    {
        public SomeSpec()
        {
            var disposables = new CompositeDisposable();

            disposables.Add(Create());
        }
        private IDisposable Create(){
            return new MemoryStream();
        }
    }
}

namespace System.Reactive.Disposables
{
    public class CompositeDisposable
    {
        public void Add(IDisposable item)
        {
        }
    }
}")
                .SetName("CompositeDisposable().AddTo(IDisposable) called with MethodInvocation");
        }

        private static TestCaseData CompositeDisposableCtorThatTakesIEnumerableOfDisposablesFromMethodInvocation()
        {
            return new TestCaseData(@"
using System;
using System.IO;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace DisFixerTest.Tracking {
    class Tracking {
        public Tracking() {
            var disposables = new CompositeDisposable(new [] {Create()});
            disposables.Dispose();
        }
        private IDisposable Create(){
            return new MemoryStream();
        }
    }
}
namespace System.Reactive.Disposables {
    public class CompositeDisposable : IDisposable {
        public CompositeDisposable(IEnumerable<IDisposable> values) { }
        public void Dispose() {
        }
    }
}")
                .SetName("new CompositeDisposable(IEnumerable<IDisposable>) called with MethodInvocation");
        }

        private static TestCaseData CompositeDisposableCtorThatTakesParamsDisposablesFromMethodInvocation()
        {
            return new TestCaseData(@"
using System;
using System.IO;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace DisFixerTest.Tracking {
    class Tracking {
        public Tracking() {
            var disposables = new CompositeDisposable(Create());
            disposables.Dispose();
        }
        private IDisposable Create(){
            return new MemoryStream();
        }
    }
}
namespace System.Reactive.Disposables {
    public class CompositeDisposable : IDisposable {
        public CompositeDisposable(params IDisposable[] values) { }
        public void Dispose() {
        }
    }
}")
                .SetName("new CompositeDisposable(params IDisposable[]) called with MethodInvocation");
        }


        private static TestCaseData CompositeDisposableCtorThatTakesIEnumerableOfDisposablesFromMultipleObjectCreation()
        {
            return new TestCaseData(@"
using System;
using System.IO;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace DisFixerTest.Tracking {
    class Tracking {
        public Tracking() {
            var disposables = new CompositeDisposable(new [] {new MemoryStream(), new MemoryStream()});
            disposables.Dispose();
        }
    }
}
namespace System.Reactive.Disposables {
    public class CompositeDisposable : IDisposable {
        public CompositeDisposable(IEnumerable<IDisposable> values) { }
        public void Dispose() {
        }
    }
}")
                .SetName("new CompositeDisposable(IEnumerable<IDisposable>) called with multiple ObjectCreation");
        }

        private static TestCaseData CompositeDisposableCtorThatTakesParamsDisposablesFromMultipleObjectCreation()
        {
            return new TestCaseData(@"
using System;
using System.IO;
using System.Collections.Generic;
using System.Reactive.Disposables;

namespace DisFixerTest.Tracking {
    class Tracking {
        public Tracking() {
            var disposables = new CompositeDisposable(new MemoryStream(), new MemoryStream());
            disposables.Dispose();
        }
    }
}
namespace System.Reactive.Disposables {
    public class CompositeDisposable : IDisposable {
        public CompositeDisposable(params IDisposable[] values) { }
        public void Dispose() {
        }
    }
}")
                .SetName("new CompositeDisposable(params IDisposable[]) called with multiple ObjectCreation");
        }
    }
}
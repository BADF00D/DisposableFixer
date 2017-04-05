using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Using
{
    internal class If_disposables_is_using_within_using : DisposeableFixerAnalyzerSpec
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(LocalDeclarationWithObjectCreationThatIsCorrectlyDisposed, 0)
                    .SetName(
                        "LocalDeclaration with ObjectCreation given to a non tracking instance that is correctly disposed")
                    ;
                yield return new TestCaseData(FactoryCallThatIsCorrectlyDisposed, 0)
                    .SetName("FactoryCall that is correctly disposed");
                yield return new TestCaseData(ObjectCreationInUsingThatIsCorrectlyDisposed, 0)
                    .SetName("ObjectCreation in using that is correctly disposed");
                yield return new TestCaseData(FactoryCallInUsingThatIsCorrectlyDisposed, 0)
                    .SetName("FactoryCall in using that is correctly disposed");
                yield return new TestCaseData(ThreeDisablesInsideUsingBlock, 3)
                    .SetName("Three disposable inside using block");
            }
        }

        private const string LocalDeclarationWithObjectCreationThatIsCorrectlyDisposed = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking {
    class Test : IDisposable {
        public static void Do() {
            var mem = new MemoryStream();

            using(mem) { }
        }
    }
}
";

        private const string ObjectCreationInUsingThatIsCorrectlyDisposed = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking {
    class Test : IDisposable {
        public static void Do() {
            using(new MemoryStream()) { }
        }
    }
}
";

        private const string FactoryCallThatIsCorrectlyDisposed = @"
using System;
using System.IO;
namespace DisFixerTest.Tracking {
    class Test : IDisposable {
        public static void Do() {
            var factory = new MemStreamFactory();
            var mem = factory.Create();

            using (mem) { }
        }
    }

    internal class MemStreamFactory
    {
        public MemoryStream Create()
        {
            return new MemoryStream();
        }
    }
}
";

        private const string FactoryCallInUsingThatIsCorrectlyDisposed = @"
using System;
using System.IO;
namespace DisFixerTest.Tracking {
    class NoneTracking : IDisposable {
        public static void Do() {
            var factory = new MemStreamFactory();
            using (factory.Create()) { }
        }
    }

    internal class MemStreamFactory
    {
        public MemoryStream Create()
        {
            return new MemoryStream();
        }
    }
}
";

        private const string ThreeDisablesInsideUsingBlock = @"
using System;
using System.IO;
namespace DisFixerTest.ObjectCreation {
    class ObjectCreationInUsingBlock {
        public ObjectCreationInUsingBlock() {
            using(var memStream = new MemoryStream()) 
            {
                new MemoryStream(); //this should be marked as not disposed
                var tmp = new MemoryStream(); //this should be marked as not disposed
                var tmp2 = Create();//this should be marked as not disposed
            }
        }
        private IDisposable Create() {
            return new MemoryStream();
        }
    }
}
";

        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }
    }
}
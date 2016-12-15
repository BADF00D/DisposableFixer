using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    internal class If_disposables_is_given_in_ctor_to_an_class_that_tracks_disposables : DisposeableFixerAnalyzerSpec
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(LocalDeclarationWithObjectCreation, 0)
                    .SetName("LocalDeclaration with ObjectCreation given to a tracking instance");
                yield return new TestCaseData(FactoryCallGivenToNonTrackingInstance, 0)
                    .SetName("Factory call given to a tracking instance");
                yield return new TestCaseData(ObjectCreationInCallToCtorOfNonTrackingInstance, 0)
                    .SetName("ObjectCreation in call to ctor of a tracking instance");
                yield return new TestCaseData(FactoryCallWithinCtorCallOfNonTrackingInstance, 0)
                    .SetName("FactoryCall in call to ctor of a tracking instance");
            }
        }


        private const string LocalDeclarationWithObjectCreation = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking {
    class Tracking : IDisposable {
        public static void Do() {
            var mem = new MemoryStream();

            using (var nontracking = new StreamReader(mem)) { }
        }
        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
";

        private const string ObjectCreationInCallToCtorOfNonTrackingInstance = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking {
    class Tracking : IDisposable {
        public static void Do() {
            using(var nontracking = new StreamReader(new MemoryStream())) { }
        }
        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
";
        private const string FactoryCallGivenToNonTrackingInstance = @"
using System;
using System.IO;
namespace DisFixerTest.Tracking {
    class Tracking : IDisposable {
        public static void Do() {
            var factory = new MemStreamFactory();
            var mem = factory.Create();

            using (var tracking = new StreamReader(mem)) { }
        }

        public void Dispose() {
            throw new NotImplementedException();
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
        private const string FactoryCallWithinCtorCallOfNonTrackingInstance = @"
using System;
using System.IO;
namespace DisFixerTest.Tracking {
    class Tracking : IDisposable {
        public static void Do() {
            var factory = new MemStreamFactory();

            using (var tracking = new StreamReader(factory.Create())) { }
        }

        public void Dispose() {
            throw new NotImplementedException();
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


        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code, int numberOfDisgnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDisgnostics);
        }
    }
}
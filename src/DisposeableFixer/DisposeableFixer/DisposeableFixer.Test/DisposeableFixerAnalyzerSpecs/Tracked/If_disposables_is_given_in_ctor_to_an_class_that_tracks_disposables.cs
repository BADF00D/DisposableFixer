using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    internal class If_disposables_is_given_in_ctor_to_an_class_that_tracks_disposables : DisposeableFixerAnalyzerSpec
    {
        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics_for(string code, int numberOfDisgnostics) {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDisgnostics);
        }

        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return CreateTestCaseForLocalDeclarationWithObjectCreation();
                yield return CreateTestCaseForFactoryCallGivenToNonTrackingInstance();
                yield return CreateTestCaseForObjectCreationInCallToCtorOfNonTrackingInstance();
                yield return CreateTestCaseForFactoryCallWithinCtorCallOfNonTrackingInstance();
                yield return CreateTestCaseForObjectCreationInCallToCtorOfNonTrackingInstanceOutsideAUsingBlock();
            }
        }

        

        private static TestCaseData CreateTestCaseForObjectCreationInCallToCtorOfNonTrackingInstanceOutsideAUsingBlock()
        {
            return new TestCaseData(@"
using System;
using System.IO;

namespace DisFixerTest.Tracking {
    class Tracking : IDisposable {
        public static void Do() {
            var reader =  new StreamReader(new MemoryStream();
            using(reader) { }
        }
        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
", 0)
                .SetName("ObjectCreation in call to ctor of a tracking instance outside of using block");
        }

        private static TestCaseData CreateTestCaseForFactoryCallWithinCtorCallOfNonTrackingInstance()
        {
            return new TestCaseData(@"
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
", 0)
                .SetName("FactoryCall in call to ctor of a tracking instance");
        }

        private static TestCaseData CreateTestCaseForObjectCreationInCallToCtorOfNonTrackingInstance()
        {
            return new TestCaseData(@"
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
", 0)
                .SetName("ObjectCreation in call to ctor of a tracking instance inside of using block");
        }

        private static TestCaseData CreateTestCaseForFactoryCallGivenToNonTrackingInstance()
        {
            return new TestCaseData(@"
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
", 0)
                .SetName("Factory call given to a tracking instance");
        }

        private static TestCaseData CreateTestCaseForLocalDeclarationWithObjectCreation()
        {
            return new TestCaseData(@"
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
", 0)
                .SetName("LocalDeclaration with ObjectCreation given to a tracking instance");
        }
    }
}
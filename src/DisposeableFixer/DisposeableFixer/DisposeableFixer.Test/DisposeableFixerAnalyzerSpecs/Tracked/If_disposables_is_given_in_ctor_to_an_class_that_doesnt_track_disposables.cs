using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    internal class If_disposables_is_given_in_ctor_to_an_class_that_doesnt_track_disposables : DisposeableFixerAnalyzerSpec {
        


        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code, int numberOfDisgnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDisgnostics);
        }

        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(LocalDeclarationWithObjectCreation, 1)
                    .SetName("LocalDeclaration with ObjectCreation given to a non tracking instance");
                yield return new TestCaseData(FactoryCallGivenToNonTrackingInstance, 1)
                    .SetName("Factory call given to a non tracking instance");
                yield return new TestCaseData(ObjectCreationGivenToNonTrackingInstance, 1)
                    .SetName("LocalDeclaration with ObjectCreation given to a non tracking instance");
                yield return new TestCaseData(FactoryCallWithinCtorCallOfNonTrackingInstance, 1)
                   .SetName("FactoryCall within call to ctor of a non tracking instance");

            }
        }



        private const string LocalDeclarationWithObjectCreation = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking {
    class NoneTracking : IDisposable {
        public static void Do() {
            var mem = new MemoryStream();

            using(var tracking = new NoneTracking(mem)) { }
        }

        public NoneTracking(IDisposable disp) {}

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
";

        private const string ObjectCreationGivenToNonTrackingInstance = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking {
    class NoneTracking : IDisposable {
        public static void Do() {
            using(var tracking = new NoneTracking(new MemoryStream())) { }
        }

        public NoneTracking(IDisposable disp) {}

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
";
        private const string FactoryCallGivenToNonTrackingInstance = @"
namespace DisFixerTest.Tracking {
    class NoneTracking : IDisposable {
        public static void Do() {
            var factory = new MemStreamFactory();
            var mem = factory.Create();

            using (var tracking = new NoneTracking(mem)) { }
        }

        public NoneTracking(IDisposable disp) { }

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
namespace DisFixerTest.Tracking {
    class NoneTracking : IDisposable {
        public static void Do() {
            var factory = new MemStreamFactory();

            using (var tracking = new NoneTracking(factory.Create())) { }
        }

        public NoneTracking(IDisposable disp) { }

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
    }
}



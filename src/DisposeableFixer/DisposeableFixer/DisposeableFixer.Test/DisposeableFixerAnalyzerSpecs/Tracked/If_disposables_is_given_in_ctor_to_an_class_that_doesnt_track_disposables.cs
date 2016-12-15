using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    internal class If_disposables_is_given_in_ctor_to_an_class_that_doesnt_track_disposables : DisposeableFixerAnalyzerSpec {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf() {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
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

        [Test]
        public void Then_there_should_be_no_Diagnostics() {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
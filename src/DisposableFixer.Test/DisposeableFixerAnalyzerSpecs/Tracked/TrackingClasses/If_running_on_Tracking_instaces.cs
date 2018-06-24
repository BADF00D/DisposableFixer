using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked.TrackingClasses
{
    internal partial class If_running_on_Tracking_instaces : DisposeableFixerAnalyzerSpec
    {
        [Test]
        [TestCaseSource(nameof(TestCasesForCompositeDisposable))]
        public void Then_there_should_be_no_Diagnostic_for(string code)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            Debug.WriteLine(code);
            diagnostics.Length.Should().Be(0);
        }
    }
}

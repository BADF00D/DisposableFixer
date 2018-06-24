using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal abstract class DisposeableFixerAnalyzerSpec : Spec
    {
        protected readonly DisposableFixerAnalyzer Sut;

        protected DisposeableFixerAnalyzerSpec()
        {
            Sut = new DisposableFixerAnalyzer();
        }
    }
}
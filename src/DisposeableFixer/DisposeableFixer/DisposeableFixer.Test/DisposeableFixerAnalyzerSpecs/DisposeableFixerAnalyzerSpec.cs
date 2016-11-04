using System.IO;
using NUnit.Framework;

namespace DisposeableFixer.Test.DisposeableFixerAnalyzerSpecs
{
    [TestFixture]
    internal abstract class DisposeableFixerAnalyzerSpec : Spec
    {
        protected readonly DisposeableFixerAnalyzer Sut;

        protected DisposeableFixerAnalyzerSpec()
        {
            Sut = new DisposeableFixerAnalyzer();
        }

        
    }
}
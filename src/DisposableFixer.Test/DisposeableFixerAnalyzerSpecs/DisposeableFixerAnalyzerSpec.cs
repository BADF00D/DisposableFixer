using System;
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

        protected static void PrintCodeToFix(string code)
        {
            Console.WriteLine("Code to analyze:");
            Console.WriteLine(code);
        }
    }
}
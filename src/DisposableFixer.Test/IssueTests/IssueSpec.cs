using System;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal abstract class IssueSpec : Spec
    {
        protected readonly DisposableFixerAnalyzer Sut;

        protected IssueSpec()
        {
            Sut = new DisposableFixerAnalyzer();
        }

        protected static void PrintCodeToAnalyze(string code)
        {
            Console.WriteLine("Code to analyze:");
            Console.WriteLine(code);
        }
    }
}
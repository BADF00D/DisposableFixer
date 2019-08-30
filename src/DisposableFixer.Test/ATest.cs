using System;
using System.Diagnostics;

namespace DisposableFixer.Test
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public class ATest
    {
        protected static void PrintCodeToAnalyze(string code)
        {
            Console.WriteLine("Code to analyze:");
            Console.WriteLine(code);
        }
    }
}

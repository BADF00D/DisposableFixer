using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tuple
{
    [TestFixture]
    internal class If_a_Tuple_contains_an_Disposable : DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public MyClass()
        {
            ##BEFORE##var ##VARIABLE## = CreateInternal();
            ##STATEMENT##
        }
        private (int, IDisposable) CreateInternal() => throw new NotImplementedException();
    }
}";

        [Test, TestCaseSource(nameof(TestCases))]
        public (string, string) Test(string code)
        {
            PrintCodeToAnalyze(code);
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            PrintDiagnostics(diagnostics);

            if (diagnostics.Length == 0) return (null, null);
            if (diagnostics.Length == 1) return (diagnostics[0].Id, diagnostics[0].GetMessage());
            Assert.Fail("To many diagnostics");
            throw new Exception();
        }

        private static IEnumerable<TestCaseData> TestCases {
            get
            {
                yield return CreateTesTCase("x", string.Empty)
                    .SetName("Not disposed tuple")
                    .Returns((Id.ForLocal.TupleElement, "Tuple element 'Item2' is not disposed"));
                yield return CreateTesTCase("(x,y)", string.Empty)
                    .SetName("Not disposed deconstructed tuple")
                    .Returns((Id.ForLocal.TupleElement, "Tuple element 'y' is not disposed"));
                yield return CreateTesTCase("x", "x.Item2.Dispose();")
                    .SetName("Disposed tuple")
                    .Returns((default(string), default(string)));
                yield return CreateTesTCase("(x,y)", "x.Dispose();")
                    .SetName("Disposed deconstructed tuple")
                    .Returns((default(string), default(string)));

                //special cases
                yield return CreateTesTCase("x", string.Empty, "var Item2 = new MemoryStream();\r\n\t\t\tItem2.Dispose();\r\n\t\t\t")
                    .Returns((Id.ForLocal.TupleElement, "Tuple element 'Item2' is not disposed"))
                    .SetName("Duplicated identified");
            }
        }

        private static TestCaseData CreateTesTCase(string variable, string statement, string before = null)
        {
            var code = Code
                .Replace("##BEFORE##", before ?? string.Empty)
                .Replace("##VARIABLE##", variable)
                .Replace("##STATEMENT##", statement);

            return new TestCaseData(code);
        }
    }
}
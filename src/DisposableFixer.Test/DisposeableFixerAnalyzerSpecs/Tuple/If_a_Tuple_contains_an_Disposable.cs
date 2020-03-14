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
            var ##VARIABLE## = CreateInternal();
            ##STATEMENT##
        }
        private (int, IDisposable) CreateInternal() => throw new NotImplementedException();
    }
}";

        [Test, TestCaseSource(nameof(TestCases))]
        public string Test(string code)
        {
            PrintCodeToAnalyze(code);
            var diagnostics = MyHelper.RunAnalyser(code, Sut);

            if (diagnostics.Length == 0) return null;
            if (diagnostics.Length == 1) return diagnostics[0].Id;
            Assert.Fail("To many diagnostics");
            throw new Exception();
        }

        private static IEnumerable<TestCaseData> TestCases {
            get
            {
                yield return CreateTesTCase("x", string.Empty)
                    .SetName("Not disposed tuple")
                    .Returns(Id.ForLocal.VariableInTuple);
                yield return CreateTesTCase("(x,y)", string.Empty)
                    .SetName("Not disposed deconstructed tuple")
                    .Returns(Id.ForLocal.VariableInTuple);
                yield return CreateTesTCase("x", "x.Item2.Dispose();")
                    .SetName("Disposed tuple")
                    .Returns(null);
                yield return CreateTesTCase("(x,y)", "x.Dispose();")
                    .SetName("Disposed deconstructed tuple")
                    .Returns(null);
            }
        }

        private static TestCaseData CreateTesTCase(string variable, string statement)
        {
            var code = Code.Replace("##VARIABLE##", variable)
                .Replace("##STATEMENT##", statement);

            return new TestCaseData(code);
        }
    }
}
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tuple
{
    [TestFixture]
    internal class If_a_method_returns_IDisposable_via_Tuple : DisposeableFixerAnalyzerSpec
    {
        private const string Code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        private (int, ##RETURNTYPE##) Create()
        {
            ##DECLARATION##return (3, ##RETURNVALUE##);
        }
        private IDisposable CreateInternal() => throw new NotImplementedException();
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

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return CreateTesTCase("IDisposable", string.Empty, "new MemoryStream()")
                    .SetName("Anonymous object in tuple from object creation")
                    .Returns(null);
                yield return CreateTesTCase("IDisposable", string.Empty, "CreateInternal()")
                    .SetName("Anonymous object in tuple from method invocation")
                    .Returns(null);
                yield return CreateTesTCase("IDisposable", "var x = new MemoryStream()", "x")
                    .SetName("Local variable in tuple from object creation")
                    .Returns(null);
                yield return CreateTesTCase("IDisposable", "var x = CreateInternal()", "x")
                    .SetName("Local variable in tuple from method invocation")
                    .Returns(null);

                yield return CreateTesTCase("object", string.Empty, "new MemoryStream()")
                    .SetName("Hidden anonymous object in tuple from object creation")
                    .Returns(null);
                yield return CreateTesTCase("object", string.Empty, "CreateInternal()")
                    .SetName("Hidden anonymous object in tuple from method invocation")
                    .Returns(null);
                yield return CreateTesTCase("object", "var x = new MemoryStream()", "x")
                    .SetName("Hidden local variable in tuple from object creation")
                    .Returns(null);
                yield return CreateTesTCase("object", "var x = CreateInternal()", "x")
                    .SetName("Hidden local variable in tuple from method invocation")
                    .Returns(null);

            }
        }

        private static TestCaseData CreateTesTCase(string returnType, string declaration, string returnValue)
        {
            var code = Code.Replace("##RETURNTYPE##", returnType)
                .Replace("##DECLARATION##", declaration == string.Empty ? string.Empty : declaration+"\r\n")
                .Replace("##RETURNVALUE##", returnValue);

            return new TestCaseData(code);
        }
    }
}
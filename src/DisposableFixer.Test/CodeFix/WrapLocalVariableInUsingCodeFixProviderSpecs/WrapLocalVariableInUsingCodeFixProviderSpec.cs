using System.Collections.Generic;
using System.Linq;
using DisposableFixer.CodeFix;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.CodeFix.WrapLocalVariableInUsingCodeFixProviderSpecs
{
    [TestFixture]
    internal class WrapLocalVariableInUsingCodeFixProviderSpec : DisposableAnalyserCodeFixVerifierSpec
    {
        private const string CodeWithUndisposedLocalVariableAndTrailingCode = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            var variable = Create();
            var x = 3;
        }

        private static IDisposable Create()
        {
            return new MemoryStream();
        }
    }
}
";
        private const string CodeWithUndisposedLocalVariableAndPreceedingCode = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            var y = 3;
            var variable = Create(y);
            var x = 3;
        }

        private static IDisposable Create(int dummy)
        {
            return new MemoryStream();
        }
    }
}
";
        private const string CodeWithUndisposedLocalVariableAndAMethodInvocation = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            var stream= new MemoryStream();
            stream.WriteByte(0);
        }
    }
}
";
        private const string CodeWithUndisposedLocalVariableThatIsAlsoAParameter = @"
using System.IO;
namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            var stream = new MemoryStream();
            Write(stream, 0);
        }

        private void Write(Stream stream, byte b)
        {
            stream.WriteByte(b);
        }
    }
}
";

        private static IEnumerable<TestCaseData> TestCases {
            get {
                var forNotDisposedLocalVariable = Id.ForNotDisposedLocalVariable;
                yield return new TestCaseData(CodeWithUndisposedLocalVariableAndTrailingCode,
                        forNotDisposedLocalVariable)
                    .SetName("Local variable and more code");
                yield return new TestCaseData(CodeWithUndisposedLocalVariableAndAMethodInvocation,
                        forNotDisposedLocalVariable)
                    .SetName("Local variable and trailing code that makes a MethodInvocation to variable");
                yield return new TestCaseData(CodeWithUndisposedLocalVariableThatIsAlsoAParameter,
                        forNotDisposedLocalVariable)
                    .SetName("Local variable that is a parameter");
                yield return new TestCaseData(CodeWithUndisposedLocalVariableAndPreceedingCode,
                        forNotDisposedLocalVariable)
                    .SetName("Local variable and preceeding code");
            }
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new WrapLocalVariableInUsingBlockCodeFixProvider();
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void Should_the_applied_CodeFix_solve_the_diagnostic(string code, string preFixDiagnisticId)
        {
            //arrange
            PrintCodeToFix(code);
            MyHelper.RunAnalyser(code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == preFixDiagnisticId, "this should be fixed");

            //act
            var fixedCode = ApplyCSharpCodeFix(code);
            PrintFixedCode(fixedCode);

            //assert
            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we dont want to introduce bugs");

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().NotContain(d => d.Id == preFixDiagnisticId, "this should have been fixed");
        }
    }
}
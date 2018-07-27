using System.Collections.Generic;
using DisposableFixer.CodeFix;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.CodeFix.WrapAnounymousObjectsInUsingCodeFixProviderSpecs
{
    public class WrapAnounymousObjectsInUsingCodeFixProviderSpec : DisposableAnalyserCodeFixVerifierSpec
    {
        private const string CodeWithAnonymousObjectCreation = @"
using System.IO;

namespace SomeNamespace
{
    public class SomeClass{
        public SomeClass()
        {
            new MemoryStream();
        }
    }
}
";
        private const string CodeWithAnonymousObjectCreationAndOtherCode = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            new MemoryStream();
            Console.WriteLine();
        }
    }
}
";

        private const string CodeWithMethodInvocationAndOtherCode = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            Create();
            Console.WriteLine();
        }

        private static IDisposable Create()
        {
            return new MemoryStream();
        }
    }
}
";

        private const string CodeWithObjectCreationThatIsAParameter = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            using (var writer = new SomeNonTrackingWriter(new MemoryStream()))
            {
                writer.Write(0);
            }
        }

        private static void Write(TextWriter stream, byte b)
        {
            stream.WriteLine(b);
        }

        private class SomeNonTrackingWriter : IDisposable
        {
            public SomeNonTrackingWriter(Stream stream){}
            public void Write(byte b) { }

            public void Dispose(){}
        }
    }
}
";
        private const string CodeWithMethodInvaocationThatIsAParameter = @"
using System;
using System.IO;

namespace SomeNamespace
{
    public class SomeClass
    {
        public SomeClass()
        {
            using (var writer = new SomeNonTrackingWriter(Create()))
            {
                writer.Write(0);
            }
        }

        private static Stream Create()
        {
            return new MemoryStream();
        }

        private static void Write(TextWriter stream, byte b)
        {
            stream.WriteLine(b);
        }

        private class SomeNonTrackingWriter : IDisposable
        {
            public SomeNonTrackingWriter(Stream stream){}
            public void Write(byte b) { }

            public void Dispose(){}
        }
    }
}
";

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(CodeWithAnonymousObjectCreation,
                        SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                    .SetName("Anonymous ObjectCreation");
                yield return new TestCaseData(CodeWithAnonymousObjectCreationAndOtherCode,
                        SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                    .SetName("Anonymous ObjectCreation and other code");
                yield return new TestCaseData(CodeWithObjectCreationThatIsAParameter,
                        SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                    .SetName("Anonymous ObjectCreation that is a parameter");

                yield return new TestCaseData(CodeWithMethodInvocationAndOtherCode,
                        SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation)
                    .SetName("Anonymous MethodInvocation and other code");
                yield return new TestCaseData(CodeWithObjectCreationThatIsAParameter,
                        SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromObjectCreation)
                    .SetName("Anonymous ObjectCreation that is a parameter");
                yield return new TestCaseData(CodeWithMethodInvaocationThatIsAParameter,
                        SyntaxNodeAnalysisContextExtension.IdForAnonymousObjectFromMethodInvocation)
                    .SetName("Anonymous MethodInvocation that is a parameter");
            }
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new WrapAnounymousObjectsInUsingCodeFixProvider();
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void Should_the_applied_CodeFix_solve_the_diagnostic(string code, string preFixDiagnisticId)
        {
            PrintCodeToFix(code);
            MyHelper.RunAnalyser(code, GetCSharpDiagnosticAnalyzer())
                .Should().Contain(d => d.Id == preFixDiagnisticId, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFix(code);
            PrintFixedCode(fixedCode);

            MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer())
                .Should().BeEmpty();
        }
    }
}
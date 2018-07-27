using System;
using System.Collections.Generic;
using DisposableFixer.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.CodeFix.UndisposedFieldCodeFixProviderSpecs
{
    [TestFixture]
    internal class UndisposedFieldCodeFixProviderSpec : DisposableAnalyserCodeFixVerifierSpec
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UndisposedFieldCodeFixProvider();
        }
        [Test, TestCaseSource(nameof(TestCases))]
        public void Should_there_be_no_Diagnostic(string code)
        {
            PrintCodeToFix(code);
            MyHelper.RunAnalyser(code, GetCSharpDiagnosticAnalyzer())
                .Should().HaveCount(1, "there should be something to fix");

            var fixedCode = ApplyCSharpCodeFix(code, 0);
            PrintFixedCode(fixedCode);

            //assert
            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we dont want to introduce bugs");

            var diagostics = MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer());
            diagostics.Should().HaveCount(0);
        }

        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return CreateTest(false, false, Location.PropertyOrField, false);
                yield return CreateTest(true, false, Location.PropertyOrField, false);
                yield return CreateTest(false, true, Location.PropertyOrField, false);
                yield return CreateTest(true, true, Location.PropertyOrField, false);
                yield return CreateTest(false, false, Location.Ctor, false);
                yield return CreateTest(true, false, Location.Ctor, false);
                yield return CreateTest(false, true, Location.Ctor, false);
                yield return CreateTest(true, true, Location.Ctor, false);
                yield return CreateTest(false, false, Location.Method, false);
                yield return CreateTest(true, false, Location.Method, false);
                yield return CreateTest(false, true, Location.Method, false);
                yield return CreateTest(true, true, Location.Method, false);
                yield return CreateTest(false, false, Location.PropertyOrField, true);
                yield return CreateTest(true, false, Location.PropertyOrField, true);
                yield return CreateTest(false, true, Location.PropertyOrField, true);
                yield return CreateTest(true, true, Location.PropertyOrField, true);
                yield return CreateTest(false, false, Location.Ctor, true);
                yield return CreateTest(true, false, Location.Ctor, true);
                yield return CreateTest(false, true, Location.Ctor, true);
                yield return CreateTest(true, true, Location.Ctor, true);
                yield return CreateTest(false, false, Location.Method, true);
                yield return CreateTest(true, false, Location.Method, true);
                yield return CreateTest(false, true, Location.Method, true);
                yield return CreateTest(true, true, Location.Method, true);
            }
        }

        private static TestCaseData CreateTest(bool useSystem, bool implementIDisposable, Location location, bool hasDisposeMethod)
        {
            var code = CodeWithUndisposedField
                .Replace("##usingSystem##", useSystem ? "using System;" : string.Empty)
                .Replace("##interfaceDeclaration##", implementIDisposable ? ": IDisposable" : string.Empty)
                .Replace("##FieldInitializer##", location == Location.PropertyOrField ? " = new MemoryStream()" : string.Empty)
                .Replace("##CtorInitializer##", location == Location.Ctor ? "CodeWithUndisposedField = new MemoryStream();" : string.Empty)
                .Replace("##MethodInitializer##", location == Location.Method ? "CodeWithUndisposedField = new MemoryStream();" : string.Empty)
                .Replace("##DisposeMethod##", hasDisposeMethod ? "public void Dispose(){}" : String.Empty);

            return new TestCaseData(code)
                .SetName($"useSystem: {useSystem} implementIDisposable: {implementIDisposable} location: {location} hasDisposeMethod: {hasDisposeMethod}");
        }

        private enum Location
        {
            PropertyOrField,
            Ctor,
            Method
        }

        private const string CodeWithUndisposedField =
@"##usingSystem##
using System.IO;
namespace MyNamespace 
{
    public class SomeClass ##interfaceDeclaration##
    {
        public MemoryStream CodeWithUndisposedField##FieldInitializer##;

        public SomeClass()
        {
            ##CtorInitializer##
        }
        public void SomeMethod()
        {
            ##MethodInitializer##
        }
        ##DisposeMethod##
    }
}";
    }
}
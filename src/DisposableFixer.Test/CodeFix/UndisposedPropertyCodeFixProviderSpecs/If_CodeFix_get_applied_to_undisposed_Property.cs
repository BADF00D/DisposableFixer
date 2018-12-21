using System;
using System.Collections.Generic;
using DisposableFixer.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.CodeFix.UndisposedPropertyCodeFixProviderSpecs
{
    [TestFixture]
    internal class UndisposedPropertyCodeFixProviderSpec : DisposableAnalyserCodeFixVerifierSpec
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UndisposedPropertyCodeFixProvider();
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void Should_there_be_no_Diagnostic(string code)
        {
            PrintCodeToFix(code);
            MyHelper.RunAnalyser(code, GetCSharpDiagnosticAnalyzer())
                .Should().HaveCount(1, "there should be something to fix");
            var fixedCode = ApplyCSharpCodeFix(code, 0);
            PrintFixedCode(fixedCode);

            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we dont want to introduce bugs");

            var diagostics = MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer());
            diagostics.Should().HaveCount(0);
        }

        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return CreateTest(false, false, Location.PropertyOrField, false, false, false);
                yield return CreateTest(true, false, Location.PropertyOrField, false, false, false);
                yield return CreateTest(false, true, Location.PropertyOrField, false, false, false);
                yield return CreateTest(true, true, Location.PropertyOrField, false, false, false);
                yield return CreateTest(false, false, Location.Ctor, false, false, false);
                yield return CreateTest(true, false, Location.Ctor, false, false, false);
                yield return CreateTest(false, true, Location.Ctor, false, false, false);
                yield return CreateTest(true, true, Location.Ctor, false, false, false);
                yield return CreateTest(false, false, Location.Method, false, false, false);
                yield return CreateTest(true, false, Location.Method, false, false, false);
                yield return CreateTest(false, true, Location.Method, false, false, false);
                yield return CreateTest(true, true, Location.Method, false, false, false);
                yield return CreateTest(false, false, Location.PropertyOrField, true, false, false);
                yield return CreateTest(true, false, Location.PropertyOrField, true, false, false);
                yield return CreateTest(false, true, Location.PropertyOrField, true, false, false);
                yield return CreateTest(true, true, Location.PropertyOrField, true, false, false);
                yield return CreateTest(false, false, Location.Ctor, true, false, false);
                yield return CreateTest(true, false, Location.Ctor, true, false, false);
                yield return CreateTest(false, true, Location.Ctor, true, false, false);
                yield return CreateTest(true, true, Location.Ctor, true, false, false);
                yield return CreateTest(false, false, Location.Method, true, false, false);
                yield return CreateTest(true, false, Location.Method, true, false, false);
                yield return CreateTest(false, true, Location.Method, true, false, false);
                yield return CreateTest(true, true, Location.Method, true, false, false);

                yield return CreateTest(false, false, Location.PropertyOrField, false, true, false);
                yield return CreateTest(true, false, Location.PropertyOrField, false, true, false);
                yield return CreateTest(false, true, Location.PropertyOrField, false, true, false);
                yield return CreateTest(true, true, Location.PropertyOrField, false, true, false);
                yield return CreateTest(false, false, Location.Ctor, false, true, false);
                yield return CreateTest(true, false, Location.Ctor, false, true, false);
                yield return CreateTest(false, true, Location.Ctor, false, true, false);
                yield return CreateTest(true, true, Location.Ctor, false, true, false);
                yield return CreateTest(false, false, Location.Method, false, true, false);
                yield return CreateTest(true, false, Location.Method, false, true, false);
                yield return CreateTest(false, true, Location.Method, false, true, false);
                yield return CreateTest(true, true, Location.Method, false, true, false);
                yield return CreateTest(false, false, Location.PropertyOrField, true, true, false);
                yield return CreateTest(true, false, Location.PropertyOrField, true, true, false);
                yield return CreateTest(false, true, Location.PropertyOrField, true, true, false);
                yield return CreateTest(true, true, Location.PropertyOrField, true, true, false);
                yield return CreateTest(false, false, Location.Ctor, true, true, false);
                yield return CreateTest(true, false, Location.Ctor, true, true, false);
                yield return CreateTest(false, true, Location.Ctor, true, true, false);
                yield return CreateTest(true, true, Location.Ctor, true, true, false);
                yield return CreateTest(false, false, Location.Method, true, true, false);
                yield return CreateTest(true, false, Location.Method, true, true, false);
                yield return CreateTest(false, true, Location.Method, true, true, false);
                yield return CreateTest(true, true, Location.Method, true, true, false);

                yield return CreateTest(false, false, Location.PropertyOrField, false, false, true);
                yield return CreateTest(true, false, Location.PropertyOrField, false, false, true);
                yield return CreateTest(false, true, Location.PropertyOrField, false, false, true);
                yield return CreateTest(true, true, Location.PropertyOrField, false, false, true);
                yield return CreateTest(false, false, Location.Ctor, false, false, true);
                yield return CreateTest(true, false, Location.Ctor, false, false, true);
                yield return CreateTest(false, true, Location.Ctor, false, false, true);
                yield return CreateTest(true, true, Location.Ctor, false, false, true);
                yield return CreateTest(false, false, Location.Method, false, false, true);
                yield return CreateTest(true, false, Location.Method, false, false, true);
                yield return CreateTest(false, true, Location.Method, false, false, true);
                yield return CreateTest(true, true, Location.Method, false, false, true);
                yield return CreateTest(false, false, Location.PropertyOrField, true, false, true);
                yield return CreateTest(true, false, Location.PropertyOrField, true, false, true);
                yield return CreateTest(false, true, Location.PropertyOrField, true, false, true);
                yield return CreateTest(true, true, Location.PropertyOrField, true, false, true);
                yield return CreateTest(false, false, Location.Ctor, true, false, true);
                yield return CreateTest(true, false, Location.Ctor, true, false, true);
                yield return CreateTest(false, true, Location.Ctor, true, false, true);
                yield return CreateTest(true, true, Location.Ctor, true, false, true);
                yield return CreateTest(false, false, Location.Method, true, false, true);
                yield return CreateTest(true, false, Location.Method, true, false, true);
                yield return CreateTest(false, true, Location.Method, true, false, true);
                yield return CreateTest(true, true, Location.Method, true, false, true);

                yield return CreateTest(false, false, Location.PropertyOrField, false, true, true);
                yield return CreateTest(true, false, Location.PropertyOrField, false, true, true);
                yield return CreateTest(false, true, Location.PropertyOrField, false, true, true);
                yield return CreateTest(true, true, Location.PropertyOrField, false, true, true);
                yield return CreateTest(false, false, Location.Ctor, false, true, true);
                yield return CreateTest(true, false, Location.Ctor, false, true, true);
                yield return CreateTest(false, true, Location.Ctor, false, true, true);
                yield return CreateTest(true, true, Location.Ctor, false, true, true);
                yield return CreateTest(false, false, Location.Method, false, true, true);
                yield return CreateTest(true, false, Location.Method, false, true, true);
                yield return CreateTest(false, true, Location.Method, false, true, true);
                yield return CreateTest(true, true, Location.Method, false, true, true);
                yield return CreateTest(false, false, Location.PropertyOrField, true, true, true);
                yield return CreateTest(true, false, Location.PropertyOrField, true, true, true);
                yield return CreateTest(false, true, Location.PropertyOrField, true, true, true);
                yield return CreateTest(true, true, Location.PropertyOrField, true, true, true);
                yield return CreateTest(false, false, Location.Ctor, true, true, true);
                yield return CreateTest(true, false, Location.Ctor, true, true, true);
                yield return CreateTest(false, true, Location.Ctor, true, true, true);
                yield return CreateTest(true, true, Location.Ctor, true, true, true);
                yield return CreateTest(false, false, Location.Method, true, true, true);
                yield return CreateTest(true, false, Location.Method, true, true, true);
                yield return CreateTest(false, true, Location.Method, true, true, true);
                yield return CreateTest(true, true, Location.Method, true, true, true);

                yield return new TestCaseData(UndisposedPropertyInAClassWithInnerClassThatContainsADisposeMethod)
                    .SetName("Undisposed property in class with innner class that contains a dispose method");
            }
        }

        private static TestCaseData CreateTest(bool useSystem, bool implementIDisposable, Location location, bool hasDisposeMethod, bool hasBaseClass, bool targetTypeIsObject)
        {
            var code = CodeWithUndisposedFieldTemplate
                .Replace("##targetType##", targetTypeIsObject ? "object" : "MemoryStream")
                .Replace("##usingSystem##", useSystem ? "using System;" : string.Empty)
                .Replace("##PropertyInitializer##", location == Location.PropertyOrField ? " = new MemoryStream();" : string.Empty)
                .Replace("##CtorInitializer##", location == Location.Ctor ? "Property = new MemoryStream();" : string.Empty)
                .Replace("##MethodInitializer##", location == Location.Method ? "Property = new MemoryStream();" : string.Empty)
                .Replace("##DisposeMethod##", hasDisposeMethod ? "public void Dispose(){}" : string.Empty);

            if (hasBaseClass || implementIDisposable)
            {
                if (hasBaseClass && implementIDisposable)
                {
                    code = code.Replace("##baseClass##", ": object, IDisposable");
                }
                else if (hasBaseClass)
                {
                    code = code.Replace("##baseClass##", ": object");
                }
                else
                {
                    code = code.Replace("##baseClass##", ": IDisposable");
                }
            }
            else
            {
                code = code.Replace("##baseClass##", string.Empty);
            }

            var prefix = string.Empty;
            prefix += targetTypeIsObject ? "TargetIsNotDisposable." : "TargetIsDisposable.";
            prefix += hasBaseClass ? "HasBaseClass." : "HasNoBaseClass.";
            switch (location)
            {
                case Location.PropertyOrField:
                    prefix += "CreatedInField.";
                    break;
                case Location.Ctor:
                    prefix += "CreatedInCtor.";
                    break;
                case Location.Method:
                    prefix += "CreatedInMethod.";
                    break;
            }

            return new TestCaseData(code)
                .SetName($"{prefix}useSystem: {useSystem} implementIDisposable: {implementIDisposable} hasDisposeMethod: {hasDisposeMethod}");
        }

        private enum Location
        {
            PropertyOrField,
            Ctor,
            Method
        }

        private const string CodeWithUndisposedFieldTemplate =
@"##usingSystem##
using System.IO;
namespace MyNamespace 
{
    public class SomeClass ##baseClass##
    {
        public ##targetType## Property {get; private set;} ##PropertyInitializer##

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

        private const string UndisposedPropertyInAClassWithInnerClassThatContainsADisposeMethod = @"
using System;
using System.IO;

namespace ExtensionMethodYieldsNotDisposed
{
    internal class Class1
    {
        public IDisposable MemoryStream {get;}

        public Class1()
        {
            MemoryStream = new MemoryStream();
        }

        private class InnerClassWithDisposeMethod
        {
            public void Dispose(){}
        }
    }
}
";
    }
}
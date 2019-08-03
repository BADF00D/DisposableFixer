using System.Collections.Generic;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.IdTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_Code_with_undisposed_variables
    {
        private readonly DisposableFixerAnalyzer _disposableFixerAnalyzer = new DisposableFixerAnalyzer();

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public string Then_there_should_be_one_Diagnostic_with_correct_id(string code)
        {
            return MyHelper.RunAnalyser(code, _disposableFixerAnalyzer)[0].Id;
        }

        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return StaticPropertyInitializedDirectlyViaObjectCreation();
                yield return NoneStaticPropertyInitializedDirectlyViaObjectCreation();
                yield return StaticFieldInitializedDirectlyViaObjectCreation();
                yield return NoneStaticFieldInitializedDirectlyViaObjectCreation();

            }
        }
        
        private static TestCaseData StaticPropertyInitializedDirectlyViaObjectCreation()
        {
            const string code = @"
using System;
using System.IO;

internal class SomeTestNamspace
{
    private class SomeTest
    {
        private static IDisposable Property { get; } = new MemoryStream();
    }
}";
            return new TestCaseData(code)
                .Returns(Id.ForAssignmentFromObjectCreationToStaticPropertyNotDisposed)
                .SetName("Static property initialized directly via ObjectCreation");
        }

        private static TestCaseData NoneStaticPropertyInitializedDirectlyViaObjectCreation()
        {
            const string code = @"
using System;
using System.IO;

internal class SomeTestNamspace
{
    private class SomeTest
    {
        private IDisposable Property { get; } = new MemoryStream();
    }
}";
            return new TestCaseData(code)
                .Returns(Id.ForAssignmentFromObjectCreationToPropertyNotDisposed)
                .SetName("None Static property initialized directly via ObjectCreation");
        }

        private static TestCaseData NoneStaticFieldInitializedDirectlyViaObjectCreation()
        {
            const string code = @"
using System;
using System.IO;

internal class SomeTestNamspace
{
    private class SomeTest
    {
        private IDisposable Field = new MemoryStream();
    }
}";
            return new TestCaseData(code)
                .Returns(Id.ForAssignmentFromObjectCreationToFieldNotDisposed)
                .SetName("None Static field initialized directly via ObjectCreation");
        }

        private static TestCaseData StaticFieldInitializedDirectlyViaObjectCreation()
        {
            const string code = @"
using System;
using System.IO;

internal class SomeTestNamspace
{
    private class SomeTest
    {
        private static IDisposable Field = new MemoryStream();
    }
}";
            return new TestCaseData(code)
                .Returns(Id.ForAssignmentFromObjectCreationToStaticFieldNotDisposed)
                .SetName("Static field initialized directly via ObjectCreation");
        }
    }
}
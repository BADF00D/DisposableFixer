using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue136_UndisposedMembersWithName : IssueSpec
    {
        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return CreateForStaticFieldFromObjectCreation();
                yield return CreateForNoneStaticFieldFromObjectCreation();
                yield return CreateForNoneStaticPropertyFromObjectCreation();
                yield return CreateForStaticPropertyFromObjectCreation();

                yield return CreateForStaticFieldFromMethodInvocation();
                yield return CreateForNoneStaticFieldFromMethodInvocation();
                yield return CreateForNoneStaticPropertyFromMethodInvocation();
                yield return CreateForStaticPropertyFromMethodInvocation();
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public void Then_there_should_one_diagnostic_with_correct_message(string code, string expectedMessage)
        {
            PrintCodeToAnalyze(code);
            var diagnostics = MyHelper.RunAnalyser(code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].GetMessage().Should().Be(expectedMessage);
        }

        private static TestCaseData CreateForStaticPropertyFromObjectCreation()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public static IDisposable MemoryStream { get; }

        public MyClass()
        {
            MemoryStream = new MemoryStream();
        }
    }

}
";
            return new TestCaseData(code, "Static property 'MemoryStream' is not disposed.").SetName(
                "static property from ObjectCreation");
        }

        private static TestCaseData CreateForNoneStaticPropertyFromObjectCreation()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public IDisposable MemoryStream { get; }

        public MyClass()
        {
            MemoryStream = new MemoryStream();
        }
    }

}
";
            return new TestCaseData(code, "Property 'MemoryStream' is not disposed.").SetName(
                "none static property from ObjectCretion");
        }

        private static TestCaseData CreateForNoneStaticFieldFromObjectCreation()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public IDisposable MemoryStream;

        public MyClass()
        {
            MemoryStream = new MemoryStream();
        }
    }

}
";
            return new TestCaseData(code, "Field 'MemoryStream' is not disposed.")
                .SetName("none static field from ObjectCreation");
        }

        private static TestCaseData CreateForStaticFieldFromObjectCreation()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public static IDisposable MemoryStream;

        public MyClass()
        {
            MemoryStream = new MemoryStream();
        }
    }

}
";
            return new TestCaseData(code, "Static field 'MemoryStream' is not disposed.").SetName(
                "static field from ObjectCreation");
        }



        private static TestCaseData CreateForStaticPropertyFromMethodInvocation()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public static IDisposable MemoryStream { get; }

        public MyClass()
        {
            Func<MemoryStream> Create = () => throw new NotImplementedException();
            MemoryStream = Create();
        }
    }

}
";
            return new TestCaseData(code, "Static property 'MemoryStream' is not disposed.").SetName(
                "static property from MethodInvocation");
        }

        private static TestCaseData CreateForNoneStaticPropertyFromMethodInvocation()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public IDisposable MemoryStream { get; }

        public MyClass()
        {
            Func<MemoryStream> Create = () => throw new NotImplementedException();
            MemoryStream = Create();
        }
    }

}
";
            return new TestCaseData(code, "Property 'MemoryStream' is not disposed.").SetName(
                "none static property from MethodInvocation");
        }

        private static TestCaseData CreateForNoneStaticFieldFromMethodInvocation()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public IDisposable MemoryStream;

        public MyClass()
        {
            Func<MemoryStream> Create = () => throw new NotImplementedException();
            MemoryStream = Create();
        }
    }

}
";
            return new TestCaseData(code, "Field 'MemoryStream' is not disposed.").SetName("none static field from MethodInvocation");
        }

        private static TestCaseData CreateForStaticFieldFromMethodInvocation()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public static IDisposable MemoryStream;

        public MyClass()
        {
            Func<MemoryStream> Create = () => throw new NotImplementedException();
            MemoryStream = Create();
        }
    }

}
";
            return new TestCaseData(code, "Static field 'MemoryStream' is not disposed.").SetName(
                "static field from MethodInvocation");
        }
    }
}
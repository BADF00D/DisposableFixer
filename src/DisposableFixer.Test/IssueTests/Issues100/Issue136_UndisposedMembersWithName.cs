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
                yield return StaticFieldFromObjectCreation();
                yield return NoneStaticFieldFromObjectCreation();
                yield return NoneStaticPropertyFromObjectCreation();
                yield return StaticPropertyFromObjectCreation();

                yield return StaticFieldFromMethodInvocation();
                yield return NoneStaticFieldFromMethodInvocation();
                yield return NoneStaticPropertyFromMethodInvocation();
                yield return StaticPropertyFromMethodInvocation();

                yield return StaticPropertyFactory();
                yield return NoneStaticPropertyFactory();
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

        private static TestCaseData StaticPropertyFromObjectCreation()
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

        private static TestCaseData NoneStaticPropertyFromObjectCreation()
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

        private static TestCaseData NoneStaticFieldFromObjectCreation()
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

        private static TestCaseData StaticFieldFromObjectCreation()
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



        private static TestCaseData StaticPropertyFromMethodInvocation()
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

        private static TestCaseData NoneStaticPropertyFromMethodInvocation()
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

        private static TestCaseData NoneStaticFieldFromMethodInvocation()
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

        private static TestCaseData StaticFieldFromMethodInvocation()
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


        private static TestCaseData StaticPropertyFactory()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public static IDisposable MemoryStream => new MemoryStream();
    }

}
";
            return new TestCaseData(code, "Static factory property 'MemoryStream' cannot be disposed. It recommended to change this to a static factory method.").SetName(
                "Static property factory");
        }

        private static TestCaseData NoneStaticPropertyFactory()
        {
            const string code = @"
using System;
using System.IO;

namespace MyNamespace
{
    class MyClass
    {
        public IDisposable MemoryStream => new MemoryStream();
    }

}
";
            return new TestCaseData(code, "Factory properties 'MemoryStream' cannot be disposed. It recommended to change this to a factory method.").SetName(
                "None static property factory");
        }
    }
}
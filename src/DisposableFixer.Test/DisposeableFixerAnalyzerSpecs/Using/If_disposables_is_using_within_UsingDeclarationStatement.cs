using System.Collections.Generic;
using DisposableFixer.Test.Attributes;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Using
{
    [UsingDeclarationStatement]
    internal class If_disposables_is_using_within_UsingDeclarationStatement : DisposeableFixerAnalyzerSpec
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(LocalVariableFromObjectCreation, 0)
                    .SetName("Local variable from OC");
                yield return new TestCaseData(LocalVariableFromMethodInvocation, 0)
                    .SetName("Local variable from MI");
                yield return new TestCaseData(LocalVariableCreatedByFlagFromMethodInvocationAndObjectCreation, 0)
                    .SetName("Local variable created by flag from MI and OC");
                yield return new TestCaseData(LocalVariableCreatedWithFlagFromMethodInvocation, 0)
                    .SetName("Local variable created by flag from MI");
                yield return new TestCaseData(LocalVariableCreatedByInstanceFactoryMethod, 0)
                    .SetName("Local variable created by instance factory method");
                yield return new TestCaseData(LocalVariableCreatedWithStaticFactoryMethod, 0)
                    .SetName("Local variable created by static factory method");
            }
        }

        private const string LocalVariableFromObjectCreation = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking
{
    class Test
    {
        public static void Do()
        {
            using var mem = new MemoryStream();
        }
    }
}
";

        private const string LocalVariableFromMethodInvocation = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking
{
    class Test
    {
        public static void Do()
        {
            MemoryStream Create() => throw new NotImplementedException();
            using var mem = Create();
        }
    }
}
";

     

        private const string LocalVariableCreatedWithFlagFromMethodInvocation = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking
{
    class Test
    {
        public static void Do(bool flag)
        {
            MemoryStream Create(int x) => throw new NotImplementedException();
            using var mem = flag ? Create(1) : Create(2);
        }
    }
}
";

        private const string LocalVariableCreatedByFlagFromMethodInvocationAndObjectCreation = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking
{
    class Test
    {
        public static void Do(bool flag)
        {
            MemoryStream Create(int x) => throw new NotImplementedException();
            using var mem = flag ? new MemoryStream(null): Create(1);
        }
    }
}
";
        private const string LocalVariableCreatedWithStaticFactoryMethod = @"
using System;
using System.Dynamic;
using System.IO;

namespace DisFixerTest.Tracking
{
    class Test
    {
        public static void Do(bool flag)
        {
            using var x = Factory.Create();
        }

        public class Factory
        {
            public static IDisposable Create()=> throw new NotImplementedException();
        }
    }
}
";

        private const string LocalVariableCreatedByInstanceFactoryMethod = @"
using System;
using System.Dynamic;
using System.IO;

namespace DisFixerTest.Tracking
{
    class Test
    {
        public static void Do(bool flag)
        {
            using var x = new Factory().Create();
        }

        public class Factory
        {
            public IDisposable Create()=> throw new NotImplementedException();
        }
    }
}";



        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }
    }
}
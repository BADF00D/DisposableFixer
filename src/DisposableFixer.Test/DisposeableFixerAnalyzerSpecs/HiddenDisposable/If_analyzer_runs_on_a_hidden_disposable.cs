using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.HiddenDisposable
{
    [TestFixture]
    internal class If_analyzer_runs_on_a_hidden_disposable : ATest
    {
        private readonly DisposableFixerAnalyzer _disposableFixerAnalyzer = new DisposableFixerAnalyzer();

        [TestCaseSource(nameof(TestCases))]
        public string Then_there_should_be_one_Diagnostic_with_correct_id(string code)
        {
            PrintCodeToAnalyze(code);
            return MyHelper.RunAnalyser(code, _disposableFixerAnalyzer)
                .Select(dd => dd.Id)
                .FirstOrDefault();
        }

        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return HiddenObjectCreation();
                yield return HiddenInvocationExpressionUsingFunc();
                yield return HiddenInvocationExpressionUsingStaticMethod();
                yield return HiddenInvocationExpressionUsingStaticFactoryClass();
                yield return HiddenInvocationExpressionUsingFactoryClass();
                yield return HiddenInvocationExpressionUsingLocalFunction();
                yield return HiddenInvocationExpressionOfLocalFunction();

                yield return HiddenObjectCreationReturnedLater();
                yield return HiddenInvocationExpressionUsingFuncReturnedLater();
                yield return HiddenInvocationExpressionUsingStaticMethodReturnedLater();
                yield return HiddenInvocationExpressionUsingStaticFactoryClassReturnedLater();
                yield return HiddenInvocationExpressionUsingFactoryClassReturnedLater();
                yield return HiddenInvocationExpressionUsingLocalFunctionReturnedLater();
            }
        }
        
        private static TestCaseData HiddenObjectCreation()
        {
            const string code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            return new MemoryStream();
        }
    }
}
";
            return new TestCaseData(code)
                .Returns(Id.ForHiddenIDisposable)
                .SetName("Hidden ObjectCreation");
        }

        private static TestCaseData HiddenInvocationExpressionUsingFunc()
        {
            const string code = @"
using System;
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            Func<MemoryStream> create = () => new MemoryStream();
            return create();
        }
    }
}";
            return new TestCaseData(code)
                .Returns(Id.ForHiddenIDisposable)
                .SetName("Hidden InvokcationExpression using Func");
        }

        private static TestCaseData HiddenInvocationExpressionUsingStaticMethod()
        {
            const string code = @"
using System;
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            return Create();
        }

        private static MemoryStream Create() => new MemoryStream();
    }
}";
            return new TestCaseData(code)
                .Returns(Id.ForHiddenIDisposable)
                .SetName("Hidden InvokcationExpression using static Method");
        }


        private static TestCaseData HiddenInvocationExpressionUsingStaticFactoryClass()
        {
            const string code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            return Factory.Create();
        }

        private static class Factory
        {
            public static MemoryStream Create() => new MemoryStream();
        }
    }
}
";
            return new TestCaseData(code)
                .Returns(Id.ForHiddenIDisposable)
                .SetName("Hidden InvokcationExpression using static factory class");
        }

        private static TestCaseData HiddenInvocationExpressionUsingFactoryClass()
        {
            const string code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            return new Factory().Create();
        }

        private class Factory
        {
            public MemoryStream Create() => new MemoryStream();
        }
    }
}
";
            return new TestCaseData(code)
                .Returns(Id.ForHiddenIDisposable)
                .SetName("Hidden InvokcationExpression using factory class");
        }

        private static TestCaseData HiddenInvocationExpressionUsingLocalFunction()
        {
            const string code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            MemoryStream Create()
            {
                return new MemoryStream();
            }

            return Create();
        }
    }
}
";
            return new TestCaseData(code)
                .Returns(Id.ForHiddenIDisposable)
                .SetName("Hidden InvokcationExpression using local function");
        }

        private static TestCaseData HiddenInvocationExpressionOfLocalFunction()
        {
            const string code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            object Create()
            {
                return new MemoryStream();
            }

            return Create();
        }
    }
}
";
            return new TestCaseData(code)
                .Returns(Id.ForHiddenIDisposable)
                .SetName("Hidden InvokcationExpression of local function");
        }



        private static TestCaseData HiddenObjectCreationReturnedLater()
        {
            const string code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            var mem = new MemoryStream();
            return mem;
        }
    }
}
";
            return new TestCaseData(code)
                .Returns(Id.ForHiddenIDisposable)
                .SetName("Hidden ObjectCreation returned later");
    }

    private static TestCaseData HiddenInvocationExpressionUsingFuncReturnedLater()
    {
        const string code = @"
using System;
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            Func<MemoryStream> create = () => new MemoryStream();
            var mem = create();
            return mem;
        }
    }
}";
        return new TestCaseData(code)
            .Returns(Id.ForHiddenIDisposable)
            .SetName("Hidden InvokcationExpression using Func returned later");
    }

    private static TestCaseData HiddenInvocationExpressionUsingStaticMethodReturnedLater()
    {
        const string code = @"
using System;
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            var mem = Create();
            return mem;
        }

        private static MemoryStream Create() => new MemoryStream();
    }
}";
        return new TestCaseData(code)
            .Returns(Id.ForHiddenIDisposable)
            .SetName("Hidden InvokcationExpression using static Method returned later");
    }


    private static TestCaseData HiddenInvocationExpressionUsingStaticFactoryClassReturnedLater()
    {
        const string code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            var mem = Factory.Create();
            return mem;
        }

        private static class Factory
        {
            public static MemoryStream Create() => new MemoryStream();
        }
    }
}
";
        return new TestCaseData(code)
            .Returns(Id.ForHiddenIDisposable)
            .SetName("Hidden InvokcationExpression using static factory class returned later");
    }

    private static TestCaseData HiddenInvocationExpressionUsingFactoryClassReturnedLater()
    {
        const string code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            var mem = new Factory().Create();
            return mem;
        }

        private class Factory
        {
            public MemoryStream Create() => new MemoryStream();
        }
    }
}
";
        return new TestCaseData(code)
            .Returns(Id.ForHiddenIDisposable)
            .SetName("Hidden InvokcationExpression using factory class returned later");
        }


        private static TestCaseData HiddenInvocationExpressionUsingLocalFunctionReturnedLater()
        {
            const string code = @"
using System.IO;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public object CreateDisposable()
        {
            MemoryStream Create()
            {
                return new MemoryStream();
            }

            var memoryStream = Create();
            return memoryStream;
        }
    }
}
";
            return new TestCaseData(code)
                .Returns(Id.ForHiddenIDisposable)
                .SetName("Hidden InvokcationExpression using local function returned later");
        }
    }
}
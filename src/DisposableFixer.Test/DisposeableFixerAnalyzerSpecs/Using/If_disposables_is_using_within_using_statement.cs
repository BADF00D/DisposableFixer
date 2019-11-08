using System.Collections.Generic;
using DisposableFixer.Test.Attributes;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Using
{
    [UsingStatement]
    internal class If_disposables_is_using_within_using_statement : DisposeableFixerAnalyzerSpec
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(LocalDeclarationWithObjectCreationThatIsCorrectlyDisposed, 0)
                    .SetName(
                        "LocalDeclaration with ObjectCreation given to a non tracking instance that is correctly disposed")
                    ;
                yield return new TestCaseData(FactoryCallThatIsCorrectlyDisposed, 0)
                    .SetName("FactoryCall that is correctly disposed");
                yield return new TestCaseData(ObjectCreationInUsingThatIsCorrectlyDisposed, 0)
                    .SetName("ObjectCreation in using that is correctly disposed");
                yield return new TestCaseData(FactoryCallInUsingThatIsCorrectlyDisposed, 0)
                    .SetName("FactoryCall in using that is correctly disposed");
                yield return new TestCaseData(ThreeDisablesInsideUsingBlock, 3)
                    .SetName("Three disposable inside using block");
                yield return new TestCaseData(ObjectCreationInConditionalExpressionInUsing, 0)
                    .SetName("OC in ConditionalStatement in using");
                yield return new TestCaseData(MethodInvocationInConditionalExpressionInUsing, 0)
                    .SetName("MI in ConditionalStatement in using");
                yield return new TestCaseData(ObjectCreationInConditionalExpressionInUsingWithLocalVariable, 0)
                    .SetName("OC in ConditionalStatement in using with using");
                yield return new TestCaseData(MethodInvocationInConditionalExpressionInUsingWithLocalVariable, 0)
                    .SetName("MI in ConditionalStatement in using with using");

                yield return new TestCaseData(DisposableMethodInvocationThatIsPartOfSimpleMemberAccessThatReturnsAnDisposable, 1)
                    .SetName("Disposable MI that is part of SimpleMemberAccess that returns an disposable");
                yield return new TestCaseData(DisposableObjectCreationThatIsPartOfSimpleMemberAccessThatReturnsAnDisposable,1)
                    .SetName("Disposable OC that is part of SimpleMemberAccess that returns an disposable");
            }
        }

        private const string LocalDeclarationWithObjectCreationThatIsCorrectlyDisposed = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking {
    class Test : IDisposable {
        public static void Do() {
            var mem = new MemoryStream();

            using(mem) { }
        }
    }
}
";

        private const string ObjectCreationInUsingThatIsCorrectlyDisposed = @"
using System;
using System.IO;

namespace DisFixerTest.Tracking {
    class Test : IDisposable {
        public static void Do() {
            using(new MemoryStream()) { }
        }
    }
}
";

        private const string FactoryCallThatIsCorrectlyDisposed = @"
using System;
using System.IO;
namespace DisFixerTest.Tracking {
    class Test : IDisposable {
        public static void Do() {
            var factory = new MemStreamFactory();
            var mem = factory.Create();

            using (mem) { }
        }
    }

    internal class MemStreamFactory
    {
        public MemoryStream Create()
        {
            return new MemoryStream();
        }
    }
}
";

        private const string FactoryCallInUsingThatIsCorrectlyDisposed = @"
using System;
using System.IO;
namespace DisFixerTest.Tracking {
    class NoneTracking : IDisposable {
        public static void Do() {
            var factory = new MemStreamFactory();
            using (factory.Create()) { }
        }
    }

    internal class MemStreamFactory
    {
        public MemoryStream Create()
        {
            return new MemoryStream();
        }
    }
}
";

        private const string ThreeDisablesInsideUsingBlock = @"
using System;
using System.IO;
namespace DisFixerTest.ObjectCreation {
    class ObjectCreationInUsingBlock {
        public ObjectCreationInUsingBlock() {
            using(var memStream = new MemoryStream()) 
            {
                new MemoryStream(); //this should be marked as not disposed
                var tmp = new MemoryStream(); //this should be marked as not disposed
                var tmp2 = Create();//this should be marked as not disposed
            }
        }
        private IDisposable Create() {
            return new MemoryStream();
        }
    }
}
";

        private const string MethodInvocationInConditionalExpressionInUsing = @"
using System;

internal class SomeTestNamspace
{
    Func<IDisposable> f = () => null;
    public void disposableTest(bool flag)
    {
        using (flag ? f() : f()) // whichever one we used will be disposed, but we get a warning on both
        { }
    }
}
";

        private const string ObjectCreationInConditionalExpressionInUsing = @"
using System.IO;

internal class SomeTestNamspace
{
    public void DisposableTest(bool flag)
    {
        using (flag ? new MemoryStream() : new MemoryStream())
        { }
    }
}
";
        private const string MethodInvocationInConditionalExpressionInUsingWithLocalVariable = @"
using System;

internal class SomeTestNamspace
{
    Func<IDisposable> f = () => null;
    public void disposableTest(bool flag)
    {
        using (var x = flag ? f() : f()) // whichever one we used will be disposed, but we get a warning on both
        { }
    }
}
";

        private const string ObjectCreationInConditionalExpressionInUsingWithLocalVariable = @"
using System.IO;

internal class SomeTestNamspace
{
    public void DisposableTest(bool flag)
    {
        using (var x = flag ? new MemoryStream() : new MemoryStream())
        { }
    }
}
";

        private const string DisposableObjectCreationThatIsPartOfSimpleMemberAccessThatReturnsAnDisposable = @"
using System;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        private IDisposable _field;

        public void Exchange() {
            using (var disposable = new SomeDisposable().CreateDisposable()) {
            }
        }
    }

    internal class SomeDisposable : IDisposable
    {
        public void Dispose()
        {
        }

        public SomeDisposable CreateDisposable()
        {
            return new SomeDisposable();
        }
    }
}";

        private const string DisposableMethodInvocationThatIsPartOfSimpleMemberAccessThatReturnsAnDisposable = @"
using System;

namespace RxTimeoutTest
{
    internal class SomeClass
    {
        private IDisposable _field;

        public void Exchange()
        {
            using (var disposable = Create().CreateDisposable())
            {
            }
        }
        private SomeDisposable Create() => new SomeDisposable();
    }

    internal class SomeDisposable : IDisposable
    {
        public void Dispose()
        {
        }

        public SomeDisposable CreateDisposable()
        {
            return new SomeDisposable();
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
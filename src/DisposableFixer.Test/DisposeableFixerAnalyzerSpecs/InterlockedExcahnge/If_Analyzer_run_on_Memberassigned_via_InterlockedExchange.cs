using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.InterlockedExcahnge
{
    [TestFixture]
    internal class If_Analyzer_run_on_Memberassigned_via_InterlockedExchange : DisposeableFixerAnalyzerSpec
    {
        private const string AssignedViaLocalVariableCreatedByObjectCreation = @"
using System;
using System.IO;
using System.Threading;

namespace SomeNamespace
{
    internal class SomeClass : IDisposable
    {
        private readonly IDisposable _field;

        public SomeClass()
        {
            var mem = new MemoryStream();

            Interlocked.Exchange(ref _field, mem)
                ?.Dispose();
        }

        public void Dispose()
        {
            _field?.Dispose();
        }
    }
}";

        private const string AssignedByObjectCreation = @"
using System;
using System.IO;
using System.Threading;

namespace SomeNamespace
{
    internal class SomeClass : IDisposable
    {
        private readonly IDisposable _field;

        public SomeClass()
        {
            Interlocked.Exchange(ref _field, new MemoryStream())
                ?.Dispose();
        }

        public void Dispose()
        {
            _field?.Dispose();
        }
    }
}";

        private const string AssignedViaLocalVariableCreatedByMethodInvocation = @"
using System;
using System.IO;
using System.Threading;

namespace SomeNamespace
{
    internal class SomeClass : IDisposable
    {
        private readonly IDisposable _field;

        public SomeClass()
        {
            var mem = Create();

            Interlocked.Exchange(ref _field, mem)
                ?.Dispose();
        }

        private IDisposable Create() => new MemoryStream();

        public void Dispose()
        {
            _field?.Dispose();
        }
    }
}";

        private const string AssignedByMethodInvocation = @"
using System;
using System.IO;
using System.Threading;

namespace SomeNamespace
{
    internal class SomeClass : IDisposable
    {
        private readonly IDisposable _field;

        public SomeClass()
        {
            Interlocked.Exchange(ref _field, Create())
                ?.Dispose();
        }

        private IDisposable Create() => new MemoryStream();

        public void Dispose()
        {
            _field?.Dispose();
        }
    }
}";

        private const string AssignedViaLocalVariableCreatedByMethodInvocationWhereLocalVariableIsDeclaredElsewhere = @"
using System;
using System.IO;
using System.Threading;

namespace SomeNamespace
{
    internal class SomeClass : IDisposable
    {
        private readonly IDisposable _field;

        public SomeClass()
        {
            IDisposable mem;
            mem = Create();
            Interlocked.Exchange(ref _field, mem)
                ?.Dispose();
        }

        private IDisposable Create() => new MemoryStream();

        public void Dispose()
        {
            _field?.Dispose();
        }
    }
}";

        private const string AssignedViaLocalVariableCreatedByObjectCreationWhereLocalVariableIsDeclaredElsewhere = @"
using System;
using System.IO;
using System.Threading;

namespace SomeNamespace
{
    internal class SomeClass : IDisposable
    {
        private readonly IDisposable _field;

        public SomeClass()
        {
            IDisposable mem;
            mem = new MemoryStream();
            Interlocked.Exchange(ref _field, mem)
                ?.Dispose();
        }
        
        public void Dispose()
        {
            _field?.Dispose();
        }
    }
}";

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(AssignedByObjectCreation)
                    .SetName("Assigned by ObjectCreation");
                yield return new TestCaseData(AssignedByMethodInvocation)
                    .SetName("Assigned by MethodInvocation");
                yield return new TestCaseData(AssignedViaLocalVariableCreatedByMethodInvocation)
                    .SetName("Assigned by local variable assigned by MethodInvocation");
                yield return new TestCaseData(AssignedViaLocalVariableCreatedByObjectCreation)
                    .SetName("Assigned by local variable assigned by ObjectCreation");
                yield return new TestCaseData(AssignedViaLocalVariableCreatedByObjectCreationWhereLocalVariableIsDeclaredElsewhere)
                    .SetName("Assigned by local variable assigned by ObjectCreation, where local variable was declared before");
                yield return new TestCaseData(AssignedViaLocalVariableCreatedByMethodInvocationWhereLocalVariableIsDeclaredElsewhere)
                    .SetName("Assigned by local variable assigned by MethodInvocation, where local variable was declared before");
            }
        }

        //[Test]
        [TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code)
        {
            PrintCodeToFix(code);
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Should().BeEmpty();
        }
    }
}
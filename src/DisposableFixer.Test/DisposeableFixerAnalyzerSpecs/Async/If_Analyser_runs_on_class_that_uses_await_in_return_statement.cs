using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Async
{
    [TestFixture]
    internal class If_Analyser_runs_on_a_async_MethodInvocation_assigned_to_member
    {
        private Diagnostic[] _diagnostics;

        [TestCaseSource(nameof(TestCases))]
        public string[] Should_be_correct(string code)
        {
            return MyHelper.RunAnalyser(code, new DisposableFixerAnalyzer())
                .Select(d => d.Descriptor.Id)
                .ToArray();
        }



        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return AssignedToField();
                yield return AssignedToStaticField();
                yield return AssignedToProperty();
                yield return AssignedToStaticProperty();
            }
        }

        private static TestCaseData AssignedToField()
        {
            const string Code = @"
using System.IO;
using System.Threading.Tasks;

namespace SomeNamespace
{
    public class SomeClass
    {
        private MemoryStream _mem;

        public async Task Do()
        {
            _mem = await Create();
        }
        public Task<MemoryStream> Create()
        {
            return Task.FromResult(new MemoryStream());
        }
    }
}
";

            return new TestCaseData(Code)
                .Returns(new[] { NotDisposed.Assignment.FromMethodInvocation.ToFieldNotDisposedDescriptor.Id })
                .SetName(nameof(AssignedToField));
        }

        private static TestCaseData AssignedToStaticField()
        {
            const string Code = @"
using System.IO;
using System.Threading.Tasks;

namespace SomeNamespace
{
    public class SomeClass
    {
        private static MemoryStream _mem;

        public async Task Do()
        {
            _mem = await Create();
        }
        public Task<MemoryStream> Create()
        {
            return Task.FromResult(new MemoryStream());
        }
    }
}
";

            return new TestCaseData(Code)
                .Returns(new[] { NotDisposed.Assignment.FromMethodInvocation.ToStaticFieldNotDisposedDescriptor.Id })
                .SetName(nameof(AssignedToStaticField));
        }

        private static TestCaseData AssignedToProperty()
        {
            const string Code = @"
using System.IO;
using System.Threading.Tasks;

namespace SomeNamespace
{
    public class SomeClass
    {
        private MemoryStream _mem {get; set;}

        public async Task Do()
        {
            _mem = await Create();
        }
        public Task<MemoryStream> Create()
        {
            return Task.FromResult(new MemoryStream());
        }
    }
}
";

            return new TestCaseData(Code)
                .Returns(new[] { NotDisposed.Assignment.FromMethodInvocation.ToPropertyNotDisposedDescriptor.Id })
                .SetName(nameof(AssignedToProperty));
        }

        private static TestCaseData AssignedToStaticProperty()
        {
            const string Code = @"
using System.IO;
using System.Threading.Tasks;

namespace SomeNamespace
{
    public class SomeClass
    {
        private static MemoryStream _mem {get; set;}

        public async Task Do()
        {
            _mem = await Create();
        }
        public Task<MemoryStream> Create()
        {
            return Task.FromResult(new MemoryStream());
        }
    }
}
";

            return new TestCaseData(Code)
                .Returns(new[] { NotDisposed.Assignment.FromMethodInvocation.ToStaticPropertyNotDisposedDescriptor.Id })
                .SetName(nameof(AssignedToStaticProperty));
        }


    }
}
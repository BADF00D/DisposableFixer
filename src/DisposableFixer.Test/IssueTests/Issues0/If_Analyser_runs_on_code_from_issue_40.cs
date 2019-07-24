using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_40 : IssueSpec
    {
        private const string Code = @"
using System.IO;
using System.Threading.Tasks;
namespace DisFixerTest.Async
{
    class SeperateVariableDeclarationAndAssignment
    {
        public void DoSomething(bool check)
        {
            new object().AsDisposable().Dispose();
        }
    }
    internal static class ObjectExtension {
        public static IDisposable AsDisposable(this object @object) {
        return new DisposableWrapper(@object);
    }
    private class DisposableWrapper : IDisposable {
        private readonly IDisposable _disposeable;

        public DisposableWrapper(object @object) {
            _disposeable = @object as IDisposable;
        }

        public void Dispose() {
            _disposeable?.Dispose();
        }
    }
}
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
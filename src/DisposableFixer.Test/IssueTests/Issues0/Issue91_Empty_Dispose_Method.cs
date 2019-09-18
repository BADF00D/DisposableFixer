using DisposableFixer.Analyzers;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class Issue91_Empty_Dispose_Method : IssueSpec
    {
        private const string Code = @"
using System;

namespace SomeNamespace
{
    internal class EmptyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new UnusedDisposableAnalyzer());
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Descriptor.Description.Should().Be(Unused.DisposableDescriptor.Description);
        }
    }
}
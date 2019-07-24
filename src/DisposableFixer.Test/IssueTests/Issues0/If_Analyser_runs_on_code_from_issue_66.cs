using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_66 : IssueSpec
    {
        private const string Code = @"
using System;

namespace MethodInvocationInPropertiesAlwaysYield_NotDisposedProperty {
    internal class ErrorInExtensionMethods {
        public DateTimeOffset Timestamp { get; set; }

        public bool IsNewGetter
        {
            get { return Timestamp.IsNull(); }
        }

        public bool IsNewExpressionBody => Timestamp.IsNull();

        public string EmptyStringGetter
        {
            get { return Timestamp.ReturnStringEmpty(); }
        }
    }

    public static class ObjectExtensions {
        public static bool IsNull(this object @object) {
            return ReferenceEquals(@object, null);
        }

        public static string ReturnStringEmpty(this object @object) {
            return string.Empty;
        }
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostic()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
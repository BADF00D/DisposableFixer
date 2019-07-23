using System.Linq;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_52 : IssueSpec
    {
        private const string Code = @"
namespace SomeNamespace
    public class SomeCode{
        public System.IDisposable Property => new System.IO.MemoryStream();
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_one_Diagnostics_for_Property()
        {
            _diagnostics.Length.Should().Be(1);

            var diagnostic = _diagnostics.First();
            diagnostic.Descriptor.Should()
                .Be(Descriptor.AssignmentFromObjectCreationToPropertyNotDisposedDescriptor);
        }
    }
}
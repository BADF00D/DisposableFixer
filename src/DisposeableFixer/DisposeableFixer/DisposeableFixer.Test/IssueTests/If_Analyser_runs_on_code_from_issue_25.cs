using System;
using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_25 : IssueSpec
    {
        private const string Code = @"
using System;
using System.IO;
namespace DisFixerTest.Factory {
    public class ClassThatStoredObjectCreationLocally2 {
        public IDisposable Create() {
            var inst = new MemoryStreamFactory().Create(); //this should yield a diagnostic

            return null;
        }

        internal class MemoryStreamFactory {
            public IDisposable Create() {
                return new MemoryStream();
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
        public void Then_there_should_be_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }
}
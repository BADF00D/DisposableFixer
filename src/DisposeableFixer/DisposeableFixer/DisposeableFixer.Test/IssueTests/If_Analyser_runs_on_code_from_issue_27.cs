using System;
using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_27 : IssueSpec
    {
        private const string Code = @"
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
}";

        private Diagnostic[] _diagnostics;


        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void Then_there_should_be_three_Diagnostics()
        {
            _diagnostics.Length.Should().Be(3);
        }
    }
    class ObjectCreationInUsingBlock {
        public ObjectCreationInUsingBlock() {
            using (var memStream = new MemoryStream()) {
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
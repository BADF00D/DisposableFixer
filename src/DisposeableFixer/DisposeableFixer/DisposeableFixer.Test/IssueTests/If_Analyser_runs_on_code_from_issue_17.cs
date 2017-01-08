using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_17 : IssueSpec
    {
        private readonly string _code = @"
using System;
using System.IO;

namespace DisFixerTest.ObjectCreationAssignedToField {
    internal class EnumeratorYieldsDiagnostics : IDisposable
    {
        private IDisposable _subscrioption;

        public void Init() {
            _subscrioption = new MemoryStream();
        }
        
        public void Dispose()
        {
            _subscrioption.Dispose();
        }
    }
}";

        private Diagnostic[] _diagnostics;


        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(_code, Sut);
        }

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
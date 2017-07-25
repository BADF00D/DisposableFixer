using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_41 : IssueSpec
    {
        private const string Code = @"
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Disposeables {
	public interface IDoNothing{}
    public class Implementation : IDoNothing, IDisposable{
        public void Dispose(){}
    }
    
    public class Usage{
        private readonly IDoNothing _object;
        public Usage(){
            _object = new Implementation(); //this is an undisposed IDisposable that is not recognized
        }
    }
}
";

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
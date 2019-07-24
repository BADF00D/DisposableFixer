using System;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues0
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_92 : IssueSpec
    {
        private const string Code = @"
namespace SelectManyTest
{
    internal class Dummy
    {
        #region ObjectCreations
        public object ObjectCreationWithGetter { get { return new object(); } }//Here Should be no Property not disposed warning
        public object ObjectCreationWithGetterAndLocalVariable {
            get {
                var objectCreationWithGetterAndLocalVariable = new object();
                return objectCreationWithGetterAndLocalVariable;
            }
        }
        public object ObjectCreationWithExpressionBody => new object();

        #endregion

        #region Constants
        public object Constant { get { return 0; } }
        public object ConstantWithExpressionBody => 0;
        #endregion

        #region MethodInvocation
        public object MethodInvaocationWithGetter { get { return Create(); } }
        public object MethodInvaocationWithExpressionBody => Create();
        public object Create() => new object();
        #endregion
    }
}";

        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            Console.WriteLine(Code);

            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        [Test]
        public void There_should_be_no_diagnosics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}
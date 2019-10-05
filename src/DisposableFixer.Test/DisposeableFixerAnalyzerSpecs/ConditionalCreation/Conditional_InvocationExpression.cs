using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Conditional_InvocationExpression : IssueSpec
    {

        private const string Code = @"
using System.IO;

namespace ConsoleApp
{
    class Program
    {
        static Stream _pipe;
        static void X(string[] args)
        {
            StreamWriter Create(Stream s) => new StreamWriter(s);
            StreamWriter sw = _pipe != null ? Create(_pipe): null;
            try
            {
            }
            finally
            {
                sw?.Dispose();
            }
        }
    }
}

";

        [Test]
        public void Then_there_should_be_no_diagnostic()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}
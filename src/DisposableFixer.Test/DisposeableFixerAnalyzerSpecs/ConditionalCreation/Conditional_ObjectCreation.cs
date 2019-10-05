using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Conditional_ObjectCreation : IssueSpec
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
            StreamWriter sw = _pipe != null ? new StreamWriter(_pipe) { AutoFlush = true } : null;
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
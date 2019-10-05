using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue131_ConditionalObjectCreation : IssueSpec
    {

        private const string Code = @"
using System.IO;

namespace ConsoleApp
{
    class Program
    {
        static Stream _pipe;
        static void Main(string[] args)
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
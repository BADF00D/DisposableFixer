using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal abstract class IssueSpec : Spec
    {
        protected readonly DisposableFixerAnalyzer Sut;

        protected IssueSpec()
        {
            Sut = new DisposableFixerAnalyzer();
        }
    }
}
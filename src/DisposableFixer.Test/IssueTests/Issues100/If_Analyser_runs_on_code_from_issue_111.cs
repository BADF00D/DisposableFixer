using DisposableFixer.Test.CodeFix;
using FluentAssertions;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_111 : DisposableAnalyserCodeFixVerifierSpec
    {

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DisposableFixer.CodeFix.WrapAnonymousObjectsInUsingCodeFixProvider();
        }

        private const string Code = @"
namespace RxTimeoutTest
{
    internal class SomeClass
    {
        public void Exchange()
        {
            var doc = XDocument.Load(new StringReader("" <? xml >< list ></ list > ""));
            var mgr = new XmlNamespaceManager(doc.CreateReader()?.NameTable ?? new NameTable());
            mgr.AddNamespace(""a"", ""bla"");
        }
}
internal class StringReader : System.IDisposable
{
    private string v;

    public StringReader(string v)
    {
        this.v = v;
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
internal class XDocument
{
    public static XDocument Load(StringReader reader)
    {
        return new XDocument();
    }
    public XmlReader CreateReader()
    {
        return new XmlReader();
    }
}
internal class XmlReader : System.IDisposable
{
    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
    public XmlNameTable NameTable { get; }
}
internal class XmlNameTable { }
internal class NameTable : XmlNameTable { }
internal class XmlNamespaceManager
{
    public XmlNamespaceManager(XmlNameTable nt) { }
    public void AddNamespace(string x, string y) { }
}
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToFix(Code);
            var beforeCodefixDiagnostics = MyHelper.RunAnalyser(Code, GetCSharpDiagnosticAnalyzer());
            var forAnonymousObjectFromObjectCreation = Id.ForAnonymousObjectFromObjectCreation;
            beforeCodefixDiagnostics
                .Should().Contain(d => d.Id == forAnonymousObjectFromObjectCreation, "this should not be fixed");
            var forAnonymousObjectFromMethodInvocation = Id.ForAnonymousObjectFromMethodInvocation;
            beforeCodefixDiagnostics
                .Should().Contain(d => d.Id == forAnonymousObjectFromMethodInvocation, "this should be fixed");

            var fixedCode = ApplyCSharpCodeFixTo(Code, d => d.Id == forAnonymousObjectFromMethodInvocation);
            PrintFixedCode(fixedCode);

            var cSharpCompilerDiagnostics = GetCSharpCompilerErrors(fixedCode);
            PrintFixedCodeDiagnostics(cSharpCompilerDiagnostics);
            cSharpCompilerDiagnostics
                .Should().HaveCount(0, "we don't want to introduce bugs");

            var diagnostics = MyHelper.RunAnalyser(fixedCode, GetCSharpDiagnosticAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics.Should()
                .Contain(d => d.Id == forAnonymousObjectFromObjectCreation, "this should not have been fixed");
        }
    }
}
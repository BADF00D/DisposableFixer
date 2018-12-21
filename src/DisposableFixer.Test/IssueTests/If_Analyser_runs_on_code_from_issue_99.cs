using System.Xml;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests
{
    [TestFixture]
    internal class If_Analyser_runs_on_code_from_issue_99 : IssueSpec
    {
        private const string Code = @"
using System.IO;
using System.Xml;

namespace ExtensionMethodYieldsNotDisposed
{
    internal class Tracking
    {
        public Tracking()
        {
            var x1 = new XmlTextReader(new FileStream(string.Empty, FileMode.CreateNew));
            x1.Dispose();
            var x2 = new XmlTextReader(new FileStream(string.Empty, FileMode.CreateNew), new NameTable());
            x2.Dispose();
            var xmlParserContext = new XmlParserContext();
            var x3 = new XmlTextReader(new FileStream(string.Empty, FileMode.CreateNew),XmlNodeType.CDATA,xmlParserContext);
            x3.Dispose();
            var x4 = new XmlTextReader(new StringReader(string.Empty));
            x4.Dispose();
            var x5 = new XmlTextReader(new StringReader(string.Empty), new NameTable());
            x5.Dispose();
            var x6 = new XmlTextReader(string.Empty);
            x6.Dispose();
            var x7 = new XmlTextReader(string.Empty, new FileStream(string.Empty, FileMode.Append));
            x7.Dispose();
            var x8 = new XmlTextReader(string.Empty, new FileStream(string.Empty, FileMode.Append), new NameTable());
            x8.Dispose();
            var x9 = new XmlTextReader(string.Empty, new StringReader(string.Empty));
            x9.Dispose();
            var x10 = new XmlTextReader(string.Empty, new StringReader(string.Empty), new NameTable());
            x10.Dispose();
            var x11 = new XmlTextReader(string.Empty, new NameTable());
            x11.Dispose();
            var x12 = new XmlTextReader(string.Empty, XmlNodeType.Attribute, xmlParserContext);
            x12.Dispose();
        }
    }
}

namespace System.Xml {
    public class XmlTextReader : IDisposable {
        public XmlTextReader(Stream input){}
        public XmlTextReader(Stream input, XmlNameTable nt){}
        public XmlTextReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context){}
        public XmlTextReader(TextReader input){}
        public XmlTextReader(TextReader input, XmlNameTable nt){}
        public XmlTextReader(string url){}
        public XmlTextReader(string url, Stream input){}
        public XmlTextReader(string url, Stream input, XmlNameTable nt){}
        public XmlTextReader(string url, TextReader input){}
        public XmlTextReader(string url, TextReader input, XmlNameTable nt){}
        public XmlTextReader(string url, XmlNameTable nt){}
        public XmlTextReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context){}
        public void Dispose() {
        }
    }
    public abstract class XmlNameTable{}
    public class NameTable : XmlNameTable {}
    public enum XmlNodeType
      {
        None,
        Attribute,
        Text,
        CDATA,
      }
    public class XmlParserContext {}
    public class XmlNamespaceManager {}
}";
        [Test]
        public void Apply_CodeFix_should_not_throw_Exception()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, Sut);
            diagnostics.Length.Should().Be(0);
        }
    }
}
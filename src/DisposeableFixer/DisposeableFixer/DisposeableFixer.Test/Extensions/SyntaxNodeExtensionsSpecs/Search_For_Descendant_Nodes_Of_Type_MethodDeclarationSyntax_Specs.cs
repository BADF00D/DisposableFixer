using System.Collections.Generic;
using System.Linq;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DisposableFixer.Test.Extensions.SyntaxNodeExtensionsSpecs
{
    [TestFixture]
    internal class Search_For_Descendant_Nodes_Of_Type_MethodDeclarationSyntax_Specs : Spec
    {
        private IEnumerable<MethodDeclarationSyntax> _methodDeclarationFromLongVersion;
        private IEnumerable<MethodDeclarationSyntax> _methodDeclarationFromShortVersion;
        private const string Code = @"
using System.IO;
namespace DisFixerTest.Misc
{
    public class ClassWithOtherUsing {
        public ClassWithOtherUsing()
        {
            using(var memstream = new MemoryStream()) { }
        }
        public string Dis { get; set; }
        private void Do() { }
        private string Bla;
    }
}
";

        protected override void BecauseOf()
        {
            var node = MyHelper.CompileAndRetrieveRootNode(Code);


            _methodDeclarationFromLongVersion = node.DescendantNodes().OfType<MethodDeclarationSyntax>();
            _methodDeclarationFromShortVersion = node.DescendantNodes<MethodDeclarationSyntax>();
        }

        [Test]
        public void AbbreviationShouldReturnSameAsLongVersion()
        {
            _methodDeclarationFromLongVersion.Should().ContainInOrder(_methodDeclarationFromShortVersion);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using DisposeableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DisposeableFixer.Test.Extensions.SyntaxNodeExtensionsSpecs
{
    [TestFixture]
    internal class Search_For_Descendant_Nodes_Of_Type_PropertyDeclarationSyntax_Specs : Spec {
        private IEnumerable<PropertyDeclarationSyntax> _methodDeclarationFromLongVersion;
        private IEnumerable<PropertyDeclarationSyntax> _methodDeclarationFromShortVersion;
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
        protected override void BecauseOf() {
            var node = MyHelper.CompileAndRetrieveRootNode(Code);


            _methodDeclarationFromLongVersion = node.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            _methodDeclarationFromShortVersion = node.DescendantNodes<PropertyDeclarationSyntax>();
        }

        [Test]
        public void AbbreviationShouldReturnSameAsLongVersion() {
            _methodDeclarationFromLongVersion.Should().ContainInOrder(_methodDeclarationFromShortVersion);
        }
    }
}
using System.Linq;
using DisposableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DisposableFixer.Test.Extensions.SyntaxNodeExtensionsSpecs
{
    [TestFixture]
    internal class If_FindContainingClass_is_called_on_ClassDeclaration_that_is_no_private_class : Spec
    {
        private ClassDeclarationSyntax _classDeclarationSyntax;
        private const string Code = @"
namespace DisFixerTest.Misc
{
    public class ClassWithFieldBla
    {
        public ClassWithFieldBla(){}
        public string Dis { get; set; }
        private void Do(){}
        private string Bla;

        private class PrivateClass
        {
            public PrivateClass(){}
            public string Dis { get; set; }
            private void Do() {}
            private string Bla;
        }
    }
}
";

        protected override void BecauseOf()
        {
            var namespaceDeclarationSyntax = MyHelper.CompileAndRetrieveRootNode(Code)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            _classDeclarationSyntax = namespaceDeclarationSyntax.FindContainingClass();
        }


        [Test]
        public void Then_Result_should_be_null()
        {
            _classDeclarationSyntax.Should().BeNull();
        }
    }
}
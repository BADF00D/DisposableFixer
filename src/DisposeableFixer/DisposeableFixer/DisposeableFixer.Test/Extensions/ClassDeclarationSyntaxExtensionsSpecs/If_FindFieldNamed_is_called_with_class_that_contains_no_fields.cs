using System.Linq;
using DisposeableFixer.Extensions;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DisposeableFixer.Test.Extensions.ClassDeclarationSyntaxExtensionsSpecs
{
    [TestFixture]
    internal class If_FindFieldNamed_is_called_with_class_that_contains_no_fields : Spec
    {
        private FieldDeclarationSyntax _fieldDeclaration;
        private const string Code = @"
using System;
namespace DisFixerTest.Misc{
    public class ClassWithtoutFields    {
        public ClassWithtoutFields()
        {
            Console.WriteLine("");
        }

        public string Dis { get; set; }

        private void Do()
        {
        }
    }
}
";
        protected override void BecauseOf()
        {
            var classDeclaration = MyHelper.CompileAndRetrieveRootNode(Code)
               .DescendantNodes()
               .OfType<ClassDeclarationSyntax>()
               .FirstOrDefault();

            _fieldDeclaration = classDeclaration.FindFieldNamed("Bla");
        }

        [Test]
        public void Then_result_should_be_null()
        {
            _fieldDeclaration.Should().BeNull();
        }
    }
}
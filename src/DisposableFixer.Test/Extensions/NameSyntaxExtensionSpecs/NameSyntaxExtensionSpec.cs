using System;
using System.Collections.Generic;
using System.Linq;
using DisposableFixer.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DisposableFixer.Test.Extensions.NameSyntaxExtensionSpecs
{
    [TestFixture]
    public class If_checking_SimpleNameSyntax : Spec
    {
        [TestCaseSource(nameof(TestCases))]
        public bool Should(string code)
        {
            var root = MyHelper.CompileAndRetrieveRootNode(code);
            var @class = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .First();
            var baseTypeName = @class.BaseList?.Types
                .Select(t => t.Type)
                .Cast<NameSyntax>()
                .FirstOrDefault();

            return baseTypeName.IsIDisposable();
        }

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                const string codeTemplate = @"
using System;
using IDis = System.IDisposable;
namespace SomeNamespace
{
    internal class Base : ###
    {
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}";

                yield return new TestCaseData(codeTemplate.Replace("###", "IDisposable"))
                    .SetName("IDisposable")
                    .Returns(true);
                yield return new TestCaseData(codeTemplate.Replace("###", "System.IDisposable"))
                    .SetName("System IDisposable")
                    .Returns(true);
                yield return new TestCaseData(codeTemplate.Replace("###", "IDis"))
                    .SetName("With Alias IDis")
                    .Returns(true)
                    .Ignore("Currently not implemented. Alias IDis is detected as SimpleNameSyntax");
                yield return new TestCaseData(codeTemplate.Replace("###", "System.MemoryStream"))
                    .SetName("object")
                    .Returns(false);
            }
        }
    }
}
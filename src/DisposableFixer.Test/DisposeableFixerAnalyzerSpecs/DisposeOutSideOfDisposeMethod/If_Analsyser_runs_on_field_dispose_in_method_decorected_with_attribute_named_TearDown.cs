using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.DisposeOutSideOfDisposeMethod
{
    [TestFixture]
    internal class If_Analsyser_runs_on_field_dispose_in_method_decorected_with_attribute_named_TearDown :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }
        private const string Code = @"
using System;
using NUnit.Framework;
namespace SomeNamespace
    public class SomeSpec : Spec{
        private System.IDisposable _memstream;

        public void Create(){
            _memstream = new System.IO.MemoryStream();
        }        

        [TearDown]
        public void TearDown(){
            _memstream?.Dispose();
        }

        
    }
}
namespace NUnit.Framework{
    public class TearDownAttribute : System.Attribute
    {
    }
}


";

        [Test]
        public void Then_there_should_be_no_Diagnostics()
        {
            _diagnostics.Length.Should().Be(0);
        }
    }
}

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Async
{
    [TestFixture]
    internal class Checking_Descriptor_when_assigning_a_Disposable_to_anothers_instance_class_within_ctor : ATest
    {
        
        private const string CodeTemplate = @"
using System;
using System.IO;

namespace MyNamespace
{
    class ClassWithPublicSetableProperty : IDisposable
    {

        public IDisposable Property { get; set; }
        public static IDisposable StaticProperty { get; set; }
        public IDisposable Field;
        public static IDisposable StaticField;

        public void Dispose()
        {
            Property?.Dispose();
        }
    }

    class MyClass
    {
        public MyClass()
        {
            Func<MemoryStream> create= ()=> new MemoryStream();
            using (var instance = new ClassWithPublicSetableProperty())
            {
                instance.###CALL###
            }
        }
    }
}
";

        [TestCaseSource(nameof(TestCases))]
        public DiagnosticDescriptor There_should_be_one_Diagnostic_with_correct_Descriptor(string code)
        {
            var diagnostic = MyHelper.RunAnalyser(code, new DisposableFixerAnalyzer());
            diagnostic.Length.Should().Be(1);

            return diagnostic[0].Descriptor;
        }

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                TestCaseData Create(string code)
                {
                    var codeToAnalyze = CodeTemplate.Replace("###CALL###", code);
                    return new TestCaseData(codeToAnalyze);
                }
                
                yield return Create("StaticField = create();")
                    .SetName("MethodInvocation static field")
                    .Returns(NotDisposed.Assignment.FromMethodInvocation.ToStaticField.OfAnotherTypeDescriptor);
                yield return Create("Field = create();")
                    .SetName("MethodInvocation non static field")
                    .Returns(NotDisposed.Assignment.FromMethodInvocation.ToField.OfAnotherTypeDescriptor);
                yield return Create("StaticField = new MemoryStream();")
                    .SetName("ObjectCreation static field")
                    .Returns(NotDisposed.Assignment.FromObjectCreation.ToStaticField.OfAnotherTypeDescriptor);
                yield return Create("Field = new MemoryStream();")
                    .SetName("ObjectCreation non static field")
                    .Returns(NotDisposed.Assignment.FromObjectCreation.ToField.OfAnotherTypeDescriptor);

                yield return Create("StaticProperty = create();")
                    .SetName("MethodInvocation static Property")
                    .Returns(NotDisposed.Assignment.FromMethodInvocation.ToStaticProperty.OfAnotherTypeDescriptor);
                yield return Create("Property = create();")
                    .SetName("MethodInvocation non static Property")
                    .Returns(NotDisposed.Assignment.FromMethodInvocation.ToProperty.OfAnotherTypeDescriptor);
                yield return Create("StaticProperty = new MemoryStream();")
                    .SetName("ObjectCreation static Property")
                    .Returns(NotDisposed.Assignment.FromObjectCreation.ToStaticProperty.OfAnotherTypeDescriptor);
                yield return Create("Property = new MemoryStream();")
                    .SetName("ObjectCreation non static Property")
                    .Returns(NotDisposed.Assignment.FromObjectCreation.ToProperty.OfAnotherTypeDescriptor);
            }
        }
    }
}
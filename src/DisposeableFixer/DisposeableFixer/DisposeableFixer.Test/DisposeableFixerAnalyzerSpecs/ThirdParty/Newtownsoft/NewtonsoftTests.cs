using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.Newtownsoft
{
    internal class NewtonsoftTests : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases {
            get {
                yield return JsonTextReader();
                yield return JsonTextWriter();
                yield return BsonWriter();
                yield return BsonReader();
            }
        }

        private const string NewtonsoftDefinition = @"
namespace Newtonsoft.Json
{
    public class JsonTextWriter : IDisposable
    {
        public JsonTextWriter(TextWriter writer)
        {
        }

        public void Dispose()
        {
        }
    }

    public class JsonTextReader : IDisposable
    {
        public JsonTextReader(TextReader reader)
        {

        }

        public void Dispose()
        {
        }
    }

   
}

namespace Newtonsoft.Json.Bson
{

    public class BsonWriter : IDisposable
    {
        public BsonWriter(Stream stream)
        {
        }

        public void Dispose()
        {
        }
    }

    public class BsonReader : IDisposable
    {
        public BsonReader(Stream stream)
        {

        }

        public void Dispose()
        {
        }
    }
}

";

        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }

        private static TestCaseData JsonTextReader()
        {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Newtonsoft.Json
            using System.IO;

            namespace SomeNamespace {
                public class SomeClass {
                    public SomeClass(){
                        using (var r = new JsonTextReader(new StreamReader(new MemoryStream()))){}
                    }
                }
            }" + NewtonsoftDefinition;

            return new TestCaseData(code, 0)
                .SetName("JsonTextReader");
        }

        private static TestCaseData JsonTextWriter()
        {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Newtonsoft.Json
            using System.IO;

            namespace SomeNamespace {
                public class SomeClass {
                    public SomeClass(){
                        using (var r = new JsonTextWriter(new StreamWriter(new MemoryStream()))){}
                    }
                }
            }" + NewtonsoftDefinition;

            return new TestCaseData(code, 0)
                .SetName("JsonTextWriter");
        }
        private static TestCaseData BsonReader()
        {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Newtonsoft.Json.Bson
            using System.IO;

            namespace SomeNamespace {
                public class SomeClass {
                    public SomeClass(){
                        using (var r = new BsonReader(new MemoryStream())) { }
                    }
                }
            }" + NewtonsoftDefinition;

            return new TestCaseData(code, 0)
                .SetName("BsonReader");
        }

        private static TestCaseData BsonWriter()
        {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Newtonsoft.Json.Bson
            using System.IO;

            namespace SomeNamespace {
                public class SomeClass {
                    public SomeClass(){
                        using (var r = new BsonWriter(new MemoryStream())) { }
                    }
                }
            }" + NewtonsoftDefinition;

            return new TestCaseData(code, 0)
                .SetName("BsonWriter");
        }
    }
}
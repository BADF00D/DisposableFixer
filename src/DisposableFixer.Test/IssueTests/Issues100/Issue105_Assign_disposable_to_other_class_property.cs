using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    [TestFixture]
    internal class Issue105_Assign_disposable_to_other_class_property_via_object_initialization : IssueSpec
    {

        private const string Code = @"
using System.Net;
using System.Net.Http;

namespace SomeNamespace
{
    internal class HttpResponseMessageSpec
    {
        public HttpResponseMessage CreateResponse1()
        {
            var msg = new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                /* This should not yield an error because this is initialization and we Content if null after ctor was called. TrackedSet(Once)*/
                Content = new StringContent(""some content"")
            };
            
            return msg;
        }
    }
}
namespace System.Net
{
    public enum HttpStatusCode
    {
        Accepted = 201
    }
}
namespace System.Net.Http
{
    
    public class HttpResponseMessage : IDisposable
    {
        public HttpContent Content { get; set; }

        public HttpResponseMessage(HttpStatusCode status){}
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class HttpContent : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class StringContent : HttpContent
    {
        public StringContent(string data)
        {

        }
    }
}

";
        [Test]
        public void Then_there_should_be_no()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }

    [TestFixture]
    internal class Issue105_Assign_disposable_to_other_class_property_via_property_setter : IssueSpec
    {

        private const string Code = @"
using System.Net;
using System.Net.Http;

namespace SomeNamespace
{
    internal class HttpResponseMessageSpec
    {
        public HttpResponseMessage CreateResponse2()
        {
            var msg = new HttpResponseMessage(HttpStatusCode.Accepted);
            /* This should yield an diagnostic, because there might be a previous resource that is not disposed.
             This would be allowed, if Content is marked with TrackingSetAttribute(Always) */
            msg.Content = new StringContent(""some content"");

            return msg;
        }
    }
}
namespace System.Net
{
    public enum HttpStatusCode
    {
        Accepted = 201
    }
}
namespace System.Net.Http
{
    
    public class HttpResponseMessage : IDisposable
    {
        public HttpContent Content { get; set; }

        public HttpResponseMessage(HttpStatusCode status){}
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class HttpContent : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class StringContent : HttpContent
    {
        public StringContent(string data)
        {

        }
    }
}
";
        [Test]
        public void Then_there_should_be_no()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Id.Should().Be(Id.ForAssignment.FromObjectCreation.ToProperty.OfAnotherType);
        }
    }


    [TestFixture]
    internal class Issue105_Assign_disposable_to_other_class_property_via_property_setter_that_is_not_tracked_set : IssueSpec
    {

        private const string Code = @"
using System;
using System.IO;

namespace SomeNamespace
{
    internal class HttpResponseMessageSpec
    {
        public HttpResponseMessageSpec()
        {
            var some = new SomeClass
            {
                SomeProperty = new MemoryStream()
            };
        }
    }

    public class SomeClass
    {
        public IDisposable SomeProperty { get; set; }
    }
}
";
        [Test]
        public void Then_there_should_be_no()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(1);
            diagnostics[0].Id.Should().Be(Id.ForAssignment.FromObjectCreation.ToProperty.OfAnotherType);
        }
    }

    [TestFixture]
    internal class Issue105_Assign_disposable_to_other_class_property_via_object_initialization_and_this_property_is_defined_in_base_class_as_tracked_set : IssueSpec
    {

        private const string Code = @"
using System.Net;
using System.Net.Http;

namespace SomeNamespace
{
    internal class HttpResponseMessageSpec
    {
        public HttpResponseMessage CreateResponse1()
        {
            var msg = new RtcRequestMessage(HttpStatusCode.Accepted)
            {
                /* This should not yield an error because this is initialization and we Content if null after ctor was called. TrackedSet(Once)*/
                Content = new StringContent(""some content"")
            };
            
            return msg;
        }
    }
}
namespace System.Net
{
    public enum HttpStatusCode
    {
        Accepted = 201
    }
}
namespace System.Net.Http
{
    
    public class HttpResponseMessage : IDisposable
    {
        public HttpContent Content { get; set; }

        public HttpResponseMessage(HttpStatusCode status){}
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class RtcRequestMessage : HttpResponseMessage {}

    public class HttpContent : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class StringContent : HttpContent
    {
        public StringContent(string data)
        {
        }
    }
}

";
        [Test]
        public void Then_there_should_be_no()
        {
            PrintCodeToAnalyze(Code);
            var diagnostics = MyHelper.RunAnalyser(Code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }
    }
}




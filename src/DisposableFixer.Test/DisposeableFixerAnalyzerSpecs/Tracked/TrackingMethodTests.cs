using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.Tracked
{
    [TestFixture]
    internal class TrackingMethodTests : ATest
    {
        private const string CompositeDisposable = @"
namespace System.Reactive.Disposables
{
    public class CompositeDisposable : IDisposable
    {
        public void Add(IDisposable item)
        {
        }

        public void Dispose()
        {
        }
    }
}";
        
        [TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no(string code)
        {
            PrintCodeToAnalyze(code);
            var diagnostics = MyHelper.RunAnalyser(code, new DisposableFixerAnalyzer());
            diagnostics.Should().HaveCount(0);
        }

        private static IEnumerable<TestCaseData> TestCases
        {
            get {
                yield return FieldTrackedByInvocationOfTrackingMethodAndCreatedByObjectCreationInCtor();
                yield return FieldTrackedByInvocationOfTrackingMethodAndCreatedByInvocationExpressionInCtor();
                yield return FieldTrackedByInvocationOfTrackingMethodAndCreatedByObjectCreationInMethod();
                yield return FieldTrackedByInvocationOfTrackingMethodAndCreatedByInvocationExpressionInMethod();

                yield return PropertyTrackedByInvocationOfTrackingMethodAndCreatedByObjectCreationInCtor();
                yield return PropertyTrackedByInvocationOfTrackingMethodAndCreatedByInvocationExpressionInCtor();
                yield return PropertyTrackedByInvocationOfTrackingMethodAndCreatedByObjectCreationInMethod();
                yield return PropertyTrackedByInvocationOfTrackingMethodAndCreatedByInvocationExpressionInMethod();
            }
        }

        private static TestCaseData FieldTrackedByInvocationOfTrackingMethodAndCreatedByObjectCreationInCtor()
        {
            var code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

public class WithTrackingMethod : IDisposable
{
    private MemoryStream field;
    private CompositeDisposable disposables = new CompositeDisposable();
    public WithTrackingMethod()
    {
        field = new MemoryStream(); //Warning : Field not disposed, should not be
        
        disposables.Add(field);
    }
    public void Dispose()
    {
        disposables.Dispose();
    }
}
";
            return new TestCaseData(code + CompositeDisposable)
                .SetName("Field tracked by tracking method and created by ObjectCreation in ctor");
        }

        private static TestCaseData FieldTrackedByInvocationOfTrackingMethodAndCreatedByInvocationExpressionInCtor()
        {
            var code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

public class WithTrackingMethod : IDisposable
{
    private MemoryStream field;
    private CompositeDisposable disposables = new CompositeDisposable();
    public WithTrackingMethod()
    {
        field = Create(); //Warning : Field not disposed, should not be
        
        disposables.Add(field);
    }
    private MemoryStream Create() => new MemoryStream();
    public void Dispose()
    {
        disposables.Dispose();
    }
}

";
            return new TestCaseData(code + CompositeDisposable)
                .SetName("Field tracked by tracking method and created by InvocationExpression in ctor");
        }

        private static TestCaseData FieldTrackedByInvocationOfTrackingMethodAndCreatedByObjectCreationInMethod()
        {
            var code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

public class WithTrackingMethod : IDisposable
{
    private MemoryStream field;
    private CompositeDisposable disposables = new CompositeDisposable();
    public void Method()
    {
        field = new MemoryStream(); //Warning : Field not disposed, should not be
        
        disposables.Add(field);
    }
    public void Dispose()
    {
        disposables.Dispose();
    }
}
";
            return new TestCaseData(code + CompositeDisposable)
                .SetName("Field tracked by tracking method and created by ObjectCreation in method");
        }

        private static TestCaseData FieldTrackedByInvocationOfTrackingMethodAndCreatedByInvocationExpressionInMethod()
        {
            var code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

public class WithTrackingMethod : IDisposable
{
    private MemoryStream field;
    private CompositeDisposable disposables = new CompositeDisposable();
    public void Method()
    {
        field = Create(); //Warning : Field not disposed, should not be
        
        disposables.Add(field);
    }
    private MemoryStream Create() => new MemoryStream();
    public void Dispose()
    {
        disposables.Dispose();
    }
}

";
            return new TestCaseData(code + CompositeDisposable)
                .SetName("Field tracked by tracking method and created by InvocationExpression in method");
        }

        private static TestCaseData PropertyTrackedByInvocationOfTrackingMethodAndCreatedByObjectCreationInCtor()
        {
            var code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

public class WithTrackingMethod : IDisposable
{
    private MemoryStream property {get; set;}
    private CompositeDisposable disposables = new CompositeDisposable();
    public WithTrackingMethod()
    {
        property = new MemoryStream(); //Warning : Field not disposed, should not be
        
        disposables.Add(property);
    }
    public void Dispose()
    {
        disposables.Dispose();
    }
}
";
            return new TestCaseData(code + CompositeDisposable)
                .SetName("Property tracked by tracking method and created by ObjectCreation in ctor");
        }

        private static TestCaseData PropertyTrackedByInvocationOfTrackingMethodAndCreatedByInvocationExpressionInCtor()
        {
            var code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

public class WithTrackingMethod : IDisposable
{
    private MemoryStream property {get; set;}
    private CompositeDisposable disposables = new CompositeDisposable();
    public WithTrackingMethod()
    {
        property = Create(); //Warning : Field not disposed, should not be
        
        disposables.Add(property);
    }
    private MemoryStream Create() => new MemoryStream();
    public void Dispose()
    {
        disposables.Dispose();
    }
}

";
            return new TestCaseData(code + CompositeDisposable)
                .SetName("Property tracked by tracking method and created by InvocationExpression in ctor");
        }

        private static TestCaseData PropertyTrackedByInvocationOfTrackingMethodAndCreatedByObjectCreationInMethod()
        {
            var code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

public class WithTrackingMethod : IDisposable
{
    private MemoryStream property {get; set;}
    private CompositeDisposable disposables = new CompositeDisposable();
    public void Method()
    {
        property = new MemoryStream(); //Warning : Field not disposed, should not be
        
        disposables.Add(property);
    }
    public void Dispose()
    {
        disposables.Dispose();
    }
}
";
            return new TestCaseData(code + CompositeDisposable)
                .SetName("Property tracked by tracking method and created by ObjectCreation in method");
        }

        private static TestCaseData PropertyTrackedByInvocationOfTrackingMethodAndCreatedByInvocationExpressionInMethod()
        {
            var code = @"
using System;
using System.IO;
using System.Reactive.Disposables;

public class WithTrackingMethod : IDisposable
{
    private MemoryStream property {get; set;}
    private CompositeDisposable disposables = new CompositeDisposable();
    public void Method()
    {
        property = Create(); //Warning : Field not disposed, should not be
        
        disposables.Add(property);
    }
    private MemoryStream Create() => new MemoryStream();
    public void Dispose()
    {
        disposables.Dispose();
    }
}

";
            return new TestCaseData(code + CompositeDisposable)
                .SetName("Property tracked by tracking method and created by InvocationExpression in method");
        }
    }
}
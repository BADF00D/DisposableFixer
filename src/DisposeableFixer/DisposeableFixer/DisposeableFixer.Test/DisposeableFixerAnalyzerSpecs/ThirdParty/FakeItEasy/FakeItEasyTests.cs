using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.FakeItEasy
{
    internal class FakeItEasyTests : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return AFakeOfT();
                yield return AStrictFakeOfT();
                yield return ACollectionOfFake_of_T();
                yield return ACollectionOfFake_of_T_strict();
                yield return ADummy();
                yield return ACollectionOfDummy();
                yield return A_of_T_Ignored();
                yield return A_of_T_Ignored2();
                yield return A_IDisposable_That_Matches();
                yield return A_IDisposable_That_Matches_with_Action_of_IOutputWriter();
            }
        }

        private const string FakeItEasyDefintion = @"
namespace FakeItEasy {
    public static class A {
        public static T Fake<T>(Action<IFakeOptions<T>> optionBuilder) {
            throw new System.Exception();
        }
        public static T Fake<T>() {
            throw new System.Exception();
        }
        public static IList<T> CollectionOfFake<T>(int numberOfFakes) {
            throw new NotImplementedException();
        }
        public static IList<T> CollectionOfFake<T>(int numberOfFakes, Action<IFakeOptions<T>> optionsBuilder) {
            throw new NotImplementedException();
        }
        public static T Dummy<T>() {
            throw new System.Exception();
        }
        public static IList<T> CollectionOfDummy<T>(int numberOfDummies) {
            throw new NotImplementedException();
        }
    }
    public interface IArgumentConstraintManager<T> {
        IArgumentConstraintManager<T> Not { get; }
        T Matches(Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter);
    }
    public interface IOutputWriter {
        IOutputWriter Write(string value);
        IOutputWriter WriteArgumentValue(object value);
        IDisposable Indent();
    }

    public static class A<T> {
        public static IArgumentConstraintManager<T> That
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public static T _ => Ignored;
        public static T Ignored {
            get { throw new NotImplementedException(); }
        }
    }

    public static class FakeOptionsExtensions {
        public static IFakeOptions<T> Strict<T>(this IFakeOptions<T> options) {
            throw new NotImplementedException();
        }

    }
    public static class ArgumentConstraintManagerExtensions {
        public static T IsNull<T>(this IArgumentConstraintManager<T> manager) where T : class {
            throw new NotImplementedException();
        }
        public static string Contains(this IArgumentConstraintManager<string> manager, string value) {
            throw new NotImplementedException();
        }
        public static T Contains<T>(this IArgumentConstraintManager<T> manager, object value) where T : IEnumerable {
            throw new NotImplementedException();
        }
        public static string StartsWith(this IArgumentConstraintManager<string> manager, string value) {
            throw new NotImplementedException();
        }
        public static string EndsWith(this IArgumentConstraintManager<string> manager, string value) {
            throw new NotImplementedException();
        }
        public static string IsNullOrEmpty(this IArgumentConstraintManager<string> manager) {
            throw new NotImplementedException();
        }
        public static T IsGreaterThan<T>(this IArgumentConstraintManager<T> manager, T value) where T : IComparable {
            throw new NotImplementedException();
        }
        public static T IsSameSequenceAs<T>(this IArgumentConstraintManager<T> manager, IEnumerable value) where T : IEnumerable {
            throw new NotImplementedException();
        }
        public static T IsEmpty<T>(this IArgumentConstraintManager<T> manager) where T : IEnumerable {
            throw new NotImplementedException();
        }
        public static T IsEqualTo<T>(this IArgumentConstraintManager<T> manager, T value) {
            throw new NotImplementedException();
        }
        public static T IsSameAs<T>(this IArgumentConstraintManager<T> manager, T value) {
            throw new NotImplementedException();
        }
        public static T IsInstanceOf<T>(this IArgumentConstraintManager<T> manager, Type type) {
            throw new NotImplementedException();
        }
        public static T Matches<T>(this IArgumentConstraintManager<T> scope, Func<T, bool> predicate, string description) {
            throw new NotImplementedException();
        }
        public static T Matches<T>(this IArgumentConstraintManager<T> manager, Func<T, bool> predicate, string descriptionFormat, params object[] args) {
            throw new NotImplementedException();
        }
        public static T Matches<T>(this IArgumentConstraintManager<T> scope, Expression<Func<T, bool>> predicate) {
            throw new NotImplementedException();
        }
        public static T NullCheckedMatches<T>(this IArgumentConstraintManager<T> manager, Func<T, bool> predicate, Action<IOutputWriter> descriptionWriter) {
            throw new NotImplementedException();
        }
    }
}

namespace FakeItEasy.Creation {
    public interface IFakeOptions<T> { }

}
";

        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }

        private static TestCaseData AFakeOfT()
        {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using FakeItEasy;
            using FakeItEasy.Creation;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly System.IDisposable _fake = A.Fake<IDisposable>();
                }
            }"+ FakeItEasyDefintion;

            return new TestCaseData(code, 0)
                .SetName("A.Fake<IDisposable>()");
        }

        private static TestCaseData AStrictFakeOfT()
        {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using FakeItEasy;
            using FakeItEasy.Creation;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly System.IDisposable _fake = A.Fake<IDisposable>(o => o.Strict());
                }
            }" + FakeItEasyDefintion;
            
            return new TestCaseData(code, 0)
                .SetName("A.Fake<IDisposable>(o => o.Strict())");
        }

        private static TestCaseData ACollectionOfFake_of_T() {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using FakeItEasy;
            using FakeItEasy.Creation;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly IList<IDisposable> _fakes = A.CollectionOfFake<IDisposable>(2);
                }
            }" + FakeItEasyDefintion;

            return new TestCaseData(code, 0)
                .SetName("A.CollectionOfFake<IDisposable>(2)");
        }

        private static TestCaseData ACollectionOfFake_of_T_strict() {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using FakeItEasy;
            using FakeItEasy.Creation;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly IList<IDisposable> _fakes2 = A.CollectionOfFake<IDisposable>(2, o => o.Strict());
                }
            }" + FakeItEasyDefintion;

            return new TestCaseData(code, 0)
                .SetName("A.CollectionOfFake<IDisposable>(2, o => o.Strict())");
        }

        private static TestCaseData ADummy() {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using FakeItEasy;
            using FakeItEasy.Creation;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly IDisposable _dummy = A.Dummy<IDisposable>();
                }
            }" + FakeItEasyDefintion;

            return new TestCaseData(code, 0)
                .SetName("A.Dummy<IDisposable>()");
        }

        private static TestCaseData ACollectionOfDummy() {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using FakeItEasy;
            using FakeItEasy.Creation;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly IList<IDisposable> _dummies = A.CollectionOfDummy<IDisposable>(3);
        }
            }" + FakeItEasyDefintion;

            return new TestCaseData(code, 0)
                .SetName("A.CollectionOfDummy<IDisposable>(2)");
        }

        private static TestCaseData A_of_T_Ignored() {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using FakeItEasy;
            using FakeItEasy.Creation;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly IDisposable _dummies = A<IDisposable>.Ignored;
        }
            }" + FakeItEasyDefintion;

            return new TestCaseData(code, 0)
                .SetName("A<IDisposable>.Ignored");
        }

        private static TestCaseData A_of_T_Ignored2() {
            const string code = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using FakeItEasy;
            using FakeItEasy.Creation;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly IDisposable _dummies = A<IDisposable>._;
        }
            }" + FakeItEasyDefintion;
            
            return new TestCaseData(code, 0)
                .SetName("A<IDisposable>._");
        }

        private static TestCaseData A_IDisposable_That_Matches() {
            const string code = @"
            using System;
            using System.Collections;
            using System.Collections.Generic;
            using System.Linq.Expressions;
            using FakeItEasy.Creation;
            using FakeItEasy;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly IDisposable _dummies = A<IDisposable>.That.Not.Matches(f => true);
                }
            }" + FakeItEasyDefintion;

            return new TestCaseData(code, 0)
                .SetName("A<IDisposable>.That.Matches(f => true)");
        }

        private static TestCaseData A_IDisposable_That_Matches_with_Action_of_IOutputWriter() {
            const string code = @"
            using System;
            using System.Collections;
            using System.Collections.Generic;
            using System.Linq.Expressions;
            using FakeItEasy.Creation;
            using FakeItEasy;

            namespace SomeNamespace {
                public class SomeClass {
                    private readonly IDisposable _dummies = A<IDisposable>.That.Matches(f => true, w => w.Write(string.Empty));
                }
            }" + FakeItEasyDefintion;
            
            //A<IDisposable>.That.Matches(d => true, w => w.Write(""));
            //A<IDisposable>.That.IsNull();
            return new TestCaseData(code, 0)
                .SetName("A<IDisposable>.That.Matches(f => true, w => w.Write(string.Empty))");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using TestHelper;

namespace DisposableFixer.Test.IssueTests.Issues100
{
    internal class Issue154_IgnoreEnumerableExtensions : IssueSpec
    {
        private const string Code = @"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace SomeNamespace
{
    internal class TestClass
    {
        public void SomeMethod()
        {
            IEnumerable<IDisposable> Create()
            {
                yield return (IDisposable)null;
            }
            var items = Create()
                .First(x => true);
        }

    }
}

namespace System.Collections.Generic
{
    public interface IEnumerable<out T> : IEnumerable
    {
        IEnumerator<T> GetEnumerator();
    }
    public interface IEnumerator<out T> : IDisposable, IEnumerator
    {
        T Current { get; }
    }
}
namespace System.Collections
{
    public interface IEnumerator
    {
        bool MoveNext();

        object Current { get; }

        void Reset();
    }
}

namespace System.Linq
{
    public static class Enumerable
    {
        public static IEnumerable<int> Range(int start, int end) { throw new NotImplementedException(); }

        public static TResult Min<TResult>(this IEnumerable<TResult> source)
        {
            throw new NotImplementedException();
        }
        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            throw new NotImplementedException();
        }
        public static TResult Single<TResult>(this IEnumerable<TResult> source)
        {
            throw new NotImplementedException();
        }
        public static TResult Single<TResult>(this IEnumerable<TResult> source, Func<TResult, bool> predicate)
        {
            throw new NotImplementedException();
        }
        public static TResult SingleOrDefault<TResult>(this IEnumerable<TResult> source)
        {
            throw new NotImplementedException();
        }
        public static TResult SingleOrDefault<TResult>(this IEnumerable<TResult> source, Func<TResult, bool> predicate)
        {
            throw new NotImplementedException();
        }
        public static TResult FirstOrDefault<TResult>(this IEnumerable<TResult> source)
        {
            throw new NotImplementedException();
        }
        public static TResult FirstOrDefault<TResult>(this IEnumerable<TResult> source, Func<TResult, bool> predicate)
        {
            throw new NotImplementedException();
        }
        public static TResult Last<TResult>(this IEnumerable<TResult> source)
        {
            throw new NotImplementedException();
        }
        public static TResult Last<TResult>(this IEnumerable<TResult> source, Func<TResult, bool> predicate)
        {
            throw new NotImplementedException();
        }
        public static TResult LastOrDefault<TResult>(this IEnumerable<TResult> source)
        {
            throw new NotImplementedException();
        }
        public static TResult LastOrDefault<TResult>(this IEnumerable<TResult> source, Func<TResult, bool> predicate)
        {
            throw new NotImplementedException();
        }
        public static TResult Max<TResult>(this IEnumerable<TResult> source)
        {
            throw new NotImplementedException();
        }
        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            throw new NotImplementedException();
        }
        public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            throw new NotImplementedException();
        }
        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            throw new NotImplementedException();
        }
        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> unc, Func<TAccumulate, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }
        public static TResult ElementAt<TResult>(this IEnumerable<TResult> source, Int32 index)
        {
            throw new NotImplementedException();
        }
        public static TResult ElementAtOrDefault<TResult>(this IEnumerable<TResult> source, Int32 index)
        {
            throw new NotImplementedException();
        }
        public static TResult First<TResult>(this IEnumerable<TResult> source)
        {
            throw new NotImplementedException();
        }
        public static TResult First<TResult>(this IEnumerable<TResult> source, Func<TResult, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
";
        [Test, TestCaseSource(nameof(TestCasesForSystemLinqEnumerable))]
        public void Then_there_should_be_no_diagnostic(string code)
        {
            //PrintAllMethods();

            PrintCodeToAnalyze(code);
            var document = DiagnosticVerifier.CreateDocument(code);
            var errors = document.GetSemanticModelAsync().Result.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToArray();


            
            //var diagnostics = MyHelper.RunAnalyser(code, new DisposableFixerAnalyzer());
            //diagnostics.Should().HaveCount(0);
        }

        private static  IEnumerable<TestCaseData> TestCasesForSystemLinqEnumerable
        {
            get
            {
                yield return new TestCaseData(Code.Replace("###EXT###", "Min()")).SetName("Enumerable.Min(this) : TSource");
                yield return new TestCaseData(Code.Replace("###EXT###", "Min(ms => ms)")).SetName("Enumerable.Min(this, Func`2) : TSource");
                yield return new TestCaseData(Code.Replace("###EXT###", "First()")).SetName("Enumerable.First(this) : TSource");
            }
        }

        private static void PrintAllMethods()
        {
            var typesWithExtensionsMethods = typeof(Enumerable).Assembly.GetTypes()
                .Where(t => t.IsCSharpStatic())
                .Select(t => (Type: t,
                    Methods: t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(m => m.ReturnType.IsGenericParameter)))
                .Where(tpl => tpl.Methods.Any())
                .ToArray();
            foreach (var (type, methods) in typesWithExtensionsMethods.OrderBy(e => e.Type))
            {
                foreach (var method in methods)
                {
                    //Console.WriteLine(
                    //    $"{type.FullName}.{method.Name}({string.Join(", ", method.GetParameters().Select(pi => pi.ParameterType.Name))}) : {method.ReturnType.Name}");


                    

                    if (method.GetGenericArguments().Length == 1)
                    {
                        var parameter = string.Join(", ", method
                            .GetParameters()
                            .Select(p => Format(p)));
                        Console.WriteLine($"public static TResult {method.Name}<TResult>(this {parameter}){{ \r\n\tthrow new NotImplementedException(); \r\n}}");

                        string Format(ParameterInfo p)
                        {
                            return p.Name == "predicate" 
                                ? "Func<TResult, bool> predicate" : 
                                $"{p.ParameterType.Name.Replace("`1", "<TResult>")} {p.Name}";
                        }
                    }else if (method.GetGenericArguments().Length == 2)
                    {
                        var parameter = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name.Replace("`1", "<TSource>").Replace("`2", "<TSource,TResult>")} {p.Name}"));
                        Console.WriteLine($"public static TResult {method.Name}<TSource, TResult>(this {parameter}){{ \r\n\tthrow new NotImplementedException(); \r\n }}");
                    }
                    else
                    {
                        var parameter = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name.Replace("`1", "<TResult>")} {p.Name}"));
                        Console.WriteLine($"ERROR public static TResult {method.Name}<TResult>(this {parameter}){{ \r\n\tthrow new NotImplementedException(); \r\n }}");
                    }
                    
                }
            }
        }
    }
}
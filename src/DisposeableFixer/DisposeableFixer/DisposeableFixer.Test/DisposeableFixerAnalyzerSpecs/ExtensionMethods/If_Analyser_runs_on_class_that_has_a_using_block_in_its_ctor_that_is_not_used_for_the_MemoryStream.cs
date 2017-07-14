using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ExtensionMethods.Reactive.Bindings.Extensions;
using FluentAssertions;
using JetBrains.dotMemoryUnit.Util;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ExtensionMethods
{
    [TestFixture]
    internal class If_Analyser_runs_on_ObjectCreation_that_is_part_of_method_chain_with_tracking_extension_method :
        DisposeableFixerAnalyzerSpec
    {
        private Diagnostic[] _diagnostics;

        protected override void BecauseOf()
        {
            _diagnostics = MyHelper.RunAnalyser(Code, Sut);
        }

        private const string Code = @"
using System.IO;
uasing Reactive.Bindings.Extensions;
namespace DisFixerTest {
    internal class Usage : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>(); 
        public Usage()
        {
            new MemoryStream().AddTo(_disposables);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
namespace Reactive.Bindings.Extensions{
    internal static class IDisposableExtensions
    {
        public static T AddTo<T>(this T item, ICollection<T> disposables) where T : IDisposable
        {
            disposables.Add(item);

            return item;
        }
    }
}
";

        [Test]
        public void Then_there_should_be_one_Diagnostics()
        {
            _diagnostics.Length.Should().Be(1);
        }
    }

    namespace DisFixerTest {
        internal class Usage : IDisposable {
            private readonly List<IDisposable> _disposables = new List<IDisposable>();
            public Usage() {
                new MemoryStream().AddTo(_disposables);
            }

            public void Dispose() {
                foreach (var disposable in _disposables) {
                    disposable.Dispose();
                }
            }
        }
    }
    namespace Reactive.Bindings.Extensions {
        internal static class IDisposableExtensions {
            public static T AddTo<T>(this T item, ICollection<T> disposables) where T : IDisposable {
                disposables.Add(item);

                return item;
            }
        }
    }
}
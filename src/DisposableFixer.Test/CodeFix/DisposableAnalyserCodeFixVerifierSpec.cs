using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using TestHelper;

namespace DisposableFixer.Test.CodeFix
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public class DisposableAnalyserCodeFixVerifierSpec : CodeFixVerifier
    {
        private readonly List<Action> _disposeActions = new List<Action>();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DisposableFixerAnalyzer();
        }

        public DisposableAnalyserCodeFixVerifierSpec()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [DebuggerStepThrough]
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            EstablishContext();
            BecauseOf();
        }

        [DebuggerStepThrough]
        [OneTimeTearDown]
        public void TearDown()
        {
            foreach (var dispose_action in _disposeActions)
            {
                dispose_action();
            }
            Cleanup();
        }

        /// <summary>
        ///     Test setup. Place your initialization code here.
        /// </summary>
        [DebuggerStepThrough]
        protected virtual void EstablishContext()
        {
        }

        /// <summary>
        ///     Test run. Place the tested method / action here.
        /// </summary>
        [DebuggerStepThrough]
        protected virtual void BecauseOf()
        {
        }

        /// <summary>
        ///     Test clean. Close/delete files, close database connections ..
        /// </summary>
        [DebuggerStepThrough]
        protected virtual void Cleanup()
        {
        }

        /// <summary>
        ///     Creates an Action delegate.
        /// </summary>
        /// <param name="func">Method the shall be created as delegate.</param>
        /// <returns>A delegate of type <see cref="Action" /></returns>
        protected Action Invoking(Action func)
        {
            return func;
        }

        protected void DisposeOnTearDown(IDisposable disposable)
        {
            _disposeActions.Add(() => disposable?.Dispose());
        }

        protected void PrintCodeToFix(string code)
        {
            Console.WriteLine("Code to fix:");
            Console.WriteLine(code);
        }
        protected void PrintFixedCode(string code)
        {
            Console.WriteLine("Fixed code:");
            Console.WriteLine(code);
        }
    }
}
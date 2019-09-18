using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using NUnit.Framework;

namespace DisposableFixer.Test
{
    [DebuggerStepThrough, DebuggerNonUserCode]
    public class Spec
    {
        private readonly List<Action> _dispose_actions = new List<Action>();

        public Spec()
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
            foreach (var dispose_action in _dispose_actions)
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
            _dispose_actions.Add(() => disposable?.Dispose());
        }

        protected static void PrintCodeToAnalyze(string code)
        {
            Console.WriteLine("Code to analyze:");
            Console.WriteLine(code);
        }
    }
}
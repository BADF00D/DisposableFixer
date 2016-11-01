
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace DisposeableFixer.Test
{
    [TestFixture]
    internal abstract class Spec 
    {
        private readonly IList<Action> _tearDownActions = new List<Action>();

        [SetUp]
        private void StartUp()
        {
            Establish();
            BecauseOf();
        }

        protected virtual void Establish() { }

        protected virtual void BecauseOf() { }

        protected void OnTearDownDispose(IDisposable disposable)
        {
            _tearDownActions.Add(disposable.Dispose);
        }

        protected void OnTearDown(Action action) {
            _tearDownActions.Add(action);
        }

        [TearDown]
        private void InternalCleanup()
        {
            foreach (var tearDownAction in _tearDownActions)
            {
                tearDownAction();
            }
            CleanUp();
        } 

        
        public virtual void CleanUp() { }
    }
}
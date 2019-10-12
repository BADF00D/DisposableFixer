using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.ReactiveProperty
{
    internal class ReactivePropertyTests : DisposeableFixerAnalyzerSpec
    {
        [Test, TestCaseSource(nameof(TestCases))]
        public void Then_there_should_be_no_Diagnostics(string code)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(0);
        }

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return SetValidateNotifyErrorFuncIntToString();
                yield return SetValidateNotifyErrorFuncIntToIEnumerable();
                yield return SetValidateNotifyErrorFuncIntToTaskIEnumerable();
                yield return SetValidateNotifyErrorFuncIntToTaskString();
                yield return SetValidateNotifyErrorFuncObseravleIntToObservableIEnumerable();
                yield return SetValidateNotifyErrorFuncObseravleIntToObservableString();
            }
        }

        private static TestCaseData SetValidateNotifyErrorFuncIntToString()
        {
            const string sut = @"Func<int, string> validate = null;";
            var code = ReactivePropertyDefintion.Replace("###SUT###", sut);
            return new TestCaseData(code)
                .SetName("SetValidateNotifyError with Func int -> string");
        }

        private static TestCaseData SetValidateNotifyErrorFuncIntToTaskString()
        {
            const string sut = @"Func<int, Task<string>> validate = null;";
            var code = ReactivePropertyDefintion.Replace("###SUT###", sut);
            return new TestCaseData(code)
                .SetName("SetValidateNotifyError with Func int -> Task string");
        }

        private static TestCaseData SetValidateNotifyErrorFuncIntToTaskIEnumerable()
        {
            const string sut = @"Func<int, Task<IEnumerable>> validate = null;";
            var code = ReactivePropertyDefintion.Replace("###SUT###", sut);
            return new TestCaseData(code)
                .SetName("SetValidateNotifyError with Func int -> Task IEnumerable");
        }

        private static TestCaseData SetValidateNotifyErrorFuncIntToIEnumerable()
        {
            const string sut = @"Func<int, IEnumerable> validate = null;";
            var code = ReactivePropertyDefintion.Replace("###SUT###", sut);
            return new TestCaseData(code)
                .SetName("SetValidateNotifyError with Func int -> IEnumerable");
        }

        private static TestCaseData SetValidateNotifyErrorFuncObseravleIntToObservableString()
        {
            const string sut = @"Func<IObservable<int>, IObservable<string>> validate = null;";
            var code = ReactivePropertyDefintion.Replace("###SUT###", sut);
            return new TestCaseData(code)
                .SetName("SetValidateNotifyError with Func IObservable int -> IObservable string");
        }

        private static TestCaseData SetValidateNotifyErrorFuncObseravleIntToObservableIEnumerable()
        {
            const string sut = @"Func<IObservable<int>, IObservable<IEnumerable>> validate = null;";
            var code = ReactivePropertyDefintion.Replace("###SUT###", sut);
            return new TestCaseData(code)
                .SetName("SetValidateNotifyError with Func IObservable int -> Task IObservable IEnumerable");
        }

        private const string ReactivePropertyDefintion = @"

using System;
using System.Collections;
using System.Threading.Tasks;
using Reactive.Binding;

class Program
{
    internal class TestViewModel : IDisposable
    {
        ReactiveProperty<int> RP;

        public TestViewModel()
        {
            ###SUT###
            RP = new ReactiveProperty<int>().SetValidateNotifyError(validate)

            //Func<int, string> validate1 = null;
            //Func<int, Task<string>> validate2 = null;
            //Func<int, Task<IEnumerable>> validate3 = null;
            //Func<int, IEnumerable> validate4 = null;
            //Func<IObservable<int>, IObservable<string>> validate5 = null;
            //Func<IObservable<int>, IObservable<IEnumerable>> validate6 = null;

            RP = RP
                    .SetValidateNotifyError(###VALIDATE###)
                ;
        }



        public void Dispose()
        {
            RP?.Dispose();
        }
    }
    
}

namespace Reactive.Binding
{
    public class ReactiveProperty<T> : IDisposable
    {
        public void Dispose()
        {
        }

        public ReactiveProperty<T> SetValidateNotifyError(Func<int, string> validate) => throw new NotImplementedException();
        public ReactiveProperty<T> SetValidateNotifyError(Func<int, Task<string>> validate) => throw new NotImplementedException();
        public ReactiveProperty<T> SetValidateNotifyError(Func<int, Task<IEnumerable>> validate) => throw new NotImplementedException();
        public ReactiveProperty<T> SetValidateNotifyError(Func<int, IEnumerable> validate) => throw new NotImplementedException();
        public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<int>, IObservable<string>> validate) => throw new NotImplementedException();
        public ReactiveProperty<T> SetValidateNotifyError(Func<IObservable<int>, IObservable<IEnumerable>> validate) => throw new NotImplementedException();
    }

}

";

        

    }
}
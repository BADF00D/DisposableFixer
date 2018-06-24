using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.DisposeOutSideOfDisposeMethod.SpecialDisposeMethods
{
    internal partial class SystemIOPorsSerialPortTests : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return CloseSerialPortCreatedByObjectCreation();
                yield return CloseSerialPortOfFactoryMethod();
                yield return CloseSerialPortCreatedByObjectCreationWithConditionalAccess();
                yield return CloseSerialPortOfFactoryMethodWithConditionalAccess();

                yield return CloseSerialPortOfFieldByFactoryMethod();
                yield return CloseSerialPortOfFieldByObjectCreation();
                yield return CloseSerialPortOfFieldByFactoryMethodWithConditionalAccess();
                yield return CloseSerialPortOfFieldByObjectCreationWithConditionalAccess();

                yield return CloseSerialPortOfPropertyByObjectCreation();
                yield return CloseSerialPortOfProertyByFactoryMethod();
                yield return CloseSerialPortOfPropertyByObjectCreationWithConditionalAccess();
                yield return CloseSerialPortOfProertyByFactoryMethodWithConditionalAccess();
            }
        }

        [Test, TestCaseSource(nameof(TestCases))]
        public void The_number_of_Diagnostics_should_be_correct(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }
    }
}
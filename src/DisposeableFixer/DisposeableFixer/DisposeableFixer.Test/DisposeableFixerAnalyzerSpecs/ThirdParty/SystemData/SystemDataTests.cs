using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DisposableFixer.Test.DisposeableFixerAnalyzerSpecs.ThirdParty.SystemData
{
    internal class SystemDataTests : DisposeableFixerAnalyzerSpec
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return DataSet();
                yield return DataColumn();
                yield return DataView();
                yield return DataViewManager();
                yield return DataTable();
            }
        }

        private const string Mock = @"
using System.Data;

namespace bla
{
    internal class SomeClass
    {
        public SomeClass()
        {
            ###SUT###
        }
    }
}
";

        [Test, TestCaseSource(nameof(TestCases))]
        public void The_number_of_Diagnostics_should_be_correct(string code, int numberOfDiagnostics)
        {
            var diagnostics = MyHelper.RunAnalyser(code, Sut);
            diagnostics.Length.Should().Be(numberOfDiagnostics);
        }

        private static TestCaseData DataSet()
        {
            const string code = @"var ds = new DataSet();";

            return new TestCaseData(Mock.Replace("###SUT###", code), 0)
                .SetName("DataSet should be ignored");
        }
        private static TestCaseData DataTable() {
            const string code = @"var ds = new DataTable();";

            return new TestCaseData(Mock.Replace("###SUT###", code), 0)
                .SetName("DataTable should be ignored");
        }
        private static TestCaseData DataView() {
            const string code = @"var ds = new DataView();";

            return new TestCaseData(Mock.Replace("###SUT###", code), 1)
                .SetName("DataView should not be ignored");
        }

        private static TestCaseData DataColumn() {
            const string code = @"var ds = new DataColumn();";

            return new TestCaseData(Mock.Replace("###SUT###", code), 0)
                .SetName("DataColumn should be ignored");
        }
        private static TestCaseData DataViewManager() {
            const string code = @"var ds = new DataViewManager();";

            return new TestCaseData(Mock.Replace("###SUT###", code), 0)
                .SetName("DataViewManager should be ignored");
        }
    }
}
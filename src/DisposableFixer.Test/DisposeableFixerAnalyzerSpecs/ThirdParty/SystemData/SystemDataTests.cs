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
                yield return CreateSutAndClose("DataTableReader");
                yield return CreateSutAndClose("OdbcDataReader");
                yield return CreateSutAndClose("OleDbDataReader");
                yield return CreateSutAndClose("OracleDataReader");
                yield return CreateSutAndClose("SqlDataReader");
                yield return CreateSutAndClose("SqlDataReaderSmi");
            }
        }

        private const string Mock = @"
using System.Data;
using System.Data.SqlClient

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
namespace System.Data
{
    public class DataSet : IDisposable {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class DataTable : IDisposable {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class DataView : IDisposable {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class DataColumn : IDisposable {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    public class DataViewManager : IDisposable {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
namespace System.Data.SqlClient
{
    internal class SqlDataReader : IDisposable
    {
        public void Dispose(){}
        public void Close(){}
    }
    internal class DataTableReader : IDisposable
    {
        public void Dispose(){}
        public void Close(){}
    }
    internal class OdbcDataReader : IDisposable
    {
        public void Dispose() { }
        public void Close() { }
    }
    internal class OleDbDataReader : IDisposable
    {
        public void Dispose() { }
        public void Close() { }
    }
    internal class OracleDataReader : IDisposable
    {
        public void Dispose() { }
        public void Close() { }
    }
    internal class SqlDataReaderSmi : IDisposable
    {
        public void Dispose() { }
        public void Close() { }
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

        private static TestCaseData CreateSutAndClose(string type)
        {
            var code = $"var ds = new {type}();ds.Close()";

            return new TestCaseData(Mock.Replace("###SUT###", code), 0)
                .SetName($"{type} Close() should be ignored");
        }
    }
}
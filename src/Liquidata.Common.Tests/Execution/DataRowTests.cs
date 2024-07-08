using Liquidata.Common.Execution;

namespace Liquidata.Common.Tests.Execution
{
    public class DataRowTests
    {
        [Fact]
        public void GivenDataRow_WhenNoData_ThenHasNoData()
        {
            var record = new DataRecord();
            var hasData = record.HasData();

            Assert.False(hasData);
        }

        [Fact]
        public void GivenDataRow_WhenData_ThenHasData()
        {
            var record = new DataRecord();
            record.AddData("Test", "Test");

            var hasData = record.HasData();

            Assert.True(hasData);
        }

        [Fact]
        public void GivenDataRow_WhenAddData_ThenData()
        {
            var testValue = new KeyValuePair<string, string>("Test1", "Test2");
            var record = new DataRecord();
            record.AddData(testValue.Key, testValue.Value);

            Assert.Equal(testValue.Value, record.GetRowData(testValue.Key));
        }

        [Fact]
        public void AllColumns_ReturnsAllColumns()
        {
            // Arrange
            var dataRecord = new DataRecord();
            dataRecord.AddData("Column1", "Value1");
            dataRecord.AddData("Column2", "Value2");

            // Act
            var columns = dataRecord.AllColumns;

            // Assert
            Assert.Equal(new[] { "Column1", "Column2" }, columns);
        }

        [Fact]
        public void GivenDataRow_WhenNoData_ThenColumnIsNull()
        {
            var record = new DataRecord();
            var result = record.GetRowData("xyz");

            Assert.Null(result);
        }
    }
}

using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services;

namespace Liquidata.Common.Tests.Services
{
    public class DataHandlerServiceTests
    {
        [Fact]
        public void GivenDataHandler_WhenCloned_ThenScopePreserved()
        {
            var source = new DataHandlerService();
            source.DataScope = "XYZ";

            var cloned = source.Clone();
            Assert.Equal(source.DataScope, cloned.DataScope);
        }

        [Fact]
        public async Task GivenDataHandler_WhenMergeData_ThenDataMerged()
        {
            var column1Data = Guid.NewGuid().ToString();
            var column2Data = Guid.NewGuid().ToString();
            var column3Data = Guid.NewGuid().ToString();
            var column4Data = Guid.NewGuid().ToString();
            var screenshot1Data = new byte[] { 1, 2, 3 };
            var screenshot2Data = new byte[] { 1, 2, 3, 4 };

            var handler1 = new DataHandlerService();
            await handler1.AddDataAsync("Column1", column1Data);
            await handler1.AddRecordAsync();
            await handler1.AddDataAsync("Column2", column2Data);
            await handler1.AddScreenshotAsync("Test1", [1, 2, 3]);
            var handler1Data = handler1.GetExecutionResults();

            var handler2 = new DataHandlerService();
            await handler2.AddDataAsync("Column3", column3Data);
            await handler2.AddRecordAsync();
            await handler2.AddDataAsync("Column4", column4Data);
            await handler2.AddScreenshotAsync("Test2", [1, 2, 3, 4]);
            var handler2Data = handler2.GetExecutionResults();

            var mergedHandler = new DataHandlerService();
            await mergedHandler.MergeDataAsync(handler1);
            await mergedHandler.MergeDataAsync(handler2);
            var mergedData = mergedHandler.GetExecutionResults();

            Assert.Equal(["Column1", "Column2"], handler1Data.AllColumns);
            Assert.Equal(2, handler1Data.Records.Length);
            Assert.Single(handler1Data.Screenshots);
            Assert.Contains(handler1Data.Records, x => x.GetRowData("Column1") == column1Data);
            Assert.Contains(handler1Data.Records, x => x.GetRowData("Column2") == column2Data);
            Assert.Contains(handler1Data.Screenshots, x => x.Name.StartsWith("Test1") && x.Data.SequenceEqual(screenshot1Data));

            Assert.Equal(["Column3", "Column4"], handler2Data.AllColumns);
            Assert.Equal(2, handler2Data.Records.Length);
            Assert.Single(handler2Data.Screenshots);
            Assert.Contains(handler2Data.Records, x => x.GetRowData("Column3") == column3Data);
            Assert.Contains(handler2Data.Records, x => x.GetRowData("Column4") == column4Data);
            Assert.Contains(handler2Data.Screenshots, x => x.Name.StartsWith("Test2") && x.Data.SequenceEqual(screenshot2Data));

            Assert.Equal(["Column1", "Column2", "Column3", "Column4"], mergedData.AllColumns);
            Assert.Equal(4, mergedData.Records.Length);
            Assert.Equal(2, mergedData.Screenshots.Length); 
            Assert.Contains(mergedData.Records, x => x.GetRowData("Column1") == column1Data);
            Assert.Contains(mergedData.Records, x => x.GetRowData("Column2") == column2Data);
            Assert.Contains(mergedData.Records, x => x.GetRowData("Column3") == column3Data);
            Assert.Contains(mergedData.Records, x => x.GetRowData("Column4") == column4Data);
            Assert.Contains(mergedData.Screenshots, x => x.Name.StartsWith("Test1") && x.Data.SequenceEqual(screenshot1Data));            
            Assert.Contains(mergedData.Screenshots, x => x.Name.StartsWith("Test2") && x.Data.SequenceEqual(screenshot2Data));
        }

        [Fact]
        public async Task GivenDataHandler_WhenAddData_ThenDataAdded()
        {
            var handler = new DataHandlerService();
            var before = handler.GetExecutionResults();

            await handler.AddDataAsync("Field1", "Value1");
            var after = handler.GetExecutionResults();

            Assert.Empty(before.Records);
            Assert.Single(after.Records);
            Assert.True(after.Records[0].GetRowData("Field1") == "Value1");
        }

        [Fact]
        public async Task GivenDataHandler_WhenAddRecord_ThenRecordAdded()
        {
            var handler = new DataHandlerService();
            var before = handler.GetExecutionResults();

            await handler.AddRecordAsync();
            await handler.AddRecordAsync();
            await handler.AddDataAsync("Field1", "Value1");
            var after = handler.GetExecutionResults();

            Assert.Empty(before.Records);
            Assert.Single(after.Records);
            Assert.True(after.Records[0].GetRowData("Field1") == "Value1");
        }

        [Fact]
        public async Task GivenDataHandler_WhenAddScreenshot_ThenScreenshotAdded()
        {
            var handler = new DataHandlerService();
            var before = handler.GetExecutionResults();

            await handler.AddScreenshotAsync("Test1", [1, 2, 3]);
            await handler.AddScreenshotAsync("Test2", [1, 2, 3, 4]);
            await handler.AddScreenshotAsync("Test3", [1, 2, 3, 5]);
            var after = handler.GetExecutionResults();

            Assert.Empty(before.Screenshots);
            Assert.Equal(3, after.Screenshots.Length);
            Assert.Contains(after.Screenshots, x => x.Name.StartsWith("Test1"));
            Assert.Contains(after.Screenshots, x => x.Name.StartsWith("Test2"));
            Assert.Contains(after.Screenshots, x => x.Name.StartsWith("Test3"));
        }

        [Fact]
        public void GivenDataHandler_WhenNoRecords_ThenNoResults()
        {
            var handler = new DataHandlerService();
            var results = handler.GetExecutionResults();

            Assert.Empty(results.AllColumns);
            Assert.Empty(results.Records);
            Assert.Empty(results.Screenshots);
        }

        [Fact]
        public async Task GivenDataHandler_WhenRecords_ThenResults()
        {
            var handler = new DataHandlerService();
            await handler.AddDataAsync("Field1", "Value1");
            await handler.AddRecordAsync();
            await handler.AddDataAsync("Field2", "Value2");

            var results = handler.GetExecutionResults();
            Assert.Equal(2, results.Records.Length);
        }

        [Theory]
        [InlineData("This is some text", FieldType.Text, "This is some text")]
        [InlineData("Yes", FieldType.Boolean, "True")]
        [InlineData("No", FieldType.Boolean, "False")]
        [InlineData("5 more", FieldType.Numeric, "5")]
        public async Task GivenDataHandler_WhenCleanData_ThenDataCleaned(string data, FieldType fieldType, string expected)
        {
            var handler = new DataHandlerService();
            var result = await handler.CleanDataAsync(data, fieldType);

            Assert.Equal(expected, result);
        }
    }
}

using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

public class TableExportFormatHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
             new("csv", "CSV"),
             new("xlsx", "XLSX"),
             new("xls", "XLS")
        };
    }
}
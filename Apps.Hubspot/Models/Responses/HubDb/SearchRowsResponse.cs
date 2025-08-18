using Apps.Hubspot.Models.Dtos.HubDb;

namespace Apps.Hubspot.Models.Responses.HubDb;
public class SearchRowsResponse
{
    public IEnumerable<RowDto> Rows { get; set; }
}

using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos.HubDb;
using Apps.Hubspot.Models.Requests.HubDb;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Apps.Hubspot.Models.Responses.HubDb;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net.Mime;

namespace Apps.Hubspot.Actions;

[ActionList("HubDB")]
public class HubDbActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BasePageActions(invocationContext, fileManagementClient)

{
    [Action("Search tables", Description = "Gets HubDB tables that match the search criteria")]
    public async Task<List<TableDto>> SearchTables([ActionParameter] SearchTablesRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.Version))
            throw new PluginMisconfigurationException("Table version is required");

        var version = input.Version.Trim().ToLowerInvariant();

        var endpoints = version switch
        {
            "published" => new[] { "/hubdb/tables" },
            "draft" => new[] { "/hubdb/tables/draft" },
            "both" => new[] { "/hubdb/tables", "/hubdb/tables/draft" },
            _ => throw new PluginMisconfigurationException("Table version must be one of: draft, published, both")
        };

        var allTables = new List<TableDto>();

        foreach (var endpoint in endpoints)
        {
            var request = new HubspotRequest(endpoint, Method.Get, Creds);
            var response = await Client.ExecuteWithErrorHandling<GetAllResponse<TableDto>>(request);
            allTables.AddRange(response.Results);
        }

        var filtered = allTables.Where(t =>
            (string.IsNullOrWhiteSpace(input.NameContains) || t.Name?.Contains(input.NameContains, StringComparison.OrdinalIgnoreCase) == true) &&
            (string.IsNullOrWhiteSpace(input.LabelContains) || t.Label?.Contains(input.LabelContains, StringComparison.OrdinalIgnoreCase) == true)
        );

        return filtered.ToList();
    }

    [Action("Export table", Description = "Exports a HubDB table (draft or published) in the selected format")]
    public async Task<FileResponse> ExportTable([ActionParameter] TableVersionRequest version,
    [ActionParameter] TableExportRequest input)
    {
        if (version.Version is not "draft" and not "published")
        {
            throw new PluginMisconfigurationException("Table version must be either 'Draft' or 'Published'.");
        }

        if (String.IsNullOrEmpty(input.TableIdOrName))
        {
            throw new PluginMisconfigurationException("Table ID or name is required.");
        }

        if (input.ExportFormat.ToLower() is not "csv" and not "xlsx")
        {
            throw new PluginMisconfigurationException("Export format must be either 'CSV', 'XLS' or 'XLSX'.");
        }

        var endpoint = version.Version.ToLower() == "published"
            ? $"/hubdb/tables/{input.TableIdOrName}/export"
            : $"/hubdb/tables/{input.TableIdOrName}/draft/export";

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        request.AddQueryParameter("exportFormat", input.ExportFormat);
        request.AddHeader("accept", "application/vnd.ms-excel");
        var response = await Client.ExecuteWithErrorHandling(request);
        var fileName = $"hubdb_table_{input.TableIdOrName}_{version.Version}.{input.ExportFormat.ToLower()}";

        if (response.RawBytes == null || response.RawBytes.Length == 0)
        {
            throw new PluginApplicationException("Failed to download table export from HubSpot. Response was empty.");
        }

        var fileBytes = response.RawBytes;
        using var stream = new MemoryStream(fileBytes);

        var file = await FileManagementClient.UploadAsync(stream, response.ContentType,
           fileName);

        return new()
        {
            File = file
        };
    }

    [Action("Publish table", Description = "Publishes a HubDB table by copying draft data and schema to the published version")]
    public async Task<TableDto> PublishTable([ActionParameter] DraftTableRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.TableIdOrName))
            throw new PluginMisconfigurationException("Table ID or name is required");

        var endpoint = $"/cms/v3/hubdb/tables/{input.TableIdOrName}/draft/publish";
        var request = new HubspotRequest(endpoint, Method.Post, Creds);

        var response = await Client.ExecuteWithErrorHandling<TableDto>(request);

        return response;
    }

    [Action("Search rows", Description = "Gets HubDB table rows by table ID or name")]
    public async Task<SearchRowsResponse> SearchRows([ActionParameter] SearchRowsRequest input, 
        [ActionParameter] TableVersionRequest version)
    {
        if (string.IsNullOrWhiteSpace(input.TableIdOrName))
            throw new PluginMisconfigurationException("Table ID or name is required");

        if (string.IsNullOrWhiteSpace(version.Version))
            throw new PluginMisconfigurationException("Table version is required (draft or published)");

        string endpoint = version.Version switch
        {
            "published" => $"/cms/v3/hubdb/tables/{input.TableIdOrName}/rows",
            "draft" => $"/cms/v3/hubdb/tables/{input.TableIdOrName}/rows/draft",
            _ => throw new PluginMisconfigurationException("Table version must be either draft or published")
        };

        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        if (!string.IsNullOrWhiteSpace(input.FilterQuery))
            request.AddQueryParameter("filter", input.FilterQuery);

        var response = await Client.ExecuteWithErrorHandling<GetAllResponse<RowDto>>(request);

        var rows = response.Results;

        if (input.UpdatedAfter.HasValue)
        {
            rows = rows
                .Where(r => r.UpdatedAt.HasValue && r.UpdatedAt > input.UpdatedAfter.Value)
                .ToList();
        }

        if (input.UpdatedBefore.HasValue)
        {
            rows = rows
                .Where(r => r.UpdatedAt.HasValue && r.UpdatedAt < input.UpdatedBefore.Value)
                .ToList();
        }

        return new SearchRowsResponse { Rows = rows };
    }

    [Action("Update row column", Description = "Updates a column value in a HubDB row (draft version)")]
    public async Task<RowDto> UpdateRowColumn([ActionParameter] DraftTableRequest table,
        [ActionParameter] UpdateRowColumnRequest input)
    {
        if (string.IsNullOrWhiteSpace(table.TableIdOrName))
            throw new PluginMisconfigurationException("Table ID or name is required");

        if (string.IsNullOrWhiteSpace(input.RowId))
            throw new PluginMisconfigurationException("Row ID is required");

        if (string.IsNullOrWhiteSpace(input.ColumnName))
            throw new PluginMisconfigurationException("Column name/identifier is required");

        var endpoint = $"/cms/v3/hubdb/tables/{table.TableIdOrName}/rows/{input.RowId}/draft";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds);

        var columnValue = input.StringValue ??
                          (object)input.NumericValue ??
                          (input.DateValue.HasValue ? ToEpochMilliseconds(input.DateValue.Value) : null);

        if (columnValue == null)
            throw new PluginMisconfigurationException("At least one value (string, numeric, or date) must be provided");

        var body = new
        {
            values = new Dictionary<string, object>
        {
            { input.ColumnName, columnValue }
        }
        };

        request.AddJsonBody(body);

        var response = await Client.ExecuteWithErrorHandling<RowDto>(request);

        return response;
    }

    private long ToEpochMilliseconds(DateTime dt) =>
    new DateTimeOffset(dt).ToUnixTimeMilliseconds();

}
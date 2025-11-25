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
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IO;
using System.Net.Mime;
using System.Text;

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

        var endpoint = $"/hubdb/tables/{input.TableIdOrName}/draft/publish";
        var request = new HubspotRequest(endpoint, Method.Post, Creds);

        var response = await Client.ExecuteWithErrorHandling<TableDto>(request);

        return response;
    }

    [Action("Search rows", Description = "Gets HubDB table rows by table ID or name")]
    public async Task<SearchRowsResponse> SearchRows([ActionParameter] TableVersionRequest version,
        [ActionParameter] SearchRowsRequest input )
    {
        if (string.IsNullOrWhiteSpace(input.TableIdOrName))
            throw new PluginMisconfigurationException("Table ID or name is required");

        if (string.IsNullOrWhiteSpace(version.Version))
            throw new PluginMisconfigurationException("Table version is required (draft or published)");

        string endpoint = version.Version switch
        {
            "published" => $"/hubdb/tables/{input.TableIdOrName}/rows",
            "draft" => $"/hubdb/tables/{input.TableIdOrName}/rows/draft",
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

    [Action("Download rows content", Description = "Downloads specified columns and rows from a HubDb table into an HTML file")]
    public async Task<FileResponse> ExportRowsAsHtml(
    [ActionParameter] TableVersionRequest version,
    [ActionParameter] ExportRowsHtmlRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.TableIdOrName))
            throw new PluginMisconfigurationException("Table ID or name is required");

        if (string.IsNullOrWhiteSpace(version.Version))
            throw new PluginMisconfigurationException("Table version is required (draft or published)");

        string endpoint = version.Version switch
        {
            "published" => $"/hubdb/tables/{input.TableIdOrName}/rows",
            "draft" => $"/hubdb/tables/{input.TableIdOrName}/rows/draft",
            _ => throw new PluginMisconfigurationException("Table version must be either draft or published")
        };

        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        if (!string.IsNullOrWhiteSpace(input.FilterQuery))
            request.AddQueryParameter("filter", input.FilterQuery);

        var response = await Client.ExecuteWithErrorHandling<GetAllResponse<RowDto>>(request);

        var rows = response.Results;

        var colNames = input.Columns?.Where(c => !string.IsNullOrWhiteSpace(c)).ToList()
                       ?? new List<string>();

        if (colNames.Count == 0)
            throw new PluginMisconfigurationException("At least one column name must be provided");

        var sb = new StringBuilder();

        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine($"    <meta name=\"hubdb-table-id\" content=\"{input.TableIdOrName}\" />");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        foreach (var row in rows)
        {
            string rowId = row.Id.ToString();

            foreach (var col in colNames)
            {
                row.Values.TryGetValue(col, out var cell);
                string cellValue = cell?.ToString() ?? string.Empty;

                sb.AppendLine(
                    $"<div " +
                    $"data-column-name=\"{System.Net.WebUtility.HtmlEncode(col)}\" " +
                    $"data-row-id=\"{System.Net.WebUtility.HtmlEncode(rowId)}\">" +
                    $"{System.Net.WebUtility.HtmlEncode(cellValue)}" +
                    $"</div>"
                );
            }

        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        var fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
        await using var stream = new MemoryStream(fileBytes);

        var fileName = $"hubdb_rows_{input.TableIdOrName}_{version.Version}.html";

        var file = await FileManagementClient.UploadAsync(
            stream,
            "text/html",
            fileName
        );

        return new()
        {
            File = file
        };
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

        var endpoint = $"/hubdb/tables/{table.TableIdOrName}/rows/{input.RowId}/draft";
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

    [Action("Upload rows content",
    Description = "Create or update HubDB rows accordingly from an HTML file")]
    public async Task<UploadRowsResponse> UploadRowsFromHtml(
    [ActionParameter] UploadRowsContentRequest input)
    {
        if (input.File == null)
            throw new PluginMisconfigurationException("Input file is required.");

        var stream = await FileManagementClient.DownloadAsync(input.File);
        string html;
        using (var sr = new StreamReader(stream))
            html = sr.ReadToEnd();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var tableIdMeta = doc.DocumentNode
            .SelectSingleNode("//meta[@name='hubdb-table-id']");

        if (tableIdMeta == null)
            throw new PluginMisconfigurationException("Table ID metadata not found in HTML <head>.");

        string tableId = tableIdMeta.GetAttributeValue("content", null);
        if (string.IsNullOrEmpty(tableId))
            throw new PluginMisconfigurationException("Table ID is missing in the HTML metadata.");

        var divs = doc.DocumentNode.SelectNodes("//div[@data-row-id]");
        if (divs == null || divs.Count == 0)
            throw new PluginApplicationException("No translatable blocks (<div data-row-id>) found in HTML.");

        var grouped = divs
            .GroupBy(d => d.GetAttributeValue("data-row-id", ""))
            .ToDictionary(g => g.Key, g => g.ToList());

        var results = new List<RowUpdateResult>();

        foreach (var (rowId, cells) in grouped)
        {
            if (string.IsNullOrWhiteSpace(rowId))
                continue;

            if (input.Behavior == "update")
            {
                var updateResult = await UpdateExistingRow(tableId, rowId, cells);
                results.Add(updateResult);
            }
            else if (input.Behavior == "create")
            {
                var createResult = await CreateClonedRow(tableId, rowId, cells);
                results.Add(createResult);
            }
        }

        return new UploadRowsResponse
        {
            Results = results
        };
    }

    private long ToEpochMilliseconds(DateTime dt) =>
    new DateTimeOffset(dt).ToUnixTimeMilliseconds();

    private async Task<RowUpdateResult> UpdateExistingRow(
    string tableId,
    string rowId,
    List<HtmlNode> cells)
    {
        var endpoint = $"/cms/v3/hubdb/tables/{tableId}/rows/{rowId}/draft";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds);

        var values = new Dictionary<string, object>();

        foreach (var cell in cells)
        {
            string colName = cell.GetAttributeValue("data-column-name", "");
            string text = cell.InnerText?.Trim() ?? "";

            values[colName] = text;
        }

        var body = new { values };
        request.AddJsonBody(body);

        var updated = await Client.ExecuteWithErrorHandling<RowDto>(request);

        return new RowUpdateResult
        {
            OriginalRowId = rowId,
            NewRowId = rowId,
            Action = "updated"
        };
    }

    private async Task<RowUpdateResult> CreateClonedRow(
    string tableId,
    string sourceRowId,
    List<HtmlNode> cells)
    {
        var cloneReq = new HubspotRequest(
            $"/hubdb/tables/{tableId}/rows/{sourceRowId}/draft/clone",
            Method.Post,
            Creds);

        var cloned = await Client.ExecuteWithErrorHandling<RowDto>(cloneReq);
        var newRowId = cloned.Id.ToString();

        var updatedValues = new Dictionary<string, object>();

        foreach (var cell in cells)
        {
            string colName = cell.GetAttributeValue("data-column-name", "");
            string newValue = cell.InnerText?.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(colName))
                updatedValues[colName] = newValue;
        }

        var updateReq = new HubspotRequest(
            $"/hubdb/tables/{tableId}/rows/{newRowId}/draft",
            Method.Patch,
            Creds);

        updateReq.AddJsonBody(new { values = updatedValues });

        await Client.ExecuteWithErrorHandling<RowDto>(updateReq);

        return new RowUpdateResult
        {
            OriginalRowId = sourceRowId,
            NewRowId = newRowId,
            Action = "created"
        };
    }
}
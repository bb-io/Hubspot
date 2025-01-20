﻿using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Forms;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses.Content;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices;

public class MarketingFormService(InvocationContext invocationContext) : HubSpotInvocable(invocationContext), IContentService
{
    public async Task<List<Metadata>> SearchContentAsync(TimeFilterRequest filterRequest)
    {
        var query = filterRequest.AsQuery();
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}".WithQuery(query);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var response = await Client.Paginate<MarketingFormDto>(request);

        return response.Select(x => new Metadata
        {
            Id = x.Id,
            Language = x.Configuration?.Language ?? string.Empty,
            Type = ContentTypes.Form
        }).ToList();
    }
}
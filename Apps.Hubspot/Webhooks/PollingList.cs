using Apps.Hubspot.Actions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Hubspot.Webhooks;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : HubSpotInvocable(invocationContext)
{
    [PollingEvent("On landing page created or updated", Description = "Triggered when a landing page is created or updated")]
    public async Task<PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>> OnLandingPageCreatedOrUpdated(PollingEventRequest<LandingPageCreatedOrUpdated> request)
    {
        var landingPageActions = new LandingPageActions(InvocationContext, null);
        
        if (request.Memory == null)
        {
            var landingPages = await landingPageActions.GetAllLandingPages(new SearchPagesRequest());
            var orderedLandingPages = landingPages.Items.OrderBy(x => x.Id).ToList();
            var lastLandingPage = orderedLandingPages.Last();
            
            return new PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>()
            {
                FlyBird = false,
                Memory = new LandingPageCreatedOrUpdated() { LastPageId = lastLandingPage.Id},
                Result = lastLandingPage
            };
        }
        
        var updatedLandingPages = await landingPageActions.GetAllLandingPages(new SearchPagesRequest());
        if (!updatedLandingPages.Items.Any())
        {
            return new PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }
        
        var orderedUpdatedLandingPages = updatedLandingPages.Items.OrderBy(x => x.Id).ToList();
        var lastUpdatedLandingPage = orderedUpdatedLandingPages.Last();
        if (lastUpdatedLandingPage.Id == request.Memory.LastPageId)
        {
            return new PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }
        
        return new PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>()
        {
            FlyBird = true,
            Memory = new LandingPageCreatedOrUpdated() { LastPageId = lastUpdatedLandingPage.Id },
            Result = lastUpdatedLandingPage
        };
    }
}
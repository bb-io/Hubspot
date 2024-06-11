using Apps.Hubspot.Actions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Microsoft.Extensions.Logging;

namespace Apps.Hubspot.Webhooks;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : HubSpotInvocable(invocationContext)
{
    [PollingEvent("On landing page created or updated", Description = "Triggered when a landing page is created or updated")]
    public async Task<PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>> OnLandingPageCreatedOrUpdated(PollingEventRequest<LandingPageCreatedOrUpdated> request)
    {
        try
        {
            var landingPageActions = new LandingPageActions(InvocationContext, null);

            await Logger.LogAsync(new
            {
                Request = request
            });
        
            if (request.Memory == null)
            {
                var landingPages = await landingPageActions.GetAllLandingPages(new SearchPagesRequest());
                var orderedLandingPages = landingPages.Items.OrderBy(x => x.Id).ToList();
                var lastLandingPage = orderedLandingPages.Last();
                
                await Logger.LogAsync(new
                {
                    OrderedLangingPages = orderedLandingPages,
                    Message = "Last landing page if memory is null",
                });
            
                return new PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>()
                {
                    FlyBird = true,
                    Memory = new LandingPageCreatedOrUpdated() { LastPageId = lastLandingPage.Id},
                    Result = lastLandingPage
                };
            }
        
            var updatedLandingPages = await landingPageActions.GetAllLandingPages(new SearchPagesRequest());
            
            await Logger.LogAsync(new
            {
                UpdatedLandingPages = updatedLandingPages,
                Message = "Updated landing pages",
            });
            
            if (!updatedLandingPages.Items.Any())
            {
                
                await Logger.LogAsync(new
                {
                    Message = "No updated landing pages",
                });
                
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
                await Logger.LogAsync(new
                {
                    Message = "No new landing pages",
                });
                
                return new PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>()
                {
                    FlyBird = false,
                    Memory = request.Memory,
                    Result = null
                };
            }
            
            await Logger.LogAsync(new
            {
                LastUpdatedLandingPage = lastUpdatedLandingPage,
                Message = "Last updated landing page",
            });
        
            return new PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>()
            {
                FlyBird = true,
                Memory = new LandingPageCreatedOrUpdated() { LastPageId = lastUpdatedLandingPage.Id },
                Result = lastUpdatedLandingPage
            };
        }
        catch (Exception e)
        {
            await Logger.LogAsync(e);
            
            return new PollingEventResponse<LandingPageCreatedOrUpdated, PageDto>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }
    }
}
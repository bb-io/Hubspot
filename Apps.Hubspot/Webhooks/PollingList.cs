using Apps.Hubspot.Actions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses.Pages;
using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Hubspot.Webhooks;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : HubSpotInvocable(invocationContext)
{
    [PollingEvent("On landing pages created or updated",
        Description = "Triggered when a landing page is created or updated")]
    public async Task<PollingEventResponse<LandingPageCreatedOrUpdatedMemory, LandingPagesResponse>> OnLandingPageCreatedOrUpdated(PollingEventRequest<LandingPageCreatedOrUpdatedMemory> request)
    {
        try
        {
            var landingPageActions = new LandingPageActions(InvocationContext, null);
            var landingPages = await landingPageActions.GetAllLandingPages(new SearchPagesRequest());
            var pages = landingPages.Items.ToList();

            await Logger.LogAsync(new { Request = request });

            if (request.Memory == null)
            {
                return await HandleFirstRunAsync(pages);
            }

            return await HandleSubsequentRunsAsync(request, pages);
        }
        catch (Exception e)
        {
            await Logger.LogAsync(e);

            return new PollingEventResponse<LandingPageCreatedOrUpdatedMemory, LandingPagesResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }
    }

    private async Task<PollingEventResponse<LandingPageCreatedOrUpdatedMemory, LandingPagesResponse>>HandleFirstRunAsync(List<PageDto> pages)
    {
        await Logger.LogAsync(new
        {
            OrderedLandingPages = pages,
            Message = "Last landing page if memory is null",
        });

        return new PollingEventResponse<LandingPageCreatedOrUpdatedMemory, LandingPagesResponse>()
        {
            FlyBird = false,
            Memory = new LandingPageCreatedOrUpdatedMemory() { Pages = pages },
            Result = null
        };
    }

    private async Task<PollingEventResponse<LandingPageCreatedOrUpdatedMemory, LandingPagesResponse>> HandleSubsequentRunsAsync(PollingEventRequest<LandingPageCreatedOrUpdatedMemory> request,
            List<PageDto> pages)
    {
        await Logger.LogAsync(new
        {
            UpdatedLandingPages = pages,
            Message = "Updated landing pages",
        });

        if (pages.Count == 0)
        {
            await Logger.LogAsync(new
            {
                Message = "No updated landing pages",
            });

            return new PollingEventResponse<LandingPageCreatedOrUpdatedMemory, LandingPagesResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }

        var updatedPages = pages.Where(p => request.Memory!.Pages.All(mp => mp.Id != p.Id)).ToList();
        await Logger.LogAsync(new
        {
            UpdatedPages = updatedPages,
            Message = "Updated pages",
        });

        if (updatedPages.Count == 0)
        {
            await Logger.LogAsync(new
            {
                Message = "No updated pages",
            });

            return new PollingEventResponse<LandingPageCreatedOrUpdatedMemory, LandingPagesResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }

        await Logger.LogAsync(new
        {
            UpdatedPages = updatedPages,
            Message = "Updated pages",
        });

        return new PollingEventResponse<LandingPageCreatedOrUpdatedMemory, LandingPagesResponse>()
        {
            FlyBird = true,
            Memory = new LandingPageCreatedOrUpdatedMemory() { Pages = pages },
            Result = new LandingPagesResponse() { Pages = updatedPages }
        };
    }
}
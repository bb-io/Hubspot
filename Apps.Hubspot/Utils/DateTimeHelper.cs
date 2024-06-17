using System.Globalization;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Webhooks.Models;

namespace Apps.Hubspot.Utils;

public static class DateTimeHelper
{
    public static bool IsPageUpdated(List<PageEntity> memoryPages, BlogPostDto blogPost)
    {
        var memoryPage = memoryPages.FirstOrDefault(mp => mp.Id == blogPost.Id);
        if (memoryPage != null)
        {
            string format = "yyyy-MM-ddTHH:mm:ss.fffZ";
            bool updatedParsed = DateTime.TryParseExact(blogPost.Updated, format, null,
                System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                out var updatedDateTime);
            bool memoryUpdatedParsed = DateTime.TryParse(memoryPage.Updated, out var memoryUpdatedDateTime);

            if (updatedParsed && memoryUpdatedParsed)
            {
                return updatedDateTime.ToString(CultureInfo.InvariantCulture) !=
                       memoryUpdatedDateTime.ToString(CultureInfo.InvariantCulture);
            }
        }

        return false;
    }
}
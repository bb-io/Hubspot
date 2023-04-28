using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Webhooks.Handlers
{
    public class CompanyCreationHandler : BaseWebhookHandler
    {
        const string SubscriptionEvent = "company.creation";

        public CompanyCreationHandler() : base(SubscriptionEvent) { }
    }
}

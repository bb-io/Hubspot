using Apps.Hubspot.DynamicHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
namespace Apps.Hubspot.Models.Requests
{
	public class PublishLandingPageRequest
	{
        [DataSource(typeof(LandingPageHandler))]
        public string Id { get; set; }
		public Nullable<DateTime> DateTime { get; set; }
	}
}


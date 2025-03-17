using Apps.Hubspot.Models.Responses.Pages;
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Entities;

public record JsonResultEntity(PageInfoResponse PageInfo, JObject Json);
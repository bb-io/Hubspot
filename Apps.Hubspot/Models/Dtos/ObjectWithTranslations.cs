using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Dtos;

public class ObjectWithTranslations
{
    [JsonProperty("language")]
    public string Language { get; set; } = string.Empty;
        
    [JsonProperty("translatedFromId")]
    public string? TranslatedFromId { get; set; }
    
    [JsonProperty("translations")]
    public Dictionary<string, ObjectWithId>? Translations { get; set; } 
}
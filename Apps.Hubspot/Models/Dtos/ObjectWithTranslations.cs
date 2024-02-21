namespace Apps.Hubspot.Models.Dtos
{
    public class ObjectWithTranslations
    {
        public string Language { get; set; }
        public Dictionary<string, ObjectWithId> Translations { get; set; }
    }
}

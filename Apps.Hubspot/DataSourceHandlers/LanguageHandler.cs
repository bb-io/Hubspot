using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.DataSourceHandlers
{
    public class LanguageHandler : IStaticDataSourceItemHandler
    {
        private static Dictionary<string, string> EnumValues => new()
        {
            { "af", "Afrikaans" },
            { "ar", "Arabic" },
            { "as", "Assamese" },
            { "be", "Belarusian" },
            { "bg", "Bulgarian" },
            { "bn", "Bangla" },
            { "bs", "Bosnian" },
            { "ca", "Catalan" },
            { "cs", "Czech" },
            { "da", "Danish" },
            { "de", "German" },
            { "el", "Greek" },
            { "en", "English" },
            { "es", "Spanish" },
            { "et", "Estonian" },
            { "eu", "Basque" },
            { "fa", "Persian" },
            { "fi", "Finnish" },
            { "fo", "Faroese" },
            { "fr", "French" },
            { "gl", "Galician" },
            { "gu", "Gujarati" },
            { "he", "Hebrew" },
            { "hi", "Hindi" },
            { "hr", "Croatian" },
            { "hu", "Hungarian" },
            { "hy", "Armenian" },
            { "id", "Indonesian" },
            { "is", "Icelandic" },
            { "it", "Italian" },
            { "ja", "Japanese" },
            { "ka", "Georgian" },
            { "kk", "Kazakh" },
            { "kn", "Kannada" },
            { "ko", "Korean" },
            { "ku", "Kurdish" },
            { "ky", "Kyrgyz" },
            { "lo", "Lao" },
            { "lt", "Lithuanian" },
            { "lv", "Latvian" },
            { "mk", "Macedonian" },
            { "mn", "Mongolian" },
            { "mr", "Marathi" },
            { "ms", "Malay" },
            { "my", "Burmese" },
            { "nb", "Norwegian Bokmål" },
            { "ne", "Nepali" },
            { "nl", "Dutch" },
            { "no", "Norwegian" },
            { "pa", "Punjabi" },
            { "pl", "Polish" },
            { "pt", "Portuguese" },
            { "ro", "Romanian" },
            { "ru", "Russian" },
            { "si", "Sinhala" },
            { "sk", "Slovak" },
            { "sl", "Slovenian" },
            { "sq", "Albanian" },
            { "sr", "Serbian" },
            { "sv", "Swedish" },
            { "sw", "Swahili" },
            { "ta", "Tamil" },
            { "te", "Telugu" },
            { "th", "Thai" },
            { "tl", "Tagalog" },
            { "tr", "Turkish" },
            { "uk", "Ukrainian" },
            { "ur", "Urdu" },
            { "uz", "Uzbek" },
            { "vi", "Vietnamese" },
            { "zh", "Chinese" },
        };


        IEnumerable<DataSourceItem> IStaticDataSourceItemHandler.GetData()
        {
            return EnumValues.Select(e => new DataSourceItem
            {
                Value = e.Key,
                DisplayName = e.Value
            });
        }
    }
}

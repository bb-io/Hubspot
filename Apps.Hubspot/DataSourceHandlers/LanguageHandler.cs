﻿using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.DataSourceHandlers
{
    public class LanguageHandler : IStaticDataSourceItemHandler
    {
        private static Dictionary<string, string> EnumValues => new()
        {
            { "af", "Afrikaans" },
            { "sq", "Albanian" },
            { "sq-al", "Albanian - Albania" },
            { "ar", "Arabic" },
            { "ar-dz", "Arabic - Algeria" },
            { "ar-bh", "Arabic - Bahrain" },
            { "ar-eg", "Arabic - Egypt" },
            { "ar-iq", "Arabic - Iraq" },
            { "ar-jo", "Arabic - Jordan" },
            { "ar-kw", "Arabic - Kuwait" },
            { "ar-lb", "Arabic - Lebanon" },
            { "ar-ly", "Arabic - Libya" },
            { "ar-ma", "Arabic - Morocco" },
            { "ar-om", "Arabic - Oman" },
            { "ar-qa", "Arabic - Qatar" },
            { "ar-sa", "Arabic - Saudi Arabia" },
            { "ar-sd", "Arabic - Sudan" },
            { "ar-sy", "Arabic - Syria" },
            { "ar-tn", "Arabic - Tunisia" },
            { "ar-ae", "Arabic - U.A.E." },
            { "ar-ye", "Arabic - Yemen" },
            { "hy", "Armenian" },
            { "as", "Assamese" },
            { "eu", "Basque" },
            { "be", "Belarusian" },
            { "be-by", "Belarusian - Belarus" },
            { "bn", "Bengali; Bangla" },
            { "bg", "Bulgarian" },
            { "bg-bg", "Bulgarian - Bulgaria" },
            { "my", "Burmese" },
            { "my-mm", "Burmese - Myanmar" },
            { "ca", "Catalan" },
            { "ca-es", "Catalan - Catalonia" },
            { "zh", "Chinese" },
            { "zh-hk", "Chinese - Hong Kong S.A.R." },
            { "zh-mo", "Chinese - Macao S.A.R." },
            { "zh-cn", "Chinese - PRC (Simplified)" },
            { "zh-sg", "Chinese - Singapore" },
            { "zh-tw", "Chinese - Taiwan (Traditional)" },
            { "zh-hans", "Chinese (Simplified Han)" },
            { "zh-hant", "Chinese (Traditional Han)" },
            { "hr", "Croatian" },
            { "hr-hr", "Croatian - Croatia" },
            { "cs", "Czech" },
            { "cs-cz", "Czech - Czechia" },
            { "da", "Danish" },
            { "da-dk", "Danish - Denmark" },
            { "dv", "Dhivehi; Maldivian" },
            { "nl", "Dutch" },
            { "nl-be", "Dutch - Belgium" },
            { "nl-lu", "Dutch - Luzembourg" },
            { "nl-nl", "Dutch - Netherlands" },
            { "nl-ch", "Dutch - Switzerland" },
            { "en", "English" },
            { "en-au", "English - Australia" },
            { "en-be", "English - Belize" },
            { "en-ca", "English - Canada" },
            { "en-ie", "English - Ireland" },
            { "en-nz", "English - New Zealand" },
            { "en-ph", "English - Philippines" },
            { "en-za", "English - South Africa" },
            { "en-ch", "English - Switzerland" },
            { "en-gb", "English - United Kingdom" },
            { "en-us", "English - United States" },
            { "en-zw", "English - Zimbabwe" },
            { "et", "Estonian" },
            { "et-ee", "Estonian - Estonia" },
            { "fo", "Faroese" },
            { "fa", "Farsi; Persian" },
            { "fi", "Finnish" },
            { "fi-fi", "Finnish - Finland" },
            { "fr", "French" },
            { "fr-be", "French - Belgium" },
            { "fr-ca", "French - Canada" },
            { "fr-lu", "French - Luxembourg" },
            { "fr-mc", "French - Monaco" },
            { "fr-ch", "French - Switzerland" },
            { "fr-tn", "French - Tunisia" },
            { "gl", "Galician" },
            { "ka", "Georgian" },
            { "de", "German" },
            { "de-at", "German - Austria" },
            { "de-de", "German - Germany" },
            { "de-li", "German - Liechtenstein" },
            { "de-lu", "German - Luxembourg" },
            { "de-ch", "German - Switzerland" },
            { "el", "Greek" },
            { "el-cy", "Greek - Cyprus" },
            { "el-gr", "Greek - Greece" },
            { "gu", "Gujarati" },
            { "he", "Hebrew" },
            { "hi", "Hindi" },
            { "hi-in", "Hindi - India" },
            { "hu", "Hungarian" },
            { "hu-hu", "Hungarian - Hungary" },
            { "is", "Icelandic" },
            { "is-is", "Icelandic - Iceland" },
            { "id", "Indonesian" },
            { "it", "Italian" },
            { "it-it", "Italian - Italy" },
            { "it-ch", "Italian - Switzerland" },
            { "ja", "Japanese" },
            { "ja-jp", "Japanese - Japan" },
            { "kn", "Kannada" },
            { "kk", "Kazakh" },
            { "ky", "Kirghiz; Kyrgyz" },
            { "ko", "Korean" },
            { "ko-kr", "Korean - South Korea" },
            { "lv", "Latvian" },
            { "lv-lv", "Latvian - Latvia" },
            { "lt", "Lithuanian" },
            { "lt-lv", "Lithuanian - Latvia" },
            { "lt-lt", "Lithuanian - Lithuania" },
            { "mk", "Macedonian" },
            { "mk-mk", "Macedonian - Macedonia" },
            { "ms", "Malay" },
            { "ms-bn", "Malay - Brunei" },
            { "ms-my", "Malay - Malaysia" },
            { "mr", "Marathi" },
            { "mn", "Mongolian" },
            { "ne", "Nepali" },
            { "no", "Norwegian" },
            { "no-no", "Norwegian - Norway" },
            { "nb", "Norwegian Bokm?l" },
            { "pl", "Polish" },
            { "pl-pl", "Polish - Poland" },
            { "pt", "Portuguese" },
            { "pt-br", "Portuguese - Brazil" },
            { "pt-pt", "Portuguese - Portugal" },
            { "ro", "Romanian" },
            { "ro-ro", "Romanian - Romania" },
            { "ru", "Russian" },
            { "ru-ru", "Russian - Russia" },
            { "sr", "Serbian" },
            { "sr-ba", "Serbian - Bosnia and Herzegovina" },
            { "sr-me", "Serbian - Montenegro" },
            { "sr-rs", "Serbian - Serbia" },
            { "si", "Sinhala; Sinhalese" },
            { "sk", "Slovak" },
            { "sk-sk", "Slovak - Slovakia" },
            { "sl", "Slovenian" },
            { "sl-si", "Slovenian - Slovenia" },
            { "es", "Spanish" },
            { "es-ar", "Spanish - Argentina" },
            { "es-bo", "Spanish - Bolivia" },
            { "es-cl", "Spanish - Chile" },
            { "es-co", "Spanish - Colombia" },
            { "es-cr", "Spanish - Costa Rica" },
            { "es-cu", "Spanish - Cuba" },
            { "es-do", "Spanish - Dominican Republic" },
            { "es-ec", "Spanish - Ecuador" },
            { "es-sv", "Spanish - El Salvador" },
            { "es-gt", "Spanish - Guatemala" },
            { "es-hn", "Spanish - Honduras" },
            { "es-mx", "Spanish - Mexico" },
            { "es-ni", "Spanish - Nicaragua" },
            { "es-pa", "Spanish - Panama" },
            { "es-py", "Spanish - Paraguay" },
            { "es-pe", "Spanish - Peru" },
            { "es-pr", "Spanish - Puerto Rico" },
            { "es-es", "Spanish - Spain" },
            { "es-us", "Spanish - United States" },
            { "es-uy", "Spanish - Uruguay" },
            { "es-ve", "Spanish - Venezuela" },
            { "sw", "Swahili" },
            { "sv", "Swedish" },
            { "sv-fi", "Swedish - Finland" },
            { "sv-se", "Swedish - Sweden" },
            { "ta", "Tamil" },
            { "te", "Telugu" },
            { "th", "Thai" },
            { "th-th", "Thai - Thailand" },
            { "tr", "Turkish" },
            { "tr-tr", "Turkish - Turkey" },
            { "uk", "Ukrainian" },
            { "uk-ua", "Ukrainian - Ukraine" },
            { "ur", "Urdu" },
            { "uz", "Uzbek (Latin)" },
            { "vi", "Vietnamese" },
            { "vi-vn", "Vietnamese - Vietnam" },
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

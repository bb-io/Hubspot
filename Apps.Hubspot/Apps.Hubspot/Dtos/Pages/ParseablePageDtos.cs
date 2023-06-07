//using System;
//namespace Apps.Hubspot.Dtos.Pages
//{
//    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
//    public class AdditionalProp
//    {
//        public int X { get; set; }
//        public int W { get; set; }
//        public string Name { get; set; }
//        public string Label { get; set; }
//        public string Type { get; set; }
//        public Params Params { get; set; }
//        public List<Row> Rows { get; set; }
//        public List<RowMetaDatum> RowMetaData { get; set; }
//        public List<object> Cells { get; set; }
//        public string CssClass { get; set; }
//        public string CssStyle { get; set; }
//        public string CssId { get; set; }
//        public Styles Styles { get; set; }
//        public int Id { get; set; }
//        public string Slug { get; set; }
//        public string State { get; set; }
//        public string AuthorName { get; set; }
//        public string Password { get; set; }
//        public bool PublicAccessRulesEnabled { get; set; }
//        public List<PublicAccessRule> PublicAccessRules { get; set; }
//        public string Campaign { get; set; }
//        public List<int> TagIds { get; set; }
//        public bool ArchivedInDashboard { get; set; }
//        public DateTime Created { get; set; }
//        public DateTime Updated { get; set; }
//        public DateTime PublishDate { get; set; }
//    }

//    public class Angle
//    {
//        public int Value { get; set; }
//        public string Units { get; set; }
//    }

//    public class BackgroundColor
//    {
//        public int R { get; set; }
//        public int G { get; set; }
//        public int B { get; set; }
//        public int A { get; set; }
//    }

//    public class BackgroundGradient
//    {
//        public SideOrCorner SideOrCorner { get; set; }
//        public Angle Angle { get; set; }
//        public List<Color> Colors { get; set; }
//    }

//    public class BackgroundImage
//    {
//        public string ImageUrl { get; set; }
//        public string BackgroundSize { get; set; }
//        public string BackgroundPosition { get; set; }
//    }

//    public class Color
//    {
//        public Color color { get; set; }
//    }

//    public class Color10
//    {
//        public int R { get; set; }
//        public int G { get; set; }
//        public int B { get; set; }
//        public int A { get; set; }
//    }

//    public class LayoutSections
//    {
//        public AdditionalProp1 AdditionalProp1 { get; set; }
//        public AdditionalProp2 AdditionalProp2 { get; set; }
//        public AdditionalProp3 AdditionalProp3 { get; set; }
//    }

//    public class Params
//    {
//        public AdditionalProp1 AdditionalProp1 { get; set; }
//        public AdditionalProp2 AdditionalProp2 { get; set; }
//        public AdditionalProp3 AdditionalProp3 { get; set; }
//    }

//    public class PublicAccessRule
//    {
//    }

//    public class Root
//    {
//        public string Id { get; set; }
//        public string Slug { get; set; }
//        public string ContentGroupId { get; set; }
//        public string Campaign { get; set; }
//        public int CategoryId { get; set; }
//        public string State { get; set; }
//        public string TemplatePath { get; set; }
//        public string Name { get; set; }
//        public string MabExperimentId { get; set; }
//        public string AuthorName { get; set; }
//        public string AbTestId { get; set; }
//        public string CreatedById { get; set; }
//        public string UpdatedById { get; set; }
//        public string Domain { get; set; }
//        public string Subcategory { get; set; }
//        public string AbStatus { get; set; }
//        public string FolderId { get; set; }
//        public WidgetContainers WidgetContainers { get; set; }
//        public Widgets Widgets { get; set; }
//        public string Language { get; set; }
//        public string TranslatedFromId { get; set; }
//        public Translations Translations { get; set; }
//        public string DynamicPageHubDbTableId { get; set; }
//        public int DynamicPageDataSourceType { get; set; }
//        public string DynamicPageDataSourceId { get; set; }
//        public bool PageRedirected { get; set; }
//        public string Url { get; set; }
//        public string Password { get; set; }
//        public bool CurrentlyPublished { get; set; }
//        public bool PublishImmediately { get; set; }
//        public string LinkRelCanonicalUrl { get; set; }
//        public bool ArchivedInDashboard { get; set; }
//        public string CurrentState { get; set; }
//        public bool PageExpiryEnabled { get; set; }
//        public int PageExpiryRedirectId { get; set; }
//        public string PageExpiryRedirectUrl { get; set; }
//        public int PageExpiryDate { get; set; }
//        public string ContentTypeCategory { get; set; }
//        public LayoutSections LayoutSections { get; set; }
//        public string HtmlTitle { get; set; }
//        public bool IncludeDefaultCustomCss { get; set; }
//        public bool EnableLayoutStylesheets { get; set; }
//        public bool EnableDomainStylesheets { get; set; }
//        public string FeaturedImageAltText { get; set; }
//        public List<AttachedStylesheet> AttachedStylesheets { get; set; }
//        public string MetaDescription { get; set; }
//        public string HeadHtml { get; set; }
//        public string FooterHtml { get; set; }
//        public string FeaturedImage { get; set; }
//        public bool UseFeaturedImage { get; set; }
//        public bool PublicAccessRulesEnabled { get; set; }
//        public List<PublicAccessRule> PublicAccessRules { get; set; }
//        public ThemeSettingsValues ThemeSettingsValues { get; set; }
//        public DateTime PublishDate { get; set; }
//        public DateTime Created { get; set; }
//        public DateTime Updated { get; set; }
//        public DateTime ArchivedAt { get; set; }
//    }

//    public class Row
//    {
//        public AdditionalProp1 AdditionalProp1 { get; set; }
//        public AdditionalProp2 AdditionalProp2 { get; set; }
//        public AdditionalProp3 AdditionalProp3 { get; set; }
//    }

//    public class RowMetaDatum
//    {
//        public Styles Styles { get; set; }
//        public string CssClass { get; set; }
//    }

//    public class SideOrCorner
//    {
//        public string VerticalSide { get; set; }
//        public string HorizontalSide { get; set; }
//    }

//    public class Styles
//    {
//        public string VerticalAlignment { get; set; }
//        public BackgroundColor BackgroundColor { get; set; }
//        public BackgroundImage BackgroundImage { get; set; }
//        public BackgroundGradient BackgroundGradient { get; set; }
//        public int MaxWidthSectionCentering { get; set; }
//        public bool ForceFullWidthSection { get; set; }
//        public string FlexboxPositioning { get; set; }
//    }

//    public class ThemeSettingsValues
//    {
//        public AdditionalProp1 AdditionalProp1 { get; set; }
//        public AdditionalProp2 AdditionalProp2 { get; set; }
//        public AdditionalProp3 AdditionalProp3 { get; set; }
//    }

//    public class Translations
//    {
//        public AdditionalProp1 AdditionalProp1 { get; set; }
//        public AdditionalProp2 AdditionalProp2 { get; set; }
//        public AdditionalProp3 AdditionalProp3 { get; set; }
//    }

//    public class WidgetContainers
//    {
//        public AdditionalProp1 AdditionalProp1 { get; set; }
//        public AdditionalProp2 AdditionalProp2 { get; set; }
//        public AdditionalProp3 AdditionalProp3 { get; set; }
//    }

//    public class Widgets
//    {
//        public AdditionalProp1 AdditionalProp1 { get; set; }
//        public AdditionalProp2 AdditionalProp2 { get; set; }
//        public AdditionalProp3 AdditionalProp3 { get; set; }
//    }


//}


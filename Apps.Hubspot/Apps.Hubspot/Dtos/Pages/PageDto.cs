using System;
namespace Apps.Hubspot.Dtos.Pages
{
    public class PageBaseDto
    {
        public string Id { get; set; }
        public bool CurrentlyPublished { get; set; }
        public string CurrentState { get; set; }
        /// <summary>
        /// Title of the page
        /// </summary>
        public string HtmlTitle { get; set; }
        /// <summary>
        /// Layout section that contains the components inside the page
        /// </summary>
        public dynamic LayoutSections { get; set; }
    }
    public class PageDto : PageBaseDto
    {
        public string Slug { get; set; }
        public string ContentGroupId { get; set; }
        public string CategoryId { get; set; }
        public string State { get; set; }
        public string AuthorName { get; set; }
        public string CreatedById { get; set; }
        public string UpdatedById { get; set; }
        public string Domain { get; set; }
        public string Subcategory { get; set; }
        public string FolderId { get; set; }
        public bool PageRedirected { get; set; }
        public string Url { get; set; }
        public bool PageExpiryEnabled { get; set; }
        public int PageExpiryDate { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime ArchivedAt { get; set; }

        /**
         * Translateable properties
         */

        /// <summary>
        /// Meta data of the page
        /// </summary>
        // TODO: This field isn't translated
        public string MetaDescription { get; set; }
        /// <summary>
        /// Custom HTML in head
        /// </summary>
        // TODO: This field isn't translated
        public string HeadHtml { get; set; }
        /// <summary>
        /// Custom HTML in the footer
        /// </summary>
        // TODOL This field isn't translated
        public string FooterHtml { get; set; }
        /// <summary>
        /// Name of the page
        /// </summary>
        // TODO: Need to discuss with @Mathijs if we need to translate this as well
        public string Name { get; set; }

        /**
         * Important for Translation 
         */

        /// <summary>
        /// Language of Translation
        /// </summary>
        public string? Language { get; set; }
        /// <summary>
        /// Parent page Id
        /// </summary>
        public string? TranslatedFromId { get; set; }
        /// <summary>
        /// List of Translations
        /// </summary>
        public dynamic Translations { get; set; }
    }
}


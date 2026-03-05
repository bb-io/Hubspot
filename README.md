# Blackbird.io Hubspot CMS

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

HubSpot CMS is a user-friendly platform designed to streamline the process of creating, managing, and optimizing digital content for websites. It offers a range of tools that allow businesses to build and customize their online presence without requiring extensive technical expertise. The platform's intuitive drag-and-drop interface facilitates effortless content creation and editing, and it emphasizes inbound marketing strategies. This makes HubSpot CMS a valuable solution for businesses aiming to enhance their online presence, engage their audience, and drive meaningful conversions.

With multilingual content management, global content modules, and content personalization features, HubSpot CMS makes it easy to adapt digital content for different languages and regions. Now, with a seamless connection from Blackbird.io, businesses can effortlessly synchronize and manage their content, data, and marketing strategies across platforms. Achieve higher levels of operational efficiency, reduce manual efforts, and ensure consistent brand messaging across diverse markets.

## Before setting up

Before you can connect you need to make sure that:

- You have a HubSpot CMS account and [set up your HubSpot account](https://knowledge.hubspot.com/get-started/set-up-your-account).
- You have a [project created](https://app.hubspot.com/academy/43682681/lessons/1054824/5082).
- After creating your account, you'll automatically be logged in. [Learn more about logging](https://knowledge.hubspot.com/account/why-can-t-i-log-into-hubspot) in to HubSpot and [troubleshooting password issues](https://knowledge.hubspot.com/account/reset-user-passwords).
- Your account has sufficient permissions. See below.

## Hubspot permissions required

The app makes use of the `content` and `forms` OAuth scopes. You account needs to have access to these scopes. Your account has sufficient permissions if you are a superadmin _or_ your account has **all** of the following permissions:

- Marketing
   - Forms
   - Marketing email (view/edit/publish)
   - Blog, landing pages, website pages (view/edit/publish)
   - Design tools
   - Content staging
- Account > Settings access
   - App marketplace access
   - Website settings
   - Developer tools

> To use actions related to HubDB, your app needs to have access to `hubdb` scope.

## Connecting

1. Navigate to apps and search for **Hubspot (CMS)**.
2. Click _Add Connection_.
   ![Add Connection](image/README/connection.png)
3. Name your connection for future reference e.g. 'My organization'.
4. Click _Authorize connection_.
5. Follow the instructions that HubSpot CMS gives you, authorizing Blackbird.io to act on your behalf.
6. At the last stage, Hubspot will most likely redirect you to our service without the need to press the 'Connect app' button. <b>Please wait a few seconds for this to happen and avoid pressing the button</b>, as it will duplicate the request that Hubspot has already sent and could cause a UI bug on the connection page. The new connection may appear as 'Not Connected,' but after updating the app page, the status will change to 'Connected.' If Hubspot doesn't redirect you to our service within 5-7 seconds, you can click the 'Connect app' button manually
![connect-app-button.png](image/README/connect-app-button.png)
7. When you return to Blackbird, confirm that the connection has appeared and the status is _Connected_.
   ![Connected](image/README/connected.png)

## Actions
### Content
> "Content" includes landing pages, site pages, blog posts, marketing emails, and marketing forms.
- **Search content** Search content that matches the specified filters.
    Advanced settings:
  - **Language ID**: Filter results by language.
  - **Created at**: Include content created at a specific date.
  - **Created after**: Include content created after a specific date.
  - **Created before**: Include content created before a specific date.
  - **Updated at**: Include content updated at a specific date.
  - **Updated after**: Include content updated after a specific date.
  - **Updated before**: Include content updated before a specific date.
  - **Domain equals**: Filter content by an exact domain.
  - **Domain contains**: Filter content by a partial domain value.
  - **Current state**: Filter content by workflow state.
  - **URL contains**: Filter content by URL text.
  - **Slug**: Filter content by slug.
  - **Title contains**: Filter content by partial title text.
  - **Title equals**: Filter content by an exact title.
  - **Updated by user IDs (whitelist)**: Include updates from specific users only.
  - **Updated by user IDs (blacklist)**: Exclude updates from specific users.
- **Get translation language codes** Get translation language codes for a specific content item.
- **Get content** Get metadata for a specific content item.
- **Download content** Download content for a specific content item.
    Advanced settings:
  - **Properties to include**: Include only selected properties in the downloaded file.
  - **Properties to exclude**: Exclude selected properties from the downloaded file.
- **Upload content** Update content from an uploaded file.
    Advanced settings:
  - **Content ID**: Use a specific content item ID from input instead of the file metadata.
  - **Enable internal link localization**: Localize internal links for translated content.
  - **Published site base URL**: Define the base URL used when localizing internal links.
  - **Create new content**: Create a new item for emails and forms instead of updating the original item.
  - **Original item ID**: Use a specific source item ID for blog posts, landing pages, or site pages.
  - **Update slug and meta description from file**: Update page metadata from file meta tags.
- **Update content** Update content by ID.
- **Delete content** Delete content by ID.
> Content actions are the preferred path for new automations. Other action groups remain available but are planned for deprecation over time.
### Pages
- **Search site pages** Search site pages that match the specified filters.
    Advanced settings:
  - **Not translated in language**: Return pages that do not have a translation in the selected language.
  - **Site name contains**: Filter by partial page name.
  - **Site name equals**: Filter by exact page name.
  - **State**: Filter by publication state.
  - **A/B status**: Filter by A/B test status.
- **Get site page translation language codes** Get translation language codes for a site page.
- **Get site page** Get details for a specific site page.
- **Get site page as HTML file** Download a site page for translation or review.
- **Translate site page from HTML file** Create or update a site page translation from a file.
    Advanced settings:
  - **Source page ID**: Set the source page explicitly when it is missing from file metadata.
  - **Primary language**: Set the primary language when creating the first language variation.
- **Schedule site-page for publishing** Schedule a site page for publishing.
    Advanced settings:
  - **Date time**: Publish at a specific date and time. If omitted, publishing is scheduled shortly after execution.
### Landing page
- **Search landing pages** Search landing pages that match the specified filters.
- **Get landing page** Get details for a specific landing page.
- **Get landing page translation language codes** Get translation language codes for a landing page.
- **Get landing page as HTML file** Download a landing page for translation or review.
- **Translate landing page from HTML file** Create or update a landing page translation from a file.
- **Schedule landing page for publishing** Schedule a landing page for publishing.
### Blog posts
- **Search blog posts** Search blog posts that match the specified filters.
    Advanced settings:
  - **Only IDs**: Output only blog post IDs.
- **Get blog post** Get details for a specific blog post.
- **Create blog post** Create a new blog post.
- **Update blog post** Update an existing blog post.
- **Delete blog post** Delete a blog post.
- **Get blog post as HTML file** Download a blog post for translation or review.
- **Translate blog post from HTML file** Create or update a blog post translation from a file.
    Advanced settings:
  - **Post ID**: Set the source blog post explicitly when it is missing from file metadata.
- **Schedule blog post for publishing** Schedule a blog post for publishing.
### Marketing emails
> HubSpot marks the marketing email API as beta.
- **Search marketing emails** Search marketing emails that match the specified filters.
    Advanced settings:
  - **Archived**: Include archived email records in the search.
- **Create marketing email** Create a new marketing email.
- **Get marketing email content as HTML** Download marketing email content for translation or review.
    Advanced settings:
  - **Exclude title from file**: Exclude the title from the exported HTML file.
- **Update marketing email** Update marketing email properties.
    Advanced settings:
  - **Subject**: Update the email subject.
  - **Business unit ID**: Set the business unit for the email.
- **Update marketing email content from HTML** Update marketing email content from a file.
    Advanced settings:
  - **Marketing email ID**: Use an explicit marketing email ID instead of relying on file metadata.
- **Create marketing email from HTML** Create a marketing email from a file.
### Marketing forms
> HubSpot marks the marketing forms API as beta.
- **Search marketing forms** Search marketing forms that match the specified filters.
    Advanced settings:
  - **Form types**: Filter by one or more form types.
- **Get marketing form** Get a marketing form by ID.
- **Create marketing form** Create a new marketing form.
- **Get marketing form content as HTML** Download marketing form content for translation or review.
- **Update marketing form from HTML** Update a marketing form from a file.
    Advanced settings:
  - **Form ID**: Use an explicit form ID instead of relying on file metadata.
- **Create marketing form from HTML** Create a marketing form from a file.
### HubDB
- **Search tables** Search HubDB tables that match the specified filters.
    Advanced settings:
  - **Name contains**: Filter tables by partial technical name.
  - **Label contains**: Filter tables by partial label.
  - **Updated by (User IDs)**: Filter tables by updater user IDs.
- **Export table** Download a HubDB table in the selected format.
- **Publish table** Publish a HubDB table from draft to published.
- **Search rows** Search HubDB table rows by table ID or name.
    Advanced settings:
  - **Filter query**: Apply HubDB filter syntax to row search.
- **Download rows content** Download selected columns and rows from a HubDB table.
- **Update row column** Update a column value in a HubDB draft row.
    Advanced settings:
  - **Text value**: Set a text value.
  - **Numeric value**: Set a number value.
  - **Date value**: Set a date value.
- **Upload rows content** Create or update HubDB rows from a file.
### Miscellaneous
- **Debug** Output connection credentials for debugging.
- **Get connected site info** Get account information for the connected site for debugging.
## Events
### Content
- **On content created or updated** Outputs content created or updated since the previous poll.
    Advanced settings:
  - **Content types**: Limit events to selected content types.
  - **Language**: Limit events to a specific language.
  - **Published**: Limit events to published content only.
### Blog posts
- **On blog posts created or updated** Outputs blog posts created or updated since the previous poll.
### Pages
- **On site pages created or updated** Outputs site pages created or updated since the previous poll.
### Landing page
- **On landing pages created or updated** Outputs landing pages created or updated since the previous poll.
### Marketing forms
- **On marketing forms created or updated** Outputs marketing forms created or updated since the previous poll.
### Marketing emails
- **On marketing emails created or updated** Outputs marketing emails created or updated since the previous poll.
> Events use polling. Configure an interval from 5 minutes up to 7 days.
## Useful tips

All actions that work with HTML files will add a meta tag to the HTML. This meta tag is named `blackbird-reference-id`. This tag is used to identify the content in the Hubspot CMS, eliminating the need to store IDs elsewhere

## Examples

![Example](image/README/Event-example.png)

This example uses a polling event to check for new blog posts. When a new blog post is created, the event triggers and the blog post is translated into a different language.

![Generic example](image/README/generic-bird.png)

This example demonstrates how you can work with the `Hubspot CMS` app in a generic way. This bird will pick up all updated or created content within a specified time interval and translate it into a specific language. Using this approach, you don't need to create five different birds; you can just create one for this purpose

<!-- end docs -->


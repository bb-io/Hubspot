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

### Content actions

> "Content" refers to all content types available in our app, including Landing & pages, blog posts, marketing emails, and marketing forms. These objects are unified under a single content class that includes the following properties: Content ID, Title, Language, Created At, Updated At, and Content Type (blog, landing_page, site_page, email, or form).

- **Search content**: Search for any type of content.
- **Get content**: Retrieve metadata for a specific content type based on its ID.
- **Get translation language codes** returns list of translated locales for specific content based on ID (available only for `landing/site pages` and `blog posts` content types)
- **Download content**: Download content as HTML for a specific content type based on its ID.
- **Upload content**: Update content from an HTML file.
  - **Enable internal link localization**: When enabled, this feature automatically updates internal links within your uploaded HTML content to point to the correct localized versions. This is particularly useful when managing multilingual content across different language variants. Instead of manually adjusting each link for different language versions, the system will intelligently transform URLs to match the target locale of the content being uploaded. This works for internal links pointing to site pages, landing pages, and blog posts.
  - **Published site base URL**: This optional parameter allows you to specify the base URL of your published site (e.g., https://example.com). When provided alongside the internal link localization feature, it helps the system correctly identify and process internal links by establishing the root domain for your content. This ensures that only links within your domain are processed for localization.
  - **Create new content**: This optional parameter allows you to create new content instead of updating existing content. **Important**: this is valid only for `marketing forms` and `marketing emails` content types, since they don't have a localization feature in hubspot. For all other content types, this parameter is ignored.
- **Update content**: Update content based on specified criteria using its ID.
- **Delete content**: Delete content based on its ID.

> Please note that we strongly recommend using these actions, as in the future, we will mark all other actions as deprecated. We will not add any new features to them and will eventually delete them (you will still be able to use the non-latest version of Hubspot app and have access to those actions).

### Landing & site pages

> Note: the actions for landing and site pages are the same, but have separate actions for each. It is preferable that you use the content actions as outlined above as they are set to replace these individual actions below.

A site page is a more general and versatile page on Hubspot CMS website that can contain various types of content and is part of your site's overall navigation and structure. A landing page in HubSpot CMS is a standalone page typically designed for a specific marketing campaign, with a clear call-to-action (CTA) to capture leads.

- **Get a landing/site page** Retrieve information from a specific page on your website by selecting it from a dynamic drop-down list as an input value.
- **Search landing/site pages** searches for landing/site pages matching certain criteria. One of those criteria is _Not translated in language_ which you can use to filter pages that are missing translations.
- **Get landing/site page translation language codes** returns list of translated locales for landing/site page based on ID
- **Get a landing/site page as HTML file** Get webpage details and an HTML file of its content for easy translation and integration while preserving formatting and layout.
- **Translate a landing/site page from HTML file** Create a new translation for a landing/site page based on an HTML file input. It enables seamless integration of externally translated content back into the website while maintaining the HTML structure. Note: if you are using this action with a landing/site page that has no translations yet, you need to provide the primary language.
- **Schedule a landing/site page for publishing** Automate webpage publishing to release translated content at the right time for a better user experience.

### Blog posts

- **Search blog posts** searches for blog posts matching certain criteria. One of those criteria is _Not translated in language_ which you can use to filter pages that are missing translations.
- **Get blog post** Get blog post information.
- **Create blog post** Create a new blog post. This action allows for the generation of fresh, localized content to engage global audiences and expand the reach of your blog or website, fostering international growth and user engagement.
- **Delete blog post** Delete a blog post
- **Get blog post as HTML file** Get blog post as HTML file.
- **Translate blog post from HTML file** Translate blog post from HTML file.
- **Update blog post** Update a blog post information.
- **Schedule a blog post page for publishing** Automate blog post publishing to release translated content at the right time for a better user experience.

### Marketing emails

Note: the Hubspot API marks these endpoints in beta stage.

- **Search marketing emails** searches for marketing emails based on certain criteria.
- **Create marketing email** creates a new marketing email.
- **Get marketing email content as HTML** returns a marketing email content from a (translated) HTML file. Field `Marketing Email ID` is required 
- **Update marketing email content from HTML** updates a marketing email content from a (translated) HTML file. Fields `File` and `Marketing Email ID` is required.
- **Create marketing email from HTML** creates a marketing email by extracting content from an uploaded HTML file

### Marketing forms

Note: the Hubspot API marks these endpoints in beta stage.

- **Search marketing forms** searches for marketing forms based on certain criteria.
- **Get marketing form** retrieves a marketing form.
- **Get marketing form as HTML** returns a marketing form as an HTML file for convenient translation.
- **Update marketing form from HTML** updates a marketing form from a (translated) HTML file.
- **Create marketing form** creates a new marketing form.
- **Create marketing form from HTML** create a marketing form from a HTML file content, using metadata IDs: `name`, `type`, `language`, `archived`. If there is no specific metadata, then it search for HTML tag IDs: `name`, `type`, `language`, `archived`.

> Important: when using the `Update/create .. from HTML` actions, please make sure that the HTML file was generated from the `Get .. as HTML` action. Otherwise, the HTML file may not be compatible with the Hubspot CMS and may cause errors when uploading.

## Events

- **On content created or updated** (polling): Triggered at specified time intervals and returns all blog posts, landing pages, site pages, emails, and forms that were updated or created during the specified time interval.
- **On blog posts created or updated** (polling): Triggered at specified time intervals and returns all blog posts was created or updated.
- **On landing pages created or updated** (polling): Triggered at specified time intervals and returns all landing pages was created or updated.
- **On site pages created or updated** (polling): Triggered at specified time intervals and returns all site pages was created or updated.

> This events are working on polling mechanism, so you won't be immediately notified about the changes. You can configure the polling interval starting from 5 minutes to 7 days.

## Useful tips

All actions that work with HTML files will add a meta tag to the HTML. This meta tag is named `blackbird-reference-id`. This tag is used to identify the content in the Hubspot CMS, eliminating the need to store IDs elsewhere

## Examples

![Example](image/README/Event-example.png)

This example uses a polling event to check for new blog posts. When a new blog post is created, the event triggers and the blog post is translated into a different language.

![Generic example](image/README/generic-bird.png)

This example demonstrates how you can work with the `Hubspot CMS` app in a generic way. This bird will pick up all updated or created content within a specified time interval and translate it into a specific language. Using this approach, you don't need to create five different birds; you can just create one for this purpose

<!-- end docs -->

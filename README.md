# Blackbird.io Hubspot CMS

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

HubSpot CMS is a user-friendly platform designed to streamline the process of creating, managing, and optimizing digital content for websites. It offers a range of tools that allow businesses to build and customize their online presence without requiring extensive technical expertise. The platform's intuitive drag-and-drop interface facilitates effortless content creation and editing, and it emphasizes inbound marketing strategies. This makes HubSpot CMS a valuable solution for businesses aiming to enhance their online presence, engage their audience, and drive meaningful conversions.

With multilingual content management, global content modules, and content personalization features, HubSpot CMS makes it easy to adapt digital content for different languages and regions. Now, with a seamless connection from Blackbird.io, businesses can effortlessly synchronize and manage their content, data, and marketing strategies across platforms. Achieve higher levels of operational efficiency, reduce manual efforts, and ensure consistent brand messaging across diverse markets. 


## Before setting up

Before you can connect you need to make sure that:

- You have a HubSpot CMS account and [Set up your HubSpot account](https://knowledge.hubspot.com/get-started/set-up-your-account).
- You have a [project created](https://app.hubspot.com/academy/43682681/lessons/1054824/5082).
- After creating your account, you'll automatically be logged in. [Learn more about logging](https://knowledge.hubspot.com/account/why-can-t-i-log-into-hubspot) in to HubSpot and [troubleshooting password issues](https://knowledge.hubspot.com/account/reset-user-passwords).

## Connecting

1. Navigate to apps and search for **Hubspot (CMS)**. If you cannot find Hubspot (CMS) then click _Add App_ in the top right corner, select Hubspot (CMS) and add the app to your Blackbird environment.
2. Click _Add Connection_. 
![Add Connection](<Images /README/Add Connenction.png>)
3. Name your connection for future reference e.g. 'My organization'.
5. Click _Authorize connection_.
6. Follow the instructions that HubSpot CMS gives you, authorizing Blackbird.io to act on your behalf.
7. When you return to Blackbird, confirm that the connection has appeared and the status is _Connected_.
![Connected](<Images /README/Status Connected .png>)

## Actions
### Website Pages 

- **Get a site page** Retrieve information from a specific page on your website by selecting it from a dynamic drop-down list as an input value. A website page is a more general and versatile page on Hubspot CMS website that can contain various types of content and is part of your site's overall navigation and structure.
- **Get all site pagess** Enables the retrieval of information about all website pages
- **Get all site pages not translated in language X** a list of all pages that have not been translated into the specified language; for instance identifying untranslated pages in language-X to prioritize translation efforts and improve the experience for language-X speaking customers.
- **Get all site pages updated after datetime** Get a list of all site pages that were updated after the given date and time. You can add a date and time as an inpute value. 
- **Get all site pages with language** Easily retrieving a comprehensive list of web pages tailored to a specific languag; for instance, a multilingual marketing platform can use this action to display all Spanish-language pages for targeted translation efforts.
- **Get a site page as HTML file** Get webpage details and an HTML file of its content for easy translation and integration while preserving formatting and layout.
- **Translate a site page from HTML file** Create a new translation for a site page based on an HTML file input. It enables seamless integration of externally translated content back into the website while maintaining the HTML structure.
- **Schedule a site-page for publishing** Automate webpage publishing to release translated content at the right time for a better user experience.

### Landing Pages
- **Get a landing page** Enables the retrieval of information about a particular landing page in Hubspot CMS. A landing page in HubSpot CMS is a standalone page typically designed for a specific marketing campaign, with a clear call-to-action (CTA) to capture leads.

- **Get all landing pages** This action is best for efficiently managing and updating translated content for marketing campaigns, ensuring consistency and timely delivery.
- **Get a landing page as HTML file** Regain the content of a designated landing page and return it in HTML format for easy translation and integration while preserving formatting and layout.
- **Get all landing pages not translated in language X** Get a list of all pages not translated in specified language 
- **Get all landing pages updated after datetime** Get a list of all landing pagess that were updated after the given date time. Date time is exclusive
- **Get all landing pages with language X** Get a list of all pages in a specified language. Obtaining a list of all pages in a specified language is crucial in a translation automated workflow to maintain language consistency across the entire website, enhancing user experience and SEO.
- **Schedule a landing page for publishing** Schedules a landing page for publishing on the given time. This action is essential in a translation automated workflow to ensure that translated marketing campaigns launch seamlessly, optimizing their effectiveness and reach.
- **Translate a landing page from HTML file** Create a new translation for a site page based on a file input. It enables seamless integration of externally translated content back into the landing page while maintaining the HTML structure.

### Blog posts 
- **Create blog post** Create a new blog post. This action allows for the generation of fresh, localized content to engage global audiences and expand the reach of your blog or website, fostering international growth and user engagement.
- **Create blog language variation**. Create new blog language variation; For instance a content agency can use this action to make a Spanish version of an English blog post, reaching Spanish-speaking audiences without starting from scratch.
- **Delete blog post** Delete a blog post
- **Get all blog posts** Get a list of all blog posts
- **Get blog post** Get blog post
- **Get blog post as HTML file** Get blog post as HTML file
- **Get blog posts without translations** Get blog posts without translations to specific language
- **Get blog post translation** Get blog post translation by language
- **Translate blog post from HTML file** Translate blog post from HTML file
- **Update blog post** Update a blog post information
- **Update name and body of blog post** Update name and body of blog post


## Example
![Example](<Images/README/Hubspot-CMS-Blackbird-Example.png>)

## Missing features

In the future we can add actions for:

Let us know if you're interested!

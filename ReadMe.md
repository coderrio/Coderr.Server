Coderr Community Server
=============================

[![Build status](https://1tcompany.visualstudio.com/_apis/public/build/definitions/75570083-b1ef-4e78-88e2-5db4982f756c/6/badge)]() [![Github All Releases](https://img.shields.io/github/downloads/coderrio/coderr.server/total.svg?style=flat-square)]()

# Discover more errors and solve them faster!

![Welcome screen](docs/dashboard.png)

[Watch our intro video](https://www.youtube.com/watch?v=E6q3EkEIwVk)

## Search function

![Search using your own data](docs/search.png)

.. don't want to host/maintain your own server? Try [Coderr Live](https://coderr.io/live/)

## What’s Coderr?

Coderr is an error management service developed specifically for .NET and JavaScript applications. Coderr provides an insight to all errors that are occurring and gives you control and a complete view of your applications’ status. 

## Why change?

Compared to log files and log analysis tools (like Splunk / Kibana), Coderr fouces exclusivly on errors (both exceptions and other types) where everything works out of the box. No need to customize dashboards, views or anything else.
All errors are grouped out of the box and you can see how often they occurr, in which environment (like "production") and in which application versions.

Once an error is corrected, all future reports for it will automatically be ignored unless the error surfaces in a newer application version.

With Coderr, you can focus on building new features and spend minimal time correcting bugs.

## Getting started

1. [Download Coderr Server](https://github.com/coderrio/Coderr.Server/releases), use our [cloud service](https://app.coderr.io) (free for up to five users) or use our [Docker image]()
2. Install one of our [nuget libraries](https://www.nuget.org/packages?q=coderr.client) (or [npm library](https://www.npmjs.com/package/coderr.client)).
3. Follow the instructions in the package ReadMe (max three lines of code to get started).
4. Try the code below.

**Unhandled exceptions will automatically be reported by the client libraries.**

To report exceptions manually:

```csharp
public void UpdatePost(int uid, ForumPost post)
{
	try
	{
		_service.Update(uid, post);
	}
	catch (Exception ex)
	{
		Err.Report(ex, new{ UserId = uid, ForumPost = post });
	}
}
```

The context information will in this case be attached as:

![](https://coderr.io/images/features/custom-context.png)

You can learn more about reporting errors [here](https://coderr.io/documentation/).

## Running Coderr

You can run any Coderr in development, test and in production. Coderr is available in three different ways; as Coderr Community Server (AGPL license, self-hosting), as Coderr Live (commercial license, cloud version) or on request, as Coderr running on premise (commercial license, self-hosting version). Coderr Live and Coderr on premise add powerful algorithms to prioritize errors and provide insight to how your code is improving over time with applied solutions. Coderr was rebranded and developed from OneTrueError in 2017.

[Read more](https://coderr.io/features/)


## About us

We are passionate about Open Source, Microsoft .NET and code quality. 1TCompany started in 2017 in Sweden and builds on years of coding experience and bringing products to market. Our mission is to assist fellow developers deliver quality code. To accomplish this mission, we decided to make Coderr commercially available and ready for prime time.


## Community

* [Discussion board](http://discuss.coderr.io)
* [Report bugs](https://github.com/coderr.io/coderr.server/issues)
* [Documentation](https://coderr.io/documentation)
* [Commercial support](mailto:support@coderr.io?subject=Commercial%20support%20inquiry)

## Licensing

* Community Server: [AGPL](License)
* Client libraries: [Apache 2.0](https://opensource.org/licenses/apache-2.0)
* [Coderr Live](https://coderr.io/live): Commercial
* [Coderr OnPremise](https://coderr.io/features): Commercial

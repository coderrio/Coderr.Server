Coderr Community Server
=============================

[![Build status](https://1tcompany.visualstudio.com/_apis/public/build/definitions/75570083-b1ef-4e78-88e2-5db4982f756c/6/badge)]() [![Github All Releases](https://img.shields.io/github/downloads/coderrapp/coderr.server/total.svg?style=flat-square)]()

# Skip logfiles - Use automated exception handling!

![OSS screenshot of v2.0](docs/screenshot.png)*screenshot is from the version 2.0 beta*


.. don't want to host/maintain your own server? Try [Coderr Live](https://coderr.io/live/)

## What’s Coderr?

Coderr is an error handling tool developed specifically for .NET applications. Coderr provides an insight to all errors that are occurring and gives you control and a complete view of your applications’ status. 

## Why change?

Too often, error detection and management involves relying on reports from users and random logfile scanning. The unknown amount of errors and weaknesses of the code creates uncertainty and lessens your control. With a systematic approach facilitated by a tool built for developers by developers, Coderr provides insight about the status of the applications. 


## Getting started

Once you have [downloaded and installed the server](https://github.com/coderrapp/Coderr.Server/releases) you need to install and configure one of our nuget packages. You can read more about them [here](https://coderrapp.com/documentation/client/).

Unhandled exceptions will be picked up by the client libraries. 

To report exceptions yourself:

```csharp
public void UpdatePost(int uid, ForumPost post)
{
	try
	{
		_service.Update(uid, post);
	}
	catch (Exception ex)
	{
		OneTrue.Report(ex, new{ UserId = uid, ForumPost = post });
	}
}
```

The context information will in this case be attached as:

![](https://coderrapp.com/images/features/custom-context.png)

You can learn more about reporting errors [here](https://coderr.io/documentation/).

## Running Coderr

You can run any Coderr in development, test and in production. Coderr is available in three different ways; as Coderr Community Server (AGPL license, self-hosting), as Coderr Live (commercial license, cloud version) or on request, as Coderr running on premise (commercial license, self-hosting version). Coderr Live and Coderr on premise add powerful algorithms to prioritize errors and provide insight to how your code is improving over time with applied solutions. Coderr was rebranded and developed from OneTrueError in 2017.

[Read more](https://coderr.io/features/)


## About us

We are passionate about Open Source, Microsoft .NET and code quality. 1TCompany started in 2017 in Sweden and builds on years of coding experience and bringing products to market. Our mission is to assist fellow developers deliver quality code. To accomplish this mission, we decided to make Coderr commercially available and ready for prime time.


## Community

* [Discussion board](http://discuss.coderrapp.com)
* [Report bugs](https://github.com/coderrapp/coderr.server/issues)
* [Documentation](https://coderrapp.com/documentation)
* [Commercial support](mailto:support@coderrapp.com?subject=Commercial%20support%20inquiry)

## Licensing

* Community Server: [AGPL](License)
* Client libraries: [Apache 2.0](https://opensource.org/licenses/apache-2.0)
* [codeRR Live](https://coderrapp.com/live): Commercial
* [codeRR OnPremise](https://coderrapp.com/live): Commercial

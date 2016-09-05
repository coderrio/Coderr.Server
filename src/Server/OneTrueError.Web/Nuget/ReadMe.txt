OneTrueError.Client installation
=================================

You've just installed the OneTrueError client. 

To get started add the following code to your application:

	var url = new Uri("http://yourServer/onetrueerror/");
	OneTrue.Configuration.Credentials(url, "yourAppKey", "yourSharedSecret");

Once done you can report exceptions like this:

	try
	{
		somelogic();
	}
	catch(SomeException ex)
	{
		OneTrue.Report(ex);
	}


More information
=================

* http://onetrueerror.com/ - About the service
* http://onetrueerror.com/documentation/client/index.md - Client documentation
* http://onetrueerror.com/documentation/server/index.md - Server documentation (to extend/change the OnTrueError server source code)


*this library requires that you have installed a OneTrueError server somewhere*

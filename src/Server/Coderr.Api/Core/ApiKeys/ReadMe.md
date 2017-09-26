ApiKeys
========

Used to allow external applications to talk with OneTrueError.


## Example usage

The following example calls a local OneTrueError server to retreive applications.

```csharp
var client = new OneTrueApiClient();
var uri = new Uri("http://yourServer/onetrueerror/");
client.Open(uri, "theApiKey", "sharedSecret");
var apps = await client.QueryAsync(new GetApplicationList());
```

Result (serialized as JSON):

```javascript
[{
		"Id" : 1,
		"Name" : "PublicWeb"
	}, {
		"Id" : 9,
		"Name" : "Time reporting system"
	}, {
		"Id" : 10,
		"Name" : "Coffee monitor"
	}
]
```

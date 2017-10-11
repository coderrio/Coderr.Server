ApiKeys
========

Used to allow external applications to talk with codeRR.


## Example usage

The following example calls a local codeRR server to retreive applications.

```csharp
var client = new ServerApiClient();
var uri = new Uri("http://yourServer/coderr/");
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

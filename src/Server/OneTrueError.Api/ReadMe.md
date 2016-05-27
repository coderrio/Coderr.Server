The API is based on Command/Queries and events. 
==============

Commands can be seen as the write model. All operations is done with the
help of commands. A command is not an atomic unit, but do in most cases represent an use case.

Queries are the read model in the application. They are used to fetch information. Queries are indempotent and may not change
application state.

Events are used to allow different parts of the application to talk. The publisher are not aware of if there are any 
subscribers or how many there are. The subscriber have no knowledge about who published the event.

# Implementations

There is a tool in the "Tool" root folder which are used to generate Typescript classes from these APIs. The `.ts` files can be
invoked using ajax directly from the web.

You can also invoke the DTOs directly from your application using a HTTP client. Serialize the DTO as JSON and then include
`X-Cqs-Object-Type` as a HTTP header. It should contain the assembly qualified type name of the DTO. 

Basic authentication is used. Thus we recommend that you run the site using SSL.

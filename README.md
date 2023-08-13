# Magic Onion Generic Template

This is a plug-and-play MagicOnion template with generic service and hub implementations that use MemoryPack serialization.

## Package Installation

You can install this template using [NuGet](https://www.nuget.org/packages/MagicOnionGenericTemplate):

```powershell
dotnet new install MagicOnionGenericTemplate
```

```powershell
dotnet new magic-onion-generic -n YourProjectName
```

### Template Contents
The template contains the following projects:

#### Client
This project includes implementations for service base, hub base, services, hubs, and filters.

 ##### Client Filters: 

###### AuthenticationFilter: 
 When used, this filter adds the binary token to the gRPC call context before each request.
 Note: You might need to reconfigure how to store and fetch tokens to be used in the filter.

###### RateLimiterFilter:
Imposes rate limiting rules before making a request. Configuration can be done via the appsettings.json file.

Note: To manage soft/hard blocked IPs, Redis is used. You can create your own implementation if you prefer not to use Redis.
 
 ##### Note: To store soft/hard blocked Ips, I've used Redis, you can make your own implementation if you do not want to use redis.

 
 #### Redis
This project contains implementations for rate limiting, client blocking, and token cache services. These services are currently implemented on the client side, but you can also implement them on the server side.

 ##### Note: Token caching with Redis is available in the project, although it hasn't been utilized in this implementation.
 ##### Note: Rate limiting and blocking rules can be defined in the appsettings.json file.
 
 #### Server
This project provides server-side implementations of generic hubs and services. Services and hubs include EF DbContext extensions. However, EF does not support custom query execution. Therefore, an additional feature named AQueryMaker has been implemented. This library assists in executing custom queries or stored procedures and includes a feature called ExecuteStream, which allows streaming of queries, eliminating the need to wait for complete execution. The ToModel extension can be used to convert the dictionary output into desired POCO models.

In addition, user and role classes have been implemented using MasterMemory for in-memory storage. Roles and permissions are utilized in server-side gRPC filters after token validation, enabling checks for required roles or permissions before proceeding.

### Nuget Packages Used in this project

## MagicT.Client

MagicMagicOnion.Client
- This is a client library for connecting to MagicOnion services from Blazor WebAssembly apps. It enables calling server-side APIs from client code.

MessagePipe
- Provides a lightweight messaging implementation for passing messages between Blazor components. Useful for decoupling components.

Majorsoft.Blazor.Extensions.BrowserStorage
- This library provides easy access to browser storage (localStorage and sessionStorage) from Blazor apps. It abstracts away the underlying JavaScript interop required.

MagicOnion.Serialization.MemoryPack
- This is a serializer library that MagicOnion uses to serialize/deserialize messages between client and server. It provides high performance binary serialization based on MessagePack.
 
## MagicT.Server

MagicOnion.Server
- Server-side implementation of MagicOnion for building gRPC services. Handles hosting and exposing gRPC endpoints.

MagicOnion.Server.HttpGateway
- Allows MagicOnion gRPC services to be exposed over HTTP in addition to gRPC. Useful for supporting HTTP clients.

MessagePipe
- As mentioned previously, this provides lightweight messaging between Blazor components.

Microsoft.EntityFrameworkCore
- Object-relational mapper that enables .NET apps to interact with databases. Provides mapping between database and .NET objects.

Utf8Json
- High-performance JSON serialization/deserialization for .NET. Faster than Newtonsoft.Json with a smaller footprint.

LitJWT
- Library for working with JSON Web Tokens (JWT) in .NET. Provides JWT creation, validation, encryption/decryption etc.

MagicOnion.Serialization.MemoryPack
- As mentioned previously, optimized binary serializer used by MagicOnion.


## MagicT.Shared

MagicOnion.Shared
- Contains shared types, interfaces and attributes used by both MagicOnion server and client libraries. Provides common foundation.

MasterMemory
- An in-memory database built on MemoryPack. Provides simple embedded database functionality.

MemoryPack
- Efficient binary serialization library that MasterMemory builds on. Also used by MagicOnion as mentioned earlier.

Mapster
- Library for object-object mapping and copying. Useful for transforming object models. Provides neat mapping API.
 

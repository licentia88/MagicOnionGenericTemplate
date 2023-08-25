# Magic Onion Generic Template

This is a plug-and-play MagicOnion template with generic service and hub implementations that use MemoryPack serialization.
Focused on performance and security this template introduces a built-in rate limiter using Redis. This limiter serves as a robust defense against Denial of Service (DoS) attacks and guards against resource depletion.
The template also integrates advanced encryption techniques like Diffie-Hellman and AES-GCM to secure end-to-end encryption and effective prevention of token theft. In parallel, it streamlines development by providing standard Create, Read, Update, and Delete (CRUD) operations via the services and hubs components, thereby expediting the development lifecycle.

## Package Installation

You can install this template using [NuGet](https://www.nuget.org/packages/MagicOnionGenericTemplate):

```powershell
dotnet new install MagicOnionGenericTemplate
```

#### By default, the project is created on .NET 7.0 and gRPC connections are configured to use SSL

```powershell
dotnet new magic-onion-generic -n YourProjectName
```

Alternatively, you can disable SSL configuration with:

```powershell
dotnet new magic-onion-generic -n YourProjectName -F net7.0 -G false```
```


## Quick start

Note: Before proceeding it's best to gain some knowledge in

Memorypack: Zero encoding extreme performance binary serializer for C# and Unity.

https://github.com/Cysharp/MemoryPack.

MasterMemory: Embedded Typed Readonly In-Memory Document Database for .NET Core and Unity.

https://github.com/Cysharp/MasterMemory

Before we start whenever you see something MagicT it will be replaced with your projectName

### Service Implementation 

We will be creating Service interfaces in the .Shared project, Client and Server impelemtations will be in .Server and .Client Projects.


Let's start. We will still use Magic onion but take leverage of generics

#### .Shared Project
instead of this
```csharp
 public interface IMyFirstService : IService<IMyFirstService>
```
we will be using 
```csharp
public interface ITestService : IMagicTService<ITestService, TestModel>
{
  //Your other method implementations
}

or

public interface ITestService : IMagicTService<ITestService, byte[]>
{
  //Your other method implementations
}

or

public interface ITestService : IMagicTService<ITestService, int>
{
  //Your other method implementations
}
```

IMagicTService derives from IService<TService> and has method blueprints
for Crud and Encrypted Crud operations among with query streaming.

#### .Server project

instead of 

```csharp
public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
    {
        // `UnaryResult<T>` allows the method to be treated as `async` method.
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            Console.WriteLine($"Received:{x}, {y}");
            return x + y;
        }
    }
```

 we will be using 

```csharp
public sealed class TestService : MagicTServerServiceBase<ITestService, TestModel, SomeDbContext>, ITestService
{
    public TestService(IServiceProvider provider) : base(provider)
    {
    }
}

or

//Uses MagicTContext when not defined.
public sealed class TestService : MagicTServerServiceBase<ITestService, TestModel>, ITestService
{
    public TestService(IServiceProvider provider) : base(provider)
    {
    }
}
```

The template also comes with pre configured Entity framework database with some tables for users, roles and permissions.

### .Client project

I have also Created some filters for Ratelimiting, KeyExchanging and Token authentication. which are optional to use.. 

```csharp
public sealed class TestService : MagicTClientSecureServiceBase<ITestService, TestModel>, ITestService
{
    public TestService(IServiceProvider provider) : base(provider
        , new RateLimiterFilter(provider), new AuthenticationFilter(provider))
    {
    }
}
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
 

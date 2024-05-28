# Magic Onion Generic Template

This is a plug-and-play MagicOnion template with generic service and hub implementations that use MemoryPack serialization.
Focused on performance and security this template introduces a built-in rate limiter using Redis. This limiter serves as a robust defense against Denial of Service (DoS) attacks and guards against resource depletion.
The template also integrates advanced encryption techniques like Diffie-Hellman and AES-GCM to secure end-to-end encryption and effective prevention of token theft. In parallel, it streamlines development by providing standard Create, Read, Update, and Delete (CRUD) operations via the services and hubs components, thereby expediting the development lifecycle.

## Let's Connect!
I appreciate every star ⭐ that my projects receive, and your support means a lot to me! If you find my projects useful or enjoyable, please consider giving them a star.

## Package Installation

You can install this template using [NuGet](https://www.nuget.org/packages/MagicOnionGenericTemplate):

```powershell
dotnet new install MagicOnionGenericTemplate
```
For template help

```powershell
dotnet new magic-onion-generic-h
```

#### By default, the project is created on .NET 7.0 and gRPC connections are configured to use SSL

```powershell
dotnet new magic-onion-generic -n YourProjectName
```

Alternatively, you can disable SSL configuration with:

```powershell
dotnet new magic-onion-generic -n YourProjectName -F net7.0 -S false
```


## Enviromental Setup 

If your development environment is on a macos, the ssl configuration will not work due to the lack of ALPN support on mac.

See issue here -> https://github.com/grpc/grpc-dotnet/issues/416

<br/><br/>

> [!IMPORTANT]
>Iff your development environment is windows and you have choosen SSL Configuration you must go to appsettings.json in the server project and uncomment
> ```csharp
> "HTTPS": {
>        "Url": "https://localhost:7197",
>        "Protocols": "Http2"
>  },
>```


> [!IMPORTANT]
> ### Before running the project 
> #### 1. Make sure redis server is running on localhost:6379 (Or you can change it from appsettings.json file both in Web & Server Projects)
> #### 2. Create a new migration and update database
> but before that in the server project, you must 
>  ##### 1. Set your connection string in the appsettings.json file
>  ##### 2. In program.cs change the below section according to your Database preference, by default the template uses MySql Database
> ```csharp builder.Services.AddDbContext<MagicTContext>(
>    options => options.UseMySql(
>        builder.Configuration.GetConnectionString(nameof(MagicTContext)),
>         ServerVersion.AutoDetect(builder.Configuration.GetConnectionString(nameof(MagicTContext)))
>     )
> );
> ```

<br/><br/>

> [!TIP]
> ### From this point on, for simplicity, the tutorial will examine what each project is and the technologies used in each project. The projects are:

<br/><br/>


#### 1. MagicT.Shared
#### 2. MagicT.Web.Shared
#### 3. MagicT.Web/ MagicT.WebTemplate
#### 4. MagicT.Client
#### 5. MagicT.Server

## MagicT.Shared
The Shared project is referenced by other projects and consists of models, interfaces, hub implementations, extension methods, and other helpers.

### 3rd Party Libraries in this project that utilize source code generation and their use cases:

    1. AutoRegisterInject
       Short Description: C# source generator that will automatically create Microsoft.Extensions.DependencyInjection registrations for types marked with attributes.
       Use Case: Generates boilerplate code for DI Registrations
       Repository link : https://github.com/patrickklaeren/AutoRegisterInject

    2. AutomaticDisposeImpl
       Short Description: A source generator that automatically implements methods corresponding to the IDisposable and IAsyncDisposable implementation patterns in C#
       Use Case: Generates boilerplate code for object disposal
       Repository link : https://github.com/benutomo-dev/RoslynComponents
 
    3. Generator.Equals (Optional)
       Short Description: A source code generator for automatically implementing IEquatable<T> using only attributes.
       Use Case: Generates boilerplate code for IEquatable<T>, this helps checking if all values of two differet instances of the same class are the same and is not a must use   
       feature in this project
       Repository link: https://github.com/diegofrata/Generator.Equals
    
    4. MapDataReader
       Short Description: This source code generator creates mapping code that can be used as an IDbDataReader extension. 
       It facilitates easy and efficient data mapping from a database reader to your objects.
       Use case: Generates boilerplate code for mapping, EFCore is not efficient when executing custom queries and this library comes in handy in those situations, we will come  
       review this library again in the Server Project section.
       Repository link: https://github.com/jitbit/mapdatareader

###  Other 3rd Party Libraries in this project
     1. Cysharp/MemoryPack - it provides full support for high performance serialization and deserialization of binary objects
        Use Case: Magiconion by default uses messagepack serialization, I've configured to use Memorypack serialization for this project
        Repository link: https://github.com/Cysharp/MemoryPack
        
     2. Cysharp/MessagePipe - is a high-performance in-memory/distributed messaging pipeline for .NET and Unity
        Use Case: I use this library mainly to notify view, however you can use it like kafka or RabbitMq.
        Repository link: https://github.com/Cysharp/MessagePipe

     3. Serilog - Logging Library
     4. Mapster - Mapper
     5. BouncyCastle - .NET implementation of cryptographic algorithms and protocols
        Use Case: We will be using this library to provide end to end encryption to our services. Will review this in other sections.


### Extension Methods

 I have made some extension methods for different use cases, you can find them in the extensions folder.
 these methods are both used in  web, client and server projects. The code is readable and well-documented, and you can find references to where the methods are called.  
 Therefore, I don't feel the need to explain what each extension method does.

### Cryptography 
 in the CryptoHelper.cs class we have Encrypt and Decrypt methods that we will be using for end to end encryption using diffie-hellman key exchange.

### Managers
  1. LogManager - We will be using the LogManager to log users' interactions with services and to log error messages.
  2. KeyExchangeManager - We will be using the KeyExchangeManager share public keys between the Client and Server when the app starts.
     the public keys will be used to create a shared secret key independently on both Client and Server side. 
     these shared secret keys will be used when Encrypting and Decrypting data both in client and server side.
     you can read more about Diffie-Hellman Keyexchange in : https://en.wikipedia.org/wiki/Diffie%E2%80%93Hellman_key_exchange
     or simply google it.


### Models
I have implemented some classes so that when you create and run migrations, they will become tables in the database.

These classes are responsible for storing traces of user actions (audit/history) such when a user performs a CRUD operation, the logs will be stored in these tables.

I have also created classes that hold user data, permissions, and roles. Within these classes, you can assign permissions to roles and roles to users. Additionally, permissions can be independently assigned to the users.

We will review this in the web project where we have views for these tables.

> [!IMPORTANT]
> Admin users can be configured through the appsettings.json file in the server project. The default login information is as follows: 
> ##### username: admin@admin.com
> ##### password: admin.

### Services & Hubs 

Now, onto the main dish: MagicOnion treats C# interfaces as a protocol schema, enabling seamless code sharing between C# projects without the need for .proto files.

I've inherited from these interfaces and implemented new ones. On both the client and server sides, they provide us with generic boilerplate code for CRUD operations, logging generic queries, streaming, and more. Of course, we can still use regular MagicOnion interfaces alongside the ones we've implemented.



#### Services
In magiconion this is how we create our service schema

```csharp
public interface IMyFirstService : IService<IMyFirstService>
{
    // The return type must be `UnaryResult<T>` or `UnaryResult`.
    UnaryResult<int> SumAsync(int x, int y);
}
```

When dealing with hundreds of tables, maintaining all these services with CRUD operations and data manipulation methods can be quite painful.
That's where generics come to our aid.

We have two types of interfaces for services: IMagicService<TService, TModel> and ISecureMagicService<TService, TModel>.
 
Both of these services feature the following method signatures:

* CreateAsync: Used to create a new instance of the specified model.
* FindByParentAsync: Used to retrieve a list of models based on a parent's primary key request.
* FindByParametersAsync: Used to retrieve a list of models based on given parameters.
* ReadAsync: Used to retrieve all models.
* StreamReadAllAsync: Used to retrieve all models in batches.
* UpdateAsync: Used to update the specified model.
* DeleteAsync: Used to delete the specified model.

##### ISecureMagicService additionaly contains the following method signatures:

* CreateEncrypted: Creates a new instance of the specified model using encrypted data.
* ReadEncrypted: Retrieves all models using encrypted data.
* UpdateEncrypted: Updates the specified model using encrypted data.
* DeleteEncrypted: Deletes the specified model using encrypted data.
* FindByParentEncrypted: Retrieves a list of encrypted data items of a specified model type that are associated with a parent.
* FindByParametersEncrypted: Retrieves a list of models based on given parameters.
* StreamReadAllEncypted: Streams and reads encrypted data items of a specified model type in batches.

 Example Implementation:

```csharp
public interface IUserService : ISecureMagicService<IUserService, USERS>// Where USERS is our DatabaseModel
{
  
}
```
or
```csharp
public interface IUserService : IMagicService<IUserService, USERS>// Where USERS is our DatabaseModel
{
  
}
```
#### Hubs

In magiconion this is how we create our Hub schema

```csharp
// Server -> Client definition
public interface IGamingHubReceiver
{
    // The method must have a return type of `void` and can have up to 15 parameters of any type.
    void OnJoin(Player player);
    void OnLeave(Player player);
    void OnMove(Player player);
}

// Client -> Server definition
// implements `IStreamingHub<TSelf, TReceiver>`  and share this type between server and client.
public interface IGamingHub : IStreamingHub<IGamingHub, IGamingHubReceiver>
{
    // The method must return `ValueTask`, `ValueTask<T>`, `Task` or `Task<T>` and can have up to 15 parameters of any type.
    ValueTask<Player[]> JoinAsync(string roomName, string userName, Vector3 position, Quaternion rotation);
    ValueTask LeaveAsync();
    ValueTask MoveAsync(Vector3 position, Quaternion rotation);
}
```

Instead we will now inherit from IMagicHub<THub, TReceiver, TModel> and  IMagicReceiver<TModel> which comes with following method signatures:

* ConnectAsync: Connects the client to the hub asynchronously.
* CreateAsync: Creates a new model on the server asynchronously.
* ReadAsync: Reads all models from the server asynchronously.
* StreamReadAsync: Streams models from the server asynchronously with the specified batch size.
* UpdateAsync: Updates an existing model on the server asynchronously.
* DeleteAsync: Deletes an existing model on the server asynchronously.
* FindByParentAsync: Retrieves a list of models based on the parent's primary key request.
* FindByParametersAsync: Retrieves a list of models based on given parameters.
* CollectionChanged: Notifies the clients when the collection of models changes on the server.
* KeepAliveAsync: Sends a keep-alive message to the server asynchronously.


Example Implementation:

```csharp
public interface ITestHub : IMagicHub<ITestHub, ITestHubReceiver, TestModel>
{
}

public interface ITestHubReceiver : IMagicReceiver<TestModel>
{
}
```

That's all with MagicT.Shared Section!





> [!IMPORTANT]
> When the server project is run, each service will be stored as a role in the ROLES table and each service method that can be called from the client side will be stored in the PERMISSIONS table and will be part of the Created Role.
> you can than assign the Role or prefered permissions to an User

 





# Magic Onion Generic Template

This is a plug-and-play MagicOnion template with generic service and hub implementations that use MemoryPack serialization.
Focused on performance and security this template introduces a built-in rate limiter using Redis. This limiter serves as a robust defense against Denial of Service (DoS) attacks and guards against resource depletion.
The template also integrates advanced encryption techniques like Diffie-Hellman and AES-GCM to secure end-to-end encryption and effective prevention of token theft. In parallel, it streamlines development by providing standard Create, Read, Update, and Delete (CRUD) operations via the services and hubs components, thereby expediting the development lifecycle.

## Let's Connect!
I appreciate every star ⭐ that my projects receive, and your support means a lot to me! If you find my projects useful or enjoyable, please consider giving them a star.


## Package Installation & Initial Configuration

You can install this template using [NuGet](https://www.nuget.org/packages/MagicOnionGenericTemplate):

```powershell
dotnet new install MagicOnionGenericTemplate
```
For template help

```powershell
dotnet new magic-onion-generic-h
```

By default, the project is created on **.NET 7.0** and gRPC connections are configured to use **SSL**

```powershell
dotnet new magic-onion-generic -n YourProjectName
```

Alternatively, you can disable SSL configuration with:

```powershell
dotnet new magic-onion-generic -n YourProjectName -F net7.0 -S false
```

> [!IMPORTANT]
> **Enviromental Setup**
> If your development environment is on a macos, the ssl configuration will not work due to the lack of ALPN support on mac. 
> See issue [here](https://github.com/grpc/grpc-dotnet/issues/416)
> Mac Users should also Comment the below from appsettings.json
> ```xml
> "HTTPS": {
>        "Url": "https://localhost:7197",
>        "Protocols": "Http2"
>  },
>```
> **Before running the project** 
>  * Make sure redis server is running on localhost:6379 (Or you can change it from appsettings.json file both in Web & Server Projects)
>  * Create a new migration and update database but before that in the server project, you must 
>  * Set your connection string in the appsettings.json file
>  * In program.cs change the below section according to your Database preference, by default the template uses Sql Database but suports MySql and Oracle Databases without any additional configuration. 
> ```csharp builder.Services.AddDbContext<MagicTContext>(
>    options => options.UseSqlManager (
>        builder.Configuration.GetConnectionString(nameof(MagicTContext)),
>         ServerVersion.AutoDetect(builder.Configuration.GetConnectionString(nameof(MagicTContext)))
>     )
> );
> ```


## Quick Intro

When using MagicOnion to create a protocol schema, we typically inherit from **IService<>**. However, with this template, we inherit from **IMagicService<>** or **IMagicSecureService** to leverage additional service methods. The service methods available in these interfaces can be found [here](#method-signatures).

**Important:** When using **IMagicService** or **IMagicSecureService**, it is crucial to inherit accordingly on both the client and server sides.

* If your interface inherits from **IMagicService**, the client side must inherit from **MagicClientService**, and the server side must inherit from **MagicServerService**.
* If your interface inherits from **IMagicSecureService**, the client side must inherit from **MagicClientSecureService**, and the server side must inherit from **MagicServerSecureService**.

This distinction is necessary because **IMagicSecureService** inherits from **IMagicService** and handles sending the service token to the server. The server needs to be able to read the token. Failing to follow this rule will result in either the inability to send the token from the client side or the inability to read the token on the server side, leading to exceptions.

### Secure Communication Workflow Between App and Server

1. The app and server starts, creates **Private** and **Public** keys then using the **KeyExchangeService** they share their public keys and using their private keys they each create a **Shared Key** to Encrypt/Decrpyt Data [See Here](#cryptography)
2. User signs in ([Credentials](#credentials))
3. User makes a call to the Secure Service
5. The Authorization Filter on the client side encrypts the username and token using the **Shared Key**, then adds them to the gRPC metadata
6. The Authorization Filter on the server side decrypts the encrypted data in the gRPC metadata, validates the token, and then determines whether the user can or cannot call the service.


### Class Hierarchy

**Client Side:**

MagicClientSecureService -> MagicClientService -> MagicClientServiceBase -> IService

* **MagicClientSecureService:** Implements **IMagicSecureService** (requires token) and uses **AuthorizationFilter**
* **MagicClientService:** Implements **IMagicService**.
* **MagicClientServiceBase:** Implements **IService** methods, SSL configurations, and connects to the server using the endpoint from **appsettings.json**.

**AuthorizationFilter:** Adds token to MetaData before server calls ([MagicOnion Client Filters](https://github.com/Cysharp/MagicOnion?tab=readme-ov-file#clientfilter)).
 
**Server Side:**
MagicServerSecureService -> AuditDatabaseService -> MagicServerService -> DatabaseService -> MagicServerBase

* **MagicServerSecureService:** Implements IMagicSecureService with token validation.
* **AuditDatabaseService:** Handles user-level logging.
* **MagicServerService:** Structural purpose, no specific functionality.
* **DatabaseService:** Implements IMagicService.
* **MagicServerBase:** Implements Generic Higher-Order Functions that Handles exceptions, transactions, and logging.

Enough, show me the code!

#### Shared Project

Step 1
```csharp

// By inheriting from IMagicService instead of IService, we can utilize the methods implemented in IMagicService which I provided more
// Information 
 public interface IUserService : IMagicService<IUserService, USERS> 
{
  
}
```
#### Client Project

Step 2
```csharp
//The [RegisterScoped] attribute is provided by a source generator library that generates boilerplate code for Dependency Injection. You can find a //detailed walkthrough in the documentation.
//I've mentioned this attribute, shared the repository link, and provided more information in the MagicT.Shared section

[RegisterScoped]
public sealed class UserService : MagicClientervice<IUserService, USERS>, IUserService
{
    public UserService(IServiceProvider provider) : base(provider)
    {
    }
}
```

#### Server Project

Step 3

```csharp
public sealed partial class UserService : MagicServerService<IUserService, USERS, MagicTContext>, IUserService
{
    public UserService(IServiceProvider provider) : base(provider)
    {
    }
}
```

Now you are ready to inject and call the services!

 
### Credentials

Admin users can be configured through the appsettings.json file in the server project. The default login information is as follows: 
* **Username**: admin@admin.com
* **Password**: admin.

for the WebTemplate project you can login at /admin/Login

### Swagger
Swagger runs on : **https://localhost:5028/Swagger/index.html**

<br></br>

 **From this point on, for simplicity, the tutorial will examine what each project is and the technologies used in each project. The projects are:**

 
#### 1. MagicT.Shared - Has Shared classes, extension methods, service and hub shema definitions
#### 2. MagicT.Redis - Has Redis Setup, TokenCacheService, RateLimiterService and ClientBlockerService implementations.
#### 3. MagicT.Web.Shared - Has shared base classes for MagicT.Web and MagicT.WebTemplate
#### 4. MagicT.Web/ MagicT.WebTemplate - MagicT.Web is a Blazor Server assembly project where as MagicT.WebTemplate is an MVC Core project which is configured to render blazor components 
#### 5. MagicT.Client - Has MagicOnion Client Service, Hub and Filter Implementations, LoginManager and StorageManager
#### 6. MagicT.Server - (Will update later)

<br/><br/>

## MagicT.Shared
The Shared project is referenced by other projects and consists of models, interfaces, hub implementations, extension methods, and other helpers.

###  3rd Party Libraries in this project
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

<br/><br/>  

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




### Extension Methods

 I have made some extension methods for different use cases, you can find them in the extensions folder.
 these methods are both used in  web, client and server projects. The code is readable and well-documented, and you can find references to where the methods are called.  
 Therefore, I don't feel the need to explain what each extension method does.

### Cryptography 
 in the **CryptoHelper.cs** class we have Encrypt and Decrypt methods that we will be using for end to end encryption using diffie-hellman key exchange.

### Managers
  1. LogManager - We will be using the LogManager to log users' interactions with services and to log error messages.
  2. KeyExchangeManager - We will be using the KeyExchangeManager share public keys between the Client and Server when the app starts.
     the public keys will be used to create a shared secret key independently on both Client and Server side. 
     these shared secret keys will be used when Encrypting and Decrypting data both in client and server side.
     you can read more about Diffie-Hellman Keyexchange in [here](https://davidtavarez.github.io/2019/implementing-elliptic-curve-diffie-hellman-c-sharp/)
     or simply google it.


### Models
I have implemented some classes so that when you create and run migrations, they will become tables in the database.

These classes are responsible for storing traces of user actions (audit/history) such when a user performs a CRUD operation, the logs will be stored in these tables.

I have also created classes that hold user data, permissions, and roles. Within these classes, you can assign permissions to roles and roles to users. Additionally, permissions can be independently assigned to the users.

We will review this in the web project where we have views for these tables.

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

We have two types of interfaces for services: **IMagicService<TService, TModel>** and **ISecureMagicService<TService, TModel>**.

##### Method Signatures
Both of these services feature the following method signatures:

* **CreateAsync:** Used to create a new instance of the specified model.
* **FindByParentAsync:** Used to retrieve a list of models based on a parent's primary key request.
* **FindByParametersAsync:** Used to retrieve a list of models based on given parameters.
* **ReadAsync:** Used to retrieve all models.
* **StreamReadAllAsync:** Used to retrieve all models in batches.
* **UpdateAsync:** Used to update the specified model.
* **DeleteAsync:** Used to delete the specified model.

 **ISecureMagicService** additionaly contains the following method signatures:

* **CreateEncrypted:** Creates a new instance of the specified model using encrypted data.
* **ReadEncrypted:** Retrieves all models using encrypted data.
* **UpdateEncrypted:** Updates the specified model using encrypted data.
* **DeleteEncrypted:** Deletes the specified model using encrypted data.
* **FindByParentEncrypted:** Retrieves a list of encrypted data items of a specified model type that are associated with a parent.
* **FindByParametersEncrypted:** Retrieves a list of models based on given parameters.
* **StreamReadAllEncypted:** Streams and reads encrypted data items of a specified model type in batches.

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

Instead we will now inherit from **IMagicHub<THub**, TReceiver, TModel> and  **IMagicReceiver<TModel>** which comes with following method signatures:

* **ConnectAsync**: Connects the client to the hub asynchronously.
* **CreateAsync:** Creates a new model on the server asynchronously.
* **ReadAsync:** Reads all models from the server asynchronously.
* **StreamReadAsync:** Streams models from the server asynchronously with the specified batch size.
* **UpdateAsync:** Updates an existing model on the server asynchronously.
* **DeleteAsync:** Deletes an existing model on the server asynchronously.
* **FindByParentAsync:** Retrieves a list of models based on the parent's primary key request.
* **FindByParametersAsync:** Retrieves a list of models based on given parameters.
* **CollectionChanged:** Notifies the clients when the collection of models changes on the server.
* **KeepAliveAsync:** Sends a keep-alive message to the server asynchronously.


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



## MagicT.Redis
This project is referenced by the Client and Server project and includes services for rate limiting, token caching, and IP blocking. However, the key class in this project is MagicTRedisDatabase. This class initializes a new instance of the MagicTRedisDatabase class with the specified configuration and includes the following methods:

* **Create<T>(string key, T value, TimeSpan? expiry = null):** Creates a new entry in the Redis database with the specified key and value, and an optional expiration time.

* **AddOrUpdate<T>(string key, T value, TimeSpan? expiry = null):** Adds a new entry or updates an existing entry in the Redis database with the specified key and value, and an optional expiration time.

* **ReadAs<T>(string key):** Retrieves the value associated with the specified key from the Redis database. Returns the value or null if the key is not found.

* **Update<T>(string key, T newValue, TimeSpan? expiry = null):** Updates the value of an existing entry in the Redis database with the specified key. If the key does not exist, no action is taken.

* **Delete<T>(string key):** Deletes the key-value pair associated with the specified key from the Redis database.

* **Push<T>(string key, T value):** Adds an element to the end of a list stored at the specified key in the Redis database.

* **PullAs<T>(string key):** Retrieves all elements from a list stored at the specified key in the Redis database. Returns an array of elements. 



## MagicT.Web.Shared
This project implements base classes that inherit from the Components base and are responsible for handling CRUD requests from the view and projecting exceptions/errors onto the view.
There are four base classes that our Razor pages will inherit from: PageBase, ServicePageBase, SecuredServicePageBase, and HubPageBase. Additionally, this project includes some TaskExtensions to better manage service calls.

### PageBase: 
is an abstract class designed to serve as a base class for Blazor components. It integrates several key services and provides methods to handle asynchronous and synchronous tasks with error handling and notifications. Here is a summary of its main functionalities:

#### 1. Service Integration:

* **IDialogService:** Manages dialog interactions.
* **ISnackbar:** Displays transient messages.
* **NotificationsView:** Handles and displays notifications.
* **NavigationManager:** Manages navigation within the application.

#### 2. Initialization:

The **OnInitializedAsync** method initializes the notifications view and calls the **OnBeforeInitializeAsync** method, which can be overridden by derived classes to perform additional initialization tasks.
Task Execution with Error Handling:

#### 3. Task Execution with Error Handling:
* **ExecuteAsync<T>(Func<Task<T>> task):** Executes an asynchronous task and returns its result, adding any errors to the notifications.
* **ExecuteAsync(Func<Task> task):** Executes an asynchronous task without a return value, adding any errors to the notifications.
* **Execute<T>(Func<T> task):** Executes a synchronous task and adds any errors to the notifications.

#### 4. Error Handling:
 
* The error handling in the task execution methods captures exceptions such as **RpcException**, **AuthenticationException**, and **JSDisconnectedException**, adding appropriate messages to the notifications and triggering the notifications view if any errors are recorded.

 
This class streamlines the integration of common services and error handling for Blazor components, promoting code reuse and consistency across the application.


### ServicePageBase :
The ServicePageBase class provides a base implementation for Blazor pages that interact with a service to manage data models. It extends the PageBaseClass and adds functionality for handling CRUD (Create, Read, Update, Delete) operations and file uploads. The class supports generic data models and services, allowing for flexible use with different types of data. Here's a detailed breakdown:

##### Main Class: ServicePageBase<TModel, TService>

###### 1. Generic Parameters:

* **TModel:** The type of the data model.
* **TService:** The type of the service that manages the data model, implementing IMagicService<TService, TModel>.

###### 2. Properties and Dependencies:

* **Grid:** Component reference.
* **View:** Component reference.
* **File:** Interface for handling file uploads.
* **Service:** Injected service instance.
* **DataSource:** List of data models.
* **Subscriber:** Service for subscribing to various operations.
 
###### 3. Initialization:

* The **OnBeforeInitializeAsync** method sets up subscribers for CRUD operations and streams, ensuring the component state updates on these operations.

###### 4. CRUD Operations:

* **CreateAsync:** Creates a new model instance and adds it to the data source.
* **ReadAsync:** Reads data models from the service and updates the data source.
* **UpdateAsync:** Updates an existing model instance and modifies the data source.
* **DeleteAsync:** Deletes a model instance after user confirmation and removes it from the data source.
* **FindByParametersAsync:** Searches for data models based on specified parameters and updates the data source.

  
###### 5. Utility Methods:

* **Cancel:** Reverts changes to a model instance.
* **LoadAsync:** Prepares the view for loading data.
* **OnFileUpload:** Handles file upload events.
* **ReadFileAsBytes:** Reads a file and returns its contents as a byte array.


##### Derived Class ServicePageBase<TModel, TChild, TService>


###### 1. Additional Generic Parameters:

* **TChild:** The type of the child model related to the parent model.
###### 2. Additional Properties:

* **ParentModel:** The parent data model.
###### 3. Extended Initialization:

* Overrides **OnBeforeInitializeAsync** to also call FindByParentAsync.
###### 4. Extended Methods:

* **LoadAsync:** Validates the parent model before proceeding with view loading.
* **CreateAsync:** Ensures the child model is related to the parent model.
* **FindByParentAsync:** Finds child models related to the parent model and updates the data source.

### HubPageBase :
The HubPageBase class provides a base implementation for Blazor pages that interact with SignalR hubs to manage data models. This class, which extends PageBaseClass, facilitates real-time data operations through SignalR hubs and includes support for CRUD operations, error handling, and data synchronization. Here's a detailed breakdown:

##### Main Class: HubPageBase<THub, ITHub, THubReceiver, TModel>

###### 1. Generic Parameters:

* **TModel:** The type of the data model.
* **THub:** The type of the hub client, inheriting from MagicHubClientBase<ITHub, THubReceiver, TModel>.
* **ITHub:** The type of the hub interface, implementing IMagicHub<ITHub, THubReceiver, TModel>.
* **THubReceiver:** The type of the hub receiver, implementing IMagicReceiver<TModel>.

###### 2. Properties and Dependencies:

* **Grid:** Component Reference.
* **View:** Component Reference.
* **IService:** Injected hub service instance.
* **DataSource:** List of data models managed by the hub service.
* **Subscriber:** Service for subscribing to model operations.
* **ListSubscriber:** Service for subscribing to list operations.

###### 3. Initialization:

* The **OnInitializedAsync** method sets up subscribers for CRUD and stream operations, ensuring the component state updates accordingly.

###### 4. CRUD Operations:

* **CreateAsync:** Creates a new model instance via the hub service and adds it to the data source.
* **ReadAsync:** Reads data models from the hub service and updates the data source.
* **UpdateAsync:** Updates an existing model instance via the hub service and modifies the data source.
* **DeleteAsync:** Deletes a model instance after user confirmation via the hub service and removes it from the data source.
* **FindByParametersAsync:** Searches for data models based on specified parameters and updates the data source.
  
###### 5. Utility Methods:

* **Cancel:** Reverts changes to a model instance.
* **LoadAsync:** Prepares the view for loading data.

  
##### Derived Class: HubPageBase<THub, ITHub, THubReceiver, TModel, TChild>

###### 1. Additional Generic Parameters:

* **TChild:** The type of the child model related to the parent model.

###### 2. Additional Properties:

* **ParentModel:** The parent data model.

###### 3. Extended Initialization:

* Overrides **OnBeforeInitializeAsync** to also call FindByParentAsync.

###### 4. Extended Methods:

CreateAsync: Ensures the child model is related to the parent model by setting the foreign key.
FindByParentAsync: Finds child models related to the parent model via the hub service and updates the data source.


##### Task Extensions
The TaskExtensions class provides extension methods for handling the completion of asynchronous tasks in C#. These methods allow you to execute additional actions or functions before and after the task completes, making it easier to manage task results and handle errors consistently. Here's a breakdown of each method:


###### 1. Methods

* **OnComplete<T>(this Task<T> task, Func<T, TaskResult, Task<T>> func)**
  Description: Executes an asynchronous function before and after the task completes.
  1. **Parameters:**
  2. **task:** The asynchronous task to monitor.
  3. **func:** A function that processes the task result and status, and returns a new task result.
  4. **Returns:** The result of the original task.
  5. **Usage:** Use this method when you need to perform additional asynchronous operations based on the task result and status.
           
* **OnComplete<T>(this Task<T> task, Action<T, TaskResult> action)**
  Description: Executes an action before and after the task completes.
  1. **Parameters:**
  2. **task:** The asynchronous task to monitor.
  3. **action:** An action that processes the task result and status.
  4. **Returns:** The result of the original task.
  5. **Usage:** Use this method when you need to perform additional synchronous operations based on the task result and status.
 
* **OnComplete<T, TArg>(this Task<T> task, Action<T, TaskResult, TArg> action, TArg arg)**
  Description: Executes an action with an additional argument before and after the task completes.
  1. **Parameters:**
  2. **task:** The asynchronous task to monitor.
  3. **action:** An action that processes the task result, status, and an additional argument.
  4. **arg:** The additional argument to pass to the action.
  5. **Returns:** The result of the original task.
  6. **Usage:** Use this method when you need to perform additional synchronous operations with an extra argument based on the task result and status.
 
* **OnComplete<T>(this Task<T> task, Action<TaskResult> action)**
  Description: Executes an action before and after the task completes, without handling the result data.
  1. **Parameters:**
  2. **task:** The asynchronous task to monitor.
  3. **action:** An action that processes the task status.
  4. **Returns:** The result of the original task.
  5. **Usage:** Use this method when you need to perform additional synchronous operations based only on the task status.

**Common Behavior**
Task Result Handling: Each method captures the task result and determines its status (success or fail). If the task completes successfully and returns a non-null result, the status is TaskResult.Success. If the result is null or an exception occurs, the status is TaskResult.Fail.

**Action/Function Invocation:** The specified action or function is invoked with the task result and status. This allows you to handle task completion logic in a consistent manner.

**Return Value:** The methods return the original task result, allowing you to continue using the result in your application logic.

<br></br>

## MagicT.Web & MagicT.WebTemplate
    These two web projects both reference the MagicT.Shared and MagicT.Web.Shared projects. The difference is that MagicT.Web is a Blazor Server assembly project, while MagicT.WebTemplate is an MVC Core project configured to render Blazor pages. I love Blazor, but for me, one of the biggest disadvantages is its incompatibility with some jQuery libraries. When using template pages that depend on jQuery, they do not function well. In these cases, I use the MVC Core project to apply the template and use the services in Blazor components so that I can have template UI with full functionality.

> [!Note]
> When running the WebTemplate project go to /Login page to switch to Blazor 
 

##  MagicT.Client & MagicT.Server 

The Client project implements classes that make calls to the API, and the server project implements endpoint services.

###  3rd Party Libraries in this Client Project
     1. LocalStorage - Gives access to browsers local storage
        Use Case: Storing UserCredentials etc.
        Repository link: https://github.com/Blazored/LocalStorage
        
###  3rd Party Libraries in this Server Project
     1. AQueryMaker - This is another library of mine, It simplifies Database Operations
        Use Case: Executing custom queries
        Repository link:  https://github.com/licentia88/AQueryMaker

    2. Coravel - Simplifies task/job scheduling, queuing, caching
        Use Case: Queuing background tasks
        Repository link: https://github.com/jamesmh/coravel

    3. LitJwt - Another Cysharp framework
        Use Case: Create LightWeight Tokens
        Repository link: https://github.com/jamesmh/coravel

        
 
## Contributing
  * You can contribute by implementing new ideas and features to the project
  * You can help contributing with Unit Testing
  * Documentation

Fork the repository

Create a new branch **(git checkout -b feature-branch)**

Commit your changes **(git commit -m 'Add some feature')**

Push to the branch **(git push origin feature-branch)**

Open a pull request

License
This project is licensed under the MIT License

 





# Magic Onion Generic Template

A plug n play magiconion template with generic service and hub implementation that uses memorypack serialization
 
## Package Installation

You can install this template using [NuGet](https://www.nuget.org/packages/MagicOnionGenericTemplate):

```powershell
dotnet new install MagicOnionGenericTemplate
```

```powershell
dotnet new magic-onion-generic -n YourProjectName
```

 ### The Template comes with the following projects
 
 #### Client 
 
 has Service base, Hub Base, service , hub and filter implementations. 
 
 ##### Filters: 
 
 ##### AuthenticationFilter: When used, adds the binary token to the grpc callcontext before each request.
 
 ##### Note: You might need to reconfigure how to store token and fetch the toke to use in the filter

 ##### RateLimiterFilter: Ratelimiting Rules before making a request. It can be configured from appsettings.json
 
 ##### Note: To store soft/hard blocked Ips, I've used Redis, you can make your own implementation if you do not want to use redis.

 
 #### Redis
 has Ratelimiter, ClientBlocker and TokenCache service implementations. These services are only implemented in the clientside but you 
 can easily implement them on the server side as well
 ##### Note: I've used localstorage for token storing, and though it is implemented I have not used TokenCaching with redis in this project.
 ##### Note: Rate limiting and blocking rules can be defined in appsettings.json
 
 #### Server
 has Serverside implementation of generic hubs and services.
 Services and hubs has efdbcontext extensions. However ef does not support custom query executing therefore I've also implemented
 AQueryMaker: which is a library that helps easily executing custom query or Sp into a dictionary and has a fancy feature called 
 ExecuteStream which streams the query so you do not have to wait until query is completely executed. You could also use the ToModel 
 Extension to Convert the Dictionary into the desired poco model.

 I have additionally implemented users, roles etc classes and used MasterMemory for in memorystorage.
 Roles and Permissions are later used in serverside grpc filter after token validation to check if the user has required Roles or permissions to proceed
 

### Nuget Packages Used in this project?
 
 
 

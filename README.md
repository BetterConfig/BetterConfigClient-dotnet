# BetterConfig client for .NET
BetterConfig integrates with your products to allow you to create and configure apps, backends, websites and other programs using an easy to follow online User Interface (UI).
https://betterconfig.com  

[![Build status](https://ci.appveyor.com/api/projects/status/lbvu9ttawoioaprg?svg=true)](https://ci.appveyor.com/project/BetterConfig/betterconfigclient-dotnet) [![NuGet Version](https://buildstats.info/nuget/BetterConfigClient)](https://www.nuget.org/packages/BetterConfigClient/)
## Getting Started

 1. Install [NuGet](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) package: [BetterConfigClient](https://www.nuget.org/packages/BetterConfigClient)
 ```PowerShell
 Install-Package BetterConfigClient
 ```
 2. Get your Project token from [BetterConfig.com](https://betterconfig.com) portal:
![YourConnectionUrl](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/media/readme01.png  "YourProjectToken")

 3. Create a **BetterConfigClient** instance:
```c#
var betterConfigClient = new BetterConfigClient("#YOUR-PROJECT-TOKEN#");
```
 4. Get your config value:
```c#
var myStringValue = betterConfigClient.GetValue("myStringKey", String.Empty);
Console.WriteLine("My String value from BC: {0}", myStringValue);
```
## Configuration
You can configure the client with ```BetterConfigConfiguration``` object.
Configuration parameters are the followings:

| PropertyName        | Description           | Default  | Required |
| --- | --- | --- | --- |
| ```ProjectToken```      | Project token to access your configuration  | - | YES |
| ```TimeToLiveSeconds```      | Cache time to live in seconds      |   2 | - |
| ```TraceFactory``` | Factory method to create an ```ITraceWriter``` instance for tracing.        | ```NullTrace``` (no default tracing method) | - |
| ```TraceLevel```      | Specifies message filtering to output for the ```ITraceWriter```. Values: *Off*, *Error*, *Warn*, *Info*, *Verbose*      |   ```Error``` | - |

### Example
Increase TimeToLiveSeconds to 60 seconds
``` c#
var clientConfiguration = new BetterConfigClientConfiguration
{
	ProjectToken = "#YOUR-PROJECT-TOKEN#",
	TimeToLiveSeconds = 60
};

IBetterConfigClient betterConfigClient = new BetterConfigClient(clientConfiguration);
```


## License
[MIT](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/LICENSE)

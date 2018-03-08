

# BetterConfig client for .NET
BetterConfig integrates with your products to allow you to create and configure apps, backends, websites and other programs using an easy to follow online User Interface (UI).
https://betterconfig.com  

[![Build status](https://ci.appveyor.com/api/projects/status/lbvu9ttawoioaprg?svg=true)](https://ci.appveyor.com/project/BetterConfig/betterconfigclient-dotnet) [![NuGet Version](https://buildstats.info/nuget/BetterConfigClient)](https://www.nuget.org/packages/BetterConfigClient/)
## Getting Started

 1. Install [NuGet](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) package: [BetterConfigClient](https://www.nuget.org/packages/BetterConfigClient)
 ```PowerShell
 Install-Package BetterConfigClient
 ```
 2. Get your Project secret from [BetterConfig.com](https://betterconfig.com) portal:
![ProjectSecret](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/media/readme01.png  "ProjectSecret")

 3. Create a **BetterConfigClient** instance:
```c#
var betterConfigClient = new BetterConfig.BetterConfigClient("#YOUR-PROJECT-SECRET#");
```
 4. Get your config value:
```c#
var isMyAwesomeFeatureEnabled = betterConfigClient.GetValue("isMyAwesomeFeatureEnabled", false);

if(isMyAwesomeFeatureEnabled)
{
    //show your awesome feature to the world!
}
```

## Configuration
Client supports two different modes to acquire the latest configuration from the betterconfig.

### Polling mode (default)
Client downloads the latest configuration and puts into a cache repeatedly. You can subscribe to an event ```OnConfigurationChanged``` to get notification about configuration changes. Set ```AutoPollingEnabled``` to false to disable this mode (or set ```PollingIntervalSeconds``` to zero.)

### Lazy loading mode
Client downloads the latest configuration only when it is not present or expired (use CacheTimeToLiveSeconds to manage configuration expire).


---
You can configure the client with ```BetterConfigConfiguration``` object.
Configuration parameters are the followings:

| PropertyName        | Description           | Default  | Required |
| --- | --- | --- | --- |
| ```ProjectSecret```      | Project secret to access your configuration  | - | YES |
| ```CacheTimeToLiveSeconds```      | When the client running in LazyLoadingMode use this value to cache configuration. (In the Polling mode this setting is invalid)|   60 | - |
| ```PollIntervalSeconds ```      | Polling interval (in LazyLoading mode this is invalid)|   60 | - |
| ```MaxInitWaitTimeSeconds```      | Maximum waiting time between the client initialization and the first config acquisition in secconds. (in LazyLoading mode this is invalid)|   5 | - |
| ```LoggerFactory``` | Factory to create an `ILogger` instance for tracing.        | `NullTrace` (no default tracing method) | - |
| ```AutoPollingEnabled``` | Enable/Disable polling mode | ```true``` | -

### Example - increase CacheTimeToLiveSeconds to 60 seconds
``` c#
var clientConfiguration = new BetterConfigClientConfiguration
{
	ProjectToken = "#YOUR-PROJECT-SECRET#",
	CacheTimeToLiveSeconds = 60
};

IBetterConfigClient betterConfigClient = new BetterConfigClient(clientConfiguration);
```
### Example - default value handling
You can easily to manage default values with this technique when you use your configuration in many locations in the code.
``` c#
var betterConfigClient = new BetterConfig.BetterConfigClient("#YOUR-PROJECT-SECRET#");

bool isMyAwesomeFeatureEnabled = betterConfigClient.GetConfiguration(MyApplicationFeatureConfig.Default).MyNewFeatureEnabled;

if (isMyAwesomeFeatureEnabled)
{
	//show your awesome feature to the world!
}

internal sealed class MyApplicationFeatureConfig
{
	public static readonly MyApplicationFeatureConfig Default = new MyApplicationFeatureConfig
		{
			// set my default values here
			MyNewFeatureEnabled = false
		};

	public bool MyNewFeatureEnabled { get; set; }
}
```
You can customize setting with [```Newtonsoft.Json.JsonProperty```](https://www.newtonsoft.com/json/help/html/JsonPropertyName.htm):
``` c#
[JsonProperty("My_New_Feature_Enabled")]
public bool MyNewFeatureEnabled { get; set; }
```

## Members
### Methods
| Name        | Description           |
| :------- | :--- |
| ``` GetValue<T>(string key, T defaultValue) ``` | Return a value of the key |
| ``` ForceRefresh() ``` | Fetch the latest configuration from the server. You can use this method with WebHook to ensure up to date configuration values in your application. ([see ASP.Net sample project to use webhook for cache invalidation](https://github.com/BetterConfig/BetterConfigClient-dotnet/blob/master/samples/ASP.NETCore/WebApplication/Controllers/BackdoorController.cs)) |
| ``` GetConfigurationJsonString() ``` | Return configuration as a json string |
| ``` T GetConfiguration<T>(T defaultValue) ``` | Serialize the configuration to a passed **T** type. You can customize your **T** with Newtonsoft attributes |
### Events
| Name        | Description           |
| :------- | :--- |
| ``` OnConfigurationChanged ``` | Occurs when the configuration changed (in LazyLoading mode this is invalid) |


## Lifecycle of the client
We're recommend to use client as a singleton in your application. Today you can do this easily with any IoC contanier ([see ASP.Net sample project](https://github.com/BetterConfig/BetterConfigClient-dotnet/blob/master/samples/ASP.NETCore/WebApplication/Startup.cs#L24)).
### Dispose
To ensure gracefull shutdown of the client you should use ```.Dispose()``` method.
 
## Tracing
The client doesn't use any external logging framework. If you want to add your favourite logging library you have to create an adapter to ```ILogger``` and setup a logger factory in ```BetterConfigConfiguration```.

## License
[MIT](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/LICENSE)
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
5. On application exit:
``` c#
betterConfigClient.Dispose();
```

## Configuration
Client supports three different modes to acquire the configuration from the betterconfig. When the client downloads the latest configuration puts into the internal cache and it serves any settings acquisition from cache. With these modes you can manage settings' lifetimes easily.

### Auto polling mode (default)
Client downloads the latest configuration and puts into a cache repeatedly. You can subscribe to an event ```OnConfigurationChanged``` to get notification about configuration changes. (use ```PollingIntervalSeconds``` parameter to manage polling interval.

### Lazy loading mode
Client downloads the latest configuration only when it is not present or expired (use ```CacheTimeToLiveSeconds``` to manage configuration lifetime).

### Manual polling mode
With this mode you always have to invoke ```.ForceRefresh()``` method to download a latest configuration into the cache. When the cache is empty (for example after client initialization) and try to acquire any value you'll get the default value!

---

Configuration parameters are different in each mode:
### Base configuration
| PropertyName        | Description           | Default  |
| --- | --- | --- |
| ```ProjectSecret```      | Project secret to access your configuration  | REQUIRED |
| ```LoggerFactory``` | Factory to create an `ILogger` instance for tracing.        | `NullTrace` (no default tracing method) | 
### Auto polling
| PropertyName        | Description           | Default  |
| --- | --- | --- |
| ```PollIntervalSeconds ```      | Polling interval|   60 | 
| ```MaxInitWaitTimeSeconds```      | Maximum waiting time between the client initialization and the first config acquisition in secconds.|   5 |
### Lazy loading
| PropertyName        | Description           | Default  |
| --- | --- | --- | 
| ```CacheTimeToLiveSeconds```      | When the client running in LazyLoadingMode use this value to cache configuration.|   60 |

### Example - increase CacheTimeToLiveSeconds to 60 seconds
``` c#
var clientConfiguration = new BetterConfig.Configuration.LazyLoadConfiguration
            {
                ProjectSecret = "#YOUR-PROJECT-SECRET#",
                CacheTimeToLiveSeconds = 60
            };

IBetterConfigClient betterConfigClient = new BetterConfigClient(clientConfiguration);
```
### Example - OnConfigurationChanged 
In Auto polling mode you can subscribe an event to get notification about changes.
``` c#
 var betterConfigClient = new BetterConfigClient(projectSecret);

betterConfigClient.OnConfigurationChanged += (s, a) =>
{
	//Configuration changed. Update UI!
};
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
### Configuration with clientbuilder
It is possible to use ```BetterConfigClientBuilder``` to build BetterConfigClient instance:

``` c#
IBetterConfigClient client = BetterConfigClientBuilder
	.Initialize("YOUR-PROJECT-SECRET")
    .WithLazyLoad()
    .WithCacheTimeToLiveSeconds(120)
    .Create();
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
| ``` OnConfigurationChanged ``` | Only AutoPollOccurs when the configuration changed |


## Lifecycle of the client
We're recommend to use client as a singleton in your application. Today you can do this easily with any IoC contanier ([see ASP.Net sample project](https://github.com/BetterConfig/BetterConfigClient-dotnet/blob/master/samples/ASP.NETCore/WebApplication/Startup.cs#L24)).
### Dispose
To ensure gracefull shutdown of the client you should use ```.Dispose()``` method.
 
## Logging
The client doesn't use any external logging framework. If you want to add your favourite logging library you have to create an adapter to ```ILogger``` and setup a logger factory in ```BetterConfigConfiguration```.

## License
[MIT](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/LICENSE)

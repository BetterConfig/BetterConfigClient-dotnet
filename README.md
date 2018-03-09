# BetterConfig client for .NET
BetterConfig is a cloud based configuration as a service. It integrates with your apps, backends, websites, and other programs, so you can configure them through this website even after they are deployed.
https://betterconfig.com  

[![Build status](https://ci.appveyor.com/api/projects/status/lbvu9ttawoioaprg?svg=true)](https://ci.appveyor.com/project/BetterConfig/betterconfigclient-dotnet) [![NuGet Version](https://buildstats.info/nuget/BetterConfigClient)](https://www.nuget.org/packages/BetterConfigClient/)
## Getting Started

 1. Install [NuGet](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) package: [BetterConfigClient](https://www.nuget.org/packages/BetterConfigClient)
 ```PowerShell
 Install-Package BetterConfigClient
 ```
 2. Get your Project secret from [BetterConfig.com](https://betterconfig.com) portal:
![ProjectSecret](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/media/readme02.png  "ProjectSecret")

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
Client supports three different caching policies to acquire the configuration from BetterConfig. When the client downloads the latest configuration, puts it into the internal cache and serves any configuration acquisition from cache. With these caching policies you can manage your configurations' lifetimes easily.

### Auto polling (default)
Client downloads the latest configuration and puts into a cache repeatedly. Use ```PollingIntervalSeconds``` parameter to manage polling interval.
You can subscribe to the ```OnConfigurationChanged``` event to get notification about configuration changes. 

### Lazy loading
Client downloads the latest configuration only when it is not present or expired in the cache. Use ```CacheTimeToLiveSeconds``` parameter to manage configuration lifetime.

### Manual polling
With this mode you always have to invoke ```.ForceRefresh()``` method to download a latest configuration into the cache. When the cache is empty (for example after client initialization) and you try to acquire any value you'll get the default value!

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
| ```CacheTimeToLiveSeconds```      | Use this value to manage the cache's TTL. |   60 |

### Example - increase CacheTimeToLiveSeconds to 600 seconds
``` c#
var clientConfiguration = new BetterConfig.Configuration.LazyLoadConfiguration
            {
                ProjectSecret = "#YOUR-PROJECT-SECRET#",
                CacheTimeToLiveSeconds = 600
            };

IBetterConfigClient betterConfigClient = new BetterConfigClient(clientConfiguration);
```
### Example - OnConfigurationChanged 
In Auto polling mode you can subscribe an event to get notification about changes.
``` c#
var betterConfigClient = new BetterConfigClient(projectSecret);

betterConfigClient.OnConfigurationChanged += (s, a) => 
{
	  // Configuration changed. Update UI!
}
```
### Example - default value handling
You can easily manage default values with this technique when you use your configuration in many locations in the code.
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
You can customize deserialization settings with [```Newtonsoft.Json.JsonProperty```](https://www.newtonsoft.com/json/help/html/JsonPropertyName.htm):
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
    .Build();
```

## Members
### Methods
| Name        | Description           |
| :------- | :--- |
| ``` GetValue<T>(string key, T defaultValue) ``` | Returns the value of the key |
| ``` ForceRefresh() ``` | Fetches the latest configuration from the server. You can use this method with WebHooks to ensure up to date configuration values in your application. ([see ASP.Net sample project to use webhook for cache invalidation](https://github.com/BetterConfig/BetterConfigClient-dotnet/blob/master/samples/ASP.NETCore/WebApplication/Controllers/BackdoorController.cs)) |
| ``` GetConfigurationJsonString() ``` | Return configuration as a json string |
| ``` T GetConfiguration<T>(T defaultValue) ``` | Serialize the configuration to a passed **T** type. You can customize your **T** with Newtonsoft attributes |
### Events
| Name        | Description           |
| :------- | :--- |
| ``` OnConfigurationChanged ``` | Only with AutoPolling policy. Occurs when the configuration changed |


## Lifecycle of the client
We're recommend to use client as a singleton in your application. Today you can do this easily with any IoC contanier ([see ASP.Net sample project](https://github.com/BetterConfig/BetterConfigClient-dotnet/blob/master/samples/ASP.NETCore/WebApplication/Startup.cs#L24)).
### Dispose
To ensure gracefull shutdown of the client you should use ```.Dispose()``` method.
 
## Logging
The client doesn't use any external logging framework. If you want to add your favourite logging library you have to create an adapter to ```ILogger``` and setup a logger factory in ```BetterConfigConfiguration```.

## License
[MIT](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/LICENSE)

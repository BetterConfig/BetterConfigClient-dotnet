# BetterConfig client for .NET
BetterConfig integrates with your products to allow you to create and configure apps, backends, websites and other programs using an easy to follow online User Interface (UI).
https://betterconfig.com  

[![Build status](https://ci.appveyor.com/api/projects/status/lbvu9ttawoioaprg?svg=true)](https://ci.appveyor.com/project/BetterConfig/betterconfigclient-dotnet) [![NuGet Version](https://buildstats.info/nuget/BetterConfigClient)](https://www.nuget.org/packages/BetterConfigClient/)
## Getting Started

 1. Install [NuGet](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) package: [BetterConfigClient](https://www.nuget.org/packages/BetterConfigClient)
 ```PowerShell
 Install-Package BetterConfigClient
 ```
 2. Get your Project connection url from [BetterConfig.com](https://betterconfig.com) portal:
![YourConnectionUrl](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/media/readme01sml.png  "YourConnectionUrl")

 3. Create a **BetterConfigClient** instance:
```c#
var betterConfigClient = new BetterConfigClient("#YOUR-CONNECTION-URL#");
```
 4. Get your config value:
```c#
var myStringValue = betterConfigClient.GetValue("myStringKey", String.Empty);
Console.WriteLine("My String value from BC: {0}", myStringValue);
```

## License
[MIT](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/LICENSE)

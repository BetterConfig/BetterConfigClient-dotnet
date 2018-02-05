# BetterConfig client for .NET
BetterConfig integrates with your products to allow you to create and configure apps, backends, websites and other programs using an easy to follow online User Interface (UI).
https://betterconfig.com
## Getting Started

 1. Install [NuGet](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) package: [BetterConfigClient](https://www.nuget.org/packages/BetterConfigClient)
 `Install-Package BetterConfigClient`
 2. Get your Project connection url from BetterConfig.com portal:
![YourConnectionUrl](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/media/readme01sml.png  "YourConnectionUrl")

 3. Create an instance from BetterConfigClient:
 `var betterConfigClient = new BetterConfigClient("#YOUR-CONNECTION-URL#");`
 4. Get your config value:
`var myStringValue = betterConfigClient.GetValue("myStringKey", String.Empty);`
`Console.WriteLine("My String value from BC: {0}", myStringValue);`

## License
[MIT](https://raw.githubusercontent.com/BetterConfig/BetterConfigClient-dotnet/master/LICENSE)

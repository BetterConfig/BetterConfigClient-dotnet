using BetterConfig;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {            
            const string projectToken = "samples/01";            

            var betterConfigClient = new BetterConfigClient(new BetterConfigClientConfiguration
            {
                ProjectToken = projectToken
            });

            // current project's setting key name is 'keySampleText'
            var mySettingValue = betterConfigClient.GetValue("keySampleText", String.Empty);

            // 'myKeyNotExits' setting doesn't exist in the project configuration and the client returns default value ('N/A');
            var mySettingNotExists = betterConfigClient.GetValue("myKeyNotExits", "N/A");

            Console.WriteLine();
            Console.WriteLine(" 'mySettingValue' value from BetterConfig: {0}", mySettingValue);
            Console.WriteLine();
            Console.WriteLine(" 'mySettingNotExists' value from BetterConfig: {0}", mySettingNotExists);

            Console.WriteLine("\n\nPress any key(s) to exit...");
            Console.ReadKey();            
        }
    }
}

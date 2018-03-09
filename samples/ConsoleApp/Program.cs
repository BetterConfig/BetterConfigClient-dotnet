using BetterConfig;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {            
            const string projectSecret = "samples/01";

            var betterConfigClient = BetterConfigClientBuilder
                .Initialize(projectSecret)
                .WithAutoPoll()
                .WithMaxInitWaitTimeSeconds(10)
                .Build();

            // current project's setting key name is 'keyBool'            
            var myNewFeatureEnabled = betterConfigClient.GetValue("keyBool", false);

            // is my new feature enabled?
            if (myNewFeatureEnabled)
            {
                Console.WriteLine("Here is my new feature...");
                Console.WriteLine(betterConfigClient.GetValue("keyString", ""));
            }
            
            // 'myKeyNotExits' setting doesn't exist in the project configuration and the client returns default value ('N/A');
            var mySettingNotExists = betterConfigClient.GetValue("myKeyNotExits", "N/A");
            
            Console.WriteLine();
            Console.WriteLine(" 'mySettingNotExists' value from BetterConfig: {0}", mySettingNotExists);

            Console.WriteLine("\n\nPress any key(s) to exit...");
            Console.ReadKey();            
        }      
    }
}

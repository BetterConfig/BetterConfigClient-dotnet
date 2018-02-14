using System;

namespace BetterConfig
{
    internal struct Config
    {
        public readonly static Config Empty = new Config(null, DateTime.MinValue, null);

        public string JsonString { get; set; }

        public DateTime TimeStamp { get; set; }        

        public string HttpETag {get; set;}

        internal Config(string jsonString, DateTime timeStamp, string httpETag)
        {
            this.JsonString = jsonString;

            this.TimeStamp = timeStamp;            

            this.HttpETag = httpETag;
        }
    }
}
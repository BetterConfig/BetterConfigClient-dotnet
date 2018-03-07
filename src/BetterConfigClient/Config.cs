using System;
using System.Collections.Generic;

namespace BetterConfig
{
    internal struct Config : IEquatable<Config>
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

        public override bool Equals(object obj)
        {
            return this.Equals((Config)obj);
        }

        public bool Equals(Config other)
        {
            return this.HttpETag == other.HttpETag && this.JsonString == other.JsonString;
        }

        public override int GetHashCode()
        {
            var hashCode = 1098790081;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(JsonString);            
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HttpETag);

            return hashCode;
        }
    }
}
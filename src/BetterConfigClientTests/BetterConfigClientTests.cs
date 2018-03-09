using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BetterConfig;
using BetterConfig.Configuration;

namespace BetterConfigClientTests
{

    [TestClass]
    public class BetterConfigClientTests
    {
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void CreateAnInstance_WhenProjectSecretIsEmpty_ShouldThrowArgumentNullException()
        {
            string projectSecret = string.Empty;

            new BetterConfigClient(projectSecret);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void CreateAnInstance_WhenProjectSecretIsNull_ShouldThrowArgumentNullException()
        {
            string projectSecret = null;

            new BetterConfigClient(projectSecret);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void CreateAnInstance_WhenAutoPollConfigurationProjectSecretIsNull_ShouldThrowArgumentNullException()
        {
            var clientConfiguration = new AutoPollConfiguration
            {
                ProjectSecret = null
            };

            new BetterConfigClient(clientConfiguration);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void CreateAnInstance_WhenConfigurationProjectSecretIsEmpty_ShouldThrowArgumentNullException()
        {
            var clientConfiguration = new AutoPollConfiguration
            {
                ProjectSecret = string.Empty
            };

            new BetterConfigClient(clientConfiguration);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void CreateAnInstance_WhenLazyLoadConfigurationTimeToLiveSecondsIsZero_ShouldThrowArgumentOutOfRangeException()
        {
            var clientConfiguration = new LazyLoadConfiguration
            {
                ProjectSecret = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf",
                CacheTimeToLiveSeconds = 0
            };

            new BetterConfigClient(clientConfiguration);
        }        

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void CreateAnInstance_WhenLoggerFactoryIsNull_ShouldThrowArgumentNullException()
        {
            var clientConfiguration = new AutoPollConfiguration
            {
                ProjectSecret = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf",
                LoggerFactory = null
            };

            new BetterConfigClient(clientConfiguration);

        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void CreateAnInstance_WhenAutoPollConfigurationIsNull_ShouldThrowArgumentNullException()
        {
            AutoPollConfiguration config = null;

            new BetterConfigClient(config);
        }
        
        [TestMethod]
        public void CreateAnInstance_WithValidConfiguration_ShouldCreateAnInstance()
        {
            var config = new AutoPollConfiguration
            {
                ProjectSecret = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf"
            };

            new BetterConfigClient(config);
        }

        [TestMethod]
        public void CreateAnInstance_WithProjectToken_ShouldCreateAnInstance()
        {
            string projectSecret = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf";

            new BetterConfigClient(projectSecret);
        }        
    }

    internal class BetterConfigClientConfigurationStub : ConfigurationBase { }
}

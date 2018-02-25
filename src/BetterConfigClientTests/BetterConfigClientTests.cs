using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BetterConfig;

namespace BetterConfigClientTests
{

    [TestClass]
    public class BetterConfigClientTests
    {
        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void CreateAnInstance_WhenProjectTokenIsEmpty_ShouldThrowArgumentNullException()
        {
            string projectToken = string.Empty;

            new BetterConfigClient(projectToken);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void CreateAnInstance_WhenProjectTokenIsNull_ShouldThrowArgumentNullException()
        {
            string projectToken = null;

            new BetterConfigClient(projectToken);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void CreateAnInstance_WhenConfigurationProjectTokenIsNull_ShouldThrowArgumentNullException()
        {
            var clientConfiguration = new BetterConfigClientConfiguration
            {
                ProjectToken = null
            };

            new BetterConfigClient(clientConfiguration);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void CreateAnInstance_WhenConfigurationProjectTokenIsEmpty_ShouldThrowArgumentNullException()
        {
            var clientConfiguration = new BetterConfigClientConfiguration
            {
                ProjectToken = string.Empty
            };

            new BetterConfigClient(clientConfiguration);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void CreateAnInstance_WhenConfigurationTimeToLiveSecondsIsZero_ShouldThrowArgumentOutOfRangeException()
        {
            var clientConfiguration = new BetterConfigClientConfiguration
            {
                ProjectToken = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf",
                TimeToLiveSeconds = 0
            };

            new BetterConfigClient(clientConfiguration);
        }        

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void CreateAnInstance_WhenTraceFactoryIsNull_ShouldThrowArgumentNullException()
        {
            var clientConfiguration = new BetterConfigClientConfiguration
            {
                ProjectToken = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf",
                TraceFactory = null
            };

            new BetterConfigClient(clientConfiguration);

        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void CreateAnInstance_WhenConfigurationIsNull_ShouldThrowArgumentNullException()
        {
            BetterConfigClientConfiguration config = null;

            new BetterConfigClient(config);
        }
        
        [TestMethod]
        public void CreateAnInstance_WithValidConfiguration_ShouldCreateAnInstance()
        {
            BetterConfigClientConfiguration config = new BetterConfigClientConfiguration
            {
                ProjectToken = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf"
            };

            new BetterConfigClient(config);
        }

        [TestMethod]
        public void CreateAnInstance_WithProjectToken_ShouldCreateAnInstance()
        {
            string projectToken = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf";

            new BetterConfigClient(projectToken);
        }
    }
}

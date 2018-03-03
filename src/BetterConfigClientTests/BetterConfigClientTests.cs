﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BetterConfig;
using BetterConfig.Trace;

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
        public void CreateAnInstance_WhenConfigurationProjectSecretIsNull_ShouldThrowArgumentNullException()
        {
            var clientConfiguration = new BetterConfigClientConfiguration
            {
                ProjectSecret = null
            };

            new BetterConfigClient(clientConfiguration);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void CreateAnInstance_WhenConfigurationProjectSecretIsEmpty_ShouldThrowArgumentNullException()
        {
            var clientConfiguration = new BetterConfigClientConfiguration
            {
                ProjectSecret = string.Empty
            };

            new BetterConfigClient(clientConfiguration);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [TestMethod]
        public void CreateAnInstance_WhenConfigurationTimeToLiveSecondsIsZero_ShouldThrowArgumentOutOfRangeException()
        {
            var clientConfiguration = new BetterConfigClientConfiguration
            {
                ProjectSecret = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf",
                TimeToLiveSeconds = 0
            };

            new BetterConfigClient(clientConfiguration);
        }        

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void CreateAnInstance_WhenLoggerFactoryIsNull_ShouldThrowArgumentNullException()
        {
            var clientConfiguration = new BetterConfigClientConfiguration
            {
                ProjectSecret = "hsdrTr4sxbHdSgdhHRZds346hdgsS2vfsgf/GsdrTr4sxbHdSgdhHRZds346hdOPsSgvfsgf",
                LoggerFactory = null
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

        [TestMethod]
        public void MyTestMethod()
        {
            string ps = "__rlPVCCuDMlXYrZsu5t36FQ/rlPVCJqKY-__aZpoP7PCLA";

            var bc = new BetterConfigClient(new BetterConfigClientConfiguration
            {
                ProjectSecret = ps,
                LoggerFactory = new ConsoleLoggerFactory()
            });

            Console.WriteLine(bc.GetValue("keyDouble", 1d));
            Console.WriteLine(bc.GetValue("keyDouble2", 1d));
        }
    }
}

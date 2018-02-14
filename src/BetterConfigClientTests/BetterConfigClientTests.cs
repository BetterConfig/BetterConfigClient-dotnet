using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BetterConfig;

namespace BetterConfigClientTests
{

    [TestClass]
    public class BetterConfigClientTests
    {
        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void CreateAnInstanceWhenUrlIsNullShouldThrowArgumentNullException()
        {
            string url = null;

            new BetterConfigClient(url);

        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void CreateAnInstanceWhenUrlIsEmptyStringShouldThrowArgumentNullException()
        {
            string url = string.Empty;

            new BetterConfigClient(url);

        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void CreateAnInstanceWhenConfigurationIsNullShouldThrowArgumentNullException()
        {
            BetterConfigClientConfiguration url = null;

            new BetterConfigClient(url);
        }        
    }
}

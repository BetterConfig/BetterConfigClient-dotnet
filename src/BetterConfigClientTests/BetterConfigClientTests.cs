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
        public void CreateAnInstanceWhenUrlIsNullShouldThrowArgumentException()
        {
            string url = null;

            new BetterConfigClient(url);

        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void CreateAnInstanceWhenConfigurationIsNullShouldThrowArgumentException()
        {
            BetterConfigClientConfiguration url = null;

            new BetterConfigClient(url);
        }
    }
}

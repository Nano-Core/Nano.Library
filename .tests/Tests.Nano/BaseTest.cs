using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Nano
{
    /// <summary>
    /// Base Test (abstact)
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Cleanup.
        /// Implementation designated for cleanup, after all the tests in a fixture has completed.
        /// </summary>
        [TestCleanup]
        public abstract void Cleanup();

        /// <summary>
        /// Cleanup.
        /// Implementation designated for initializing, before all the tests in a fixture starts executing.
        /// </summary>
        [TestInitialize]
        public abstract void Initialize();
    }
}

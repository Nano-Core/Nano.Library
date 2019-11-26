using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Nano
{
    /// <summary>
    /// Default Tests
    /// </summary>
    public abstract class DefaultTest : BaseTest
    {
        /// <inheritdoc/>
        [TestCleanup]
        public override void Cleanup()
        {

        }

        /// <inheritdoc/>
        [TestInitialize]
        public override void Initialize()
        {
            
        }
    }
}
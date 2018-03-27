using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Nano
{
    public abstract class BaseTest
    {
        [TestCleanup]
        public abstract void Cleanup();

        [TestInitialize]
        public abstract void Initialize();
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nano.Data;

namespace Tests.Nano.Data
{
    [TestClass]
    public class DataOptionsTest
    {
        [TestMethod]
        public void GetSectionNameTest()
        {
            Assert.AreEqual("Data", DataOptions.SectionName);
        }

        [TestMethod]
        public void DefaultValuesTest()
        {
            var options = new DataOptions();

            Assert.AreEqual(25, options.BatchSize);
            Assert.AreEqual(500, options.BulkBatchSize);
            Assert.AreEqual(1000, options.BulkBatchDelay);
            Assert.AreEqual(0, options.QueryRetryCount);
            Assert.AreEqual(4, options.QueryIncludeDepth);
            Assert.AreEqual(true, options.UseAudit);
            Assert.AreEqual(true, options.UseLazyLoading);
            Assert.AreEqual(true, options.UseMemoryCache);
            Assert.AreEqual(true, options.UseSoftDeletetion);
            Assert.AreEqual(false, options.UseCreateDatabase);
            Assert.AreEqual(true, options.UseMigrateDatabase);
            Assert.AreEqual(true, options.UseHealthCheck);
            Assert.IsNull(options.ConnectionString);
        }
    }
}
using System.Data.Entity;

namespace SlimRepository.EntityFramework.Test.Data
{
    partial class TestContext : DbContext
    {
        public DbSet<TestObject> TestObjects { get; set; }

        public TestContext() { }
    }
}

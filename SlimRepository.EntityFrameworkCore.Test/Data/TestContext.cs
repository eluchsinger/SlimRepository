using Microsoft.EntityFrameworkCore;

namespace SlimRepository.EntityFrameworkCore.Test.Data
{
    partial class TestContext : DbContext
    {
        public DbSet<TestObject> TestObjects { get; set; }

        public TestContext() { }

        public TestContext(DbContextOptions<TestContext> options)
            : base(options)
        { }
    }
}

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SlimRepository.EntityFrameworkCore.Test.Data;
using Xunit;

namespace SlimRepository.EntityFrameworkCore.Test
{
    public partial class RepositoryTests
    {
        [Fact]
        public void GivenDatabaseIsEmpty_ItShouldAddAnObject()
        {
            var options = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(Helper.GetCallerName())
                .Options;
            TestObject addedObject;

            using (var context = new TestContext(options))
            {
                var repository = new Repository<TestObject>(context);
                addedObject = repository.Add(new TestObject { Name = "Test" });
            }

            using (var context = new TestContext(options))
            {
                var returnedObject = context.TestObjects.Find(addedObject.Id);
                addedObject.Should().NotBeNull("an object should be added");
                returnedObject.Should().BeEquivalentTo(addedObject);
            }
        }

        [Fact]
        public void GivenDatabaseHasObjects_ItShouldRemoveFirstObject()
        {
            var options = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(Helper.GetCallerName())
                .Options;
            var seedData = options.EnsureSeeded();

            using (var context = new TestContext(options))
            {
                var repository = new Repository<TestObject>(context);
                repository.Delete(seedData[0]);
            }

            using (var context = new TestContext(options))
            {
                var foundObject = context.TestObjects.Find(seedData[0].Id);
                foundObject.Should().BeNull();
            }
        }

    }
}

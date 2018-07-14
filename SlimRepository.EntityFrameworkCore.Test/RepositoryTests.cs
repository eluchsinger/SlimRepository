using System.Xml.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SlimRepository.EntityFrameworkCore.Test.Data;
using Xunit;

namespace SlimRepository.EntityFrameworkCore.Test
{
    public partial class RepositoryTests
    {
        public class Add
        {
            [Fact]
            [Trait("Category", "Add")]
            public void GivenDatabaseIsEmpty_ItShouldAddAnObject()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                TestObject addedObject;

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);
                    addedObject = repository.Add(new TestObject {Name = "Test"});
                }

                using (var context = new TestContext(options))
                {
                    var returnedObject = context.TestObjects.Find(addedObject.Id);
                    addedObject.Should().NotBeNull("an object should be added");
                    returnedObject.Should().BeEquivalentTo(addedObject);
                }
            }
        }

        public class Delete
        {

            [Fact]
            [Trait("Category", "Delete")]
            public void GivenRemovingEntityNotInStore_ItShouldThrow()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);
                    repository.Invoking(r => r.Delete(new TestObject()))
                        .Should().Throw<DbUpdateConcurrencyException>()
                        .WithMessage("Attempted to update or delete an entity that does not exist in the store.");
                }
            }

            [Fact]
            [Trait("Category", "Delete")]
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

        public class GetById
        {

            [Fact]
            [Trait("Category", "GetById")]
            public void GivenDatabaseHasObjects_ItShouldReturnOneById()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var seedData = options.EnsureSeeded();
                var expectedObject = seedData[0];
                TestObject foundObject;

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);
                    foundObject = repository.GetById(expectedObject.Id);
                }

                foundObject.Should().BeEquivalentTo(expectedObject);
            }

            [Fact]
            [Trait("Category", "GetById")]
            public void GivenDatabaseHasNoObjects_ItShouldReturnNullById()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                TestObject foundObject;

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);
                    foundObject = repository.GetById(0);
                }

                foundObject.Should().BeNull();
            }
        }
        
        public class Edit
        {
            [Fact]
            [Trait("Category", "Edit")]
            public void GivenObjectContainedInDatabaseIsEdited_ItShouldBePersisted()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var seedData = options.EnsureSeeded();
                var changedObject = seedData[0];

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);

                    changedObject.Name = "EditedObject";
                    repository.Edit(changedObject);
                }

                using (var context = new TestContext(options))
                {
                    var foundObject = context.Find<TestObject>(changedObject.Id);
                    foundObject.Should().BeEquivalentTo(changedObject);
                }
            }
        }
    }
}
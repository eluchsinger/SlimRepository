using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SlimRepository.EntityFrameworkCore.Test.Data;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SlimRepository.EntityFrameworkCore.Test
{
    public class AsyncRepositoryTests
    {
        public class AddAsync
        {
            [Fact]
            [Trait("Category", "AddAsync")]
            public async void GivenDatabaseDoesNotContainObject_ItShouldAddAnObjectAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                TestObject addedObject;

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    addedObject = await repository.AddAsync(new TestObject { Name = "Test" });
                }

                using (var context = new TestContext(options))
                {
                    var returnedObject = context.TestObjects.Find(addedObject.Id);
                    addedObject.Should().NotBeNull("an object should be added");
                    returnedObject.Should().BeEquivalentTo(addedObject);
                }
            }

            [Fact]
            [Trait("Category", "AddAsync")]
            public async void GivenDatabaseContainsSameObject_ItShouldReplaceTheObjectAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var addedEntity = new TestObject(id: 1, name: "TestObject");
                TestObject actualEntity;
                using (var context = new TestContext(options))
                {
                    context.Add(addedEntity);
                }

                using (var context = new TestContext(options))
                {
                    var newEntity = new TestObject(id: 1, name: "NewTestObject");
                    var repository = new AsyncRepository<TestObject>(context);
                    actualEntity = await repository.AddAsync(newEntity);
                }

                actualEntity.Should().BeEquivalentTo(new TestObject(1, "NewTestObject"));
            }
        }

        public class AddRangeAsync
        {
            [Fact]
            [Trait("Category", "AddRangeAsync")]
            public async void GivenObjectsNotInDatabase_AddRangeOfObjectsAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var objectsToAdd = new[]
                {
                    new TestObject(name: "Test1"),
                    new TestObject(name: "Test2"),
                    new TestObject(name: "Test3"),
                    new TestObject(name: "Test4")
                };

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    await repository.AddRangeAsync(objectsToAdd);
                }

                using (var context = new TestContext(options))
                {
                    var addedObjectNames = objectsToAdd.Select(o => o.Name);
                    context.TestObjects
                        .Select(testObject => testObject.Name)
                        .All(name => addedObjectNames.Contains(name))
                        .Should()
                        .BeTrue();
                }
            }
        }

        public class Delete
        {
            [Fact]
            [Trait("Category", "DeleteAsync")]
            public void GivenRemovingEntityNotInStore_ItShouldThrowWhenAwaiting()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    repository.DeleteAsync(new TestObject())
                        .GetAwaiter()
                        .Invoking(awaiter => awaiter.GetResult())
                        .Should().Throw<DbUpdateConcurrencyException>()
                        .WithMessage("Attempted to update or delete an entity that does not exist in the store.");
                }
            }

            [Fact]
            [Trait("Category", "DeleteAsync")]
            public async void GivenDeleteZero_ItShouldRemoveFirstObjectAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var seedData = options.EnsureSeeded();

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    await repository.DeleteAsync(seedData[0]);
                }

                using (var context = new TestContext(options))
                {
                    var foundObject = context.TestObjects.Find(seedData[0].Id);
                    foundObject.Should().BeNull();
                }
            }
        }

        public class GetByIdAsync
        {
            [Fact]
            [Trait("Category", "GetByIdAsync")]
            public async void GivenDatabaseHasObjects_ItShouldReturnOneByIdAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var seedData = options.EnsureSeeded();
                var expectedObject = seedData[0];
                TestObject foundObject;

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    foundObject = await repository.GetByIdAsync(expectedObject.Id);
                }

                foundObject.Should().BeEquivalentTo(expectedObject);
            }

            [Fact]
            [Trait("Category", "GetByIdAsync")]
            public async void GivenDatabaseHasNoObjects_ItShouldReturnNullByIdAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                TestObject foundObject;

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    foundObject = await repository.GetByIdAsync(0);
                }

                foundObject.Should().BeNull();
            }
        }

        public class EditAsync
        {
            [Fact]
            [Trait("Category", "EditAsync")]
            public async void GivenObjectContainedInDatabaseIsEdited_ItShouldBePersistedAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var seedData = options.EnsureSeeded();
                var changedObject = seedData[0];

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    changedObject.Name = "EditedObject";
                    await repository.EditAsync(changedObject);
                }

                using (var context = new TestContext(options))
                {
                    var foundObject = context.Find<TestObject>(changedObject.Id);
                    foundObject.Should().BeEquivalentTo(changedObject);
                }
            }
        }

        public class ListAsync
        {
            [Fact]
            [Trait("Category", "ListAsync")]
            public async void GivenDatabaseIsEmpty_ItShouldReturnEmptyListAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    (await repository.ListAsync()).Should().BeEmpty();
                }
            }

            [Fact]
            [Trait("Category", "ListAsync")]
            public async void GivenDatabaseContainsObjects_ItShouldReturnAllObjectsAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var seedData = options.EnsureSeeded();
                IList<TestObject> returnedList;

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    returnedList = await repository.ListAsync();
                }

                returnedList.Should().BeEquivalentTo(seedData);
            }

            [Fact]
            [Trait("Category", "ListAsync")]
            public async void GivenDatabaseContainsObjects_ItShouldReturnOnlyObjectsWithAThreeInTheNameAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                options.EnsureSeeded();
                IList<TestObject> returnedList;

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    returnedList = await repository.ListAsync(o => o.Name.Contains("3"));
                }

                returnedList.Should()
                    .OnlyContain(o => o.Name.Contains("3"));
            }

            [Fact]
            [Trait("Category", "List")]
            public async void GivenDatabaseContainsObjects_ItShouldReturnAllBySpecificationAsync()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var seedData = options.EnsureSeeded();
                IList<TestObject> returnedList;

                using (var context = new TestContext(options))
                {
                    var repository = new AsyncRepository<TestObject>(context);
                    returnedList = await repository.ListAsync(new EmptySpecification<TestObject>(o => true));
                }

                returnedList.Should().BeEquivalentTo(seedData);
            }
        }
    }
}
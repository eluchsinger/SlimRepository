using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SlimRepository.EntityFrameworkCore.Test.Data;
using SlimRepository.Interfaces;
using Xunit;

namespace SlimRepository.EntityFrameworkCore.Test
{
    public class RepositoryTests
    {
        public class Add
        {
            [Fact]
            [Trait("Category", "Add")]
            public void GivenDatabaseDoesNotContainObject_ItShouldAddAnObject()
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

            [Fact]
            [Trait("Category", "Add")]
            public void GivenDatabaseContainsSameObject_ItShouldReplaceTheObject()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var addedEntity = new TestObject(id: 1, name:"TestObject");
                TestObject actualEntity;
                using (var context = new TestContext(options))
                {
                    context.Add(addedEntity);
                }

                using (var context = new TestContext(options))
                {
                    var newEntity = new TestObject(id: 1, name:"NewTestObject");
                    var repository = new Repository<TestObject>(context);
                    actualEntity = repository.Add(newEntity);
                }

                actualEntity.Should().BeEquivalentTo(new TestObject(1, "NewTestObject"));
            }
        }

        public class AddRange
        {
            [Fact]
            [Trait("Category", "AddRange")]
            public void GivenObjectsNotInDatabase_AddRangeOfObjects()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var objectsToAdd = new []
                {
                    new TestObject(name: "Test1"),
                    new TestObject(name: "Test2"),
                    new TestObject(name: "Test3"),
                    new TestObject(name: "Test4")
                };

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);
                    repository.AddRange(objectsToAdd);
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
            public void GivenDeleteZero_ItShouldRemoveFirstObject()
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

        public class List
        {
            [Fact]
            [Trait("Category", "List")]
            public void GivenDatabaseIsEmpty_ItShouldReturnEmptyList()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);
                    repository.List().Should().BeEmpty();
                }
            }

            [Fact]
            [Trait("Category", "List")]
            public void GivenDatabaseContainsObjects_ItShouldReturnAllObjects()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var seedData = options.EnsureSeeded();
                IList<TestObject> returnedList;

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);
                    returnedList = repository.List();
                }

                returnedList.Should().BeEquivalentTo(seedData);
            }

            [Fact]
            [Trait("Category", "List")]
            public void GivenDatabaseContainsObjects_ItShouldReturnOnlyObjectsWithAThreeInTheName()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                options.EnsureSeeded();
                IList<TestObject> returnedList;

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);
                    returnedList = repository.List(o => o.Name.Contains("3"));
                }

                returnedList.Should()
                    .OnlyContain(o => o.Name.Contains("3"));
            }

            [Fact]
            [Trait("Category", "List")]
            public void GivenDatabaseContainsObjects_ItShouldReturnAllBySpecification()
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseInMemoryDatabase(Helper.GetCallerName())
                    .Options;
                var seedData = options.EnsureSeeded();
                IList<TestObject> returnedList;

                using (var context = new TestContext(options))
                {
                    var repository = new Repository<TestObject>(context);
                    returnedList = repository.List(new EmptySpecification<TestObject>(o => true));
                }

                returnedList.Should().BeEquivalentTo(seedData);
            }
        }
    }
}
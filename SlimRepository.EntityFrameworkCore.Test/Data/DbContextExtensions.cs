using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlimRepository.EntityFrameworkCore.Test.Data
{
    internal static class DbContextExtensions
    {
        /// <summary>
        /// Ensures that the database is seeded and returns the seed data
        /// </summary>
        /// <param name="dbContext">The Database Context to seed.</param>
        /// <returns>Returns the newly seeded data or null, if the database was already seeded.</returns>
        public static IList<TestObject> EnsureSeeded(this TestContext dbContext)
        {
            const int amountOfObjects = 100;

            if (!dbContext.TestObjects.Any())
            {
                for (int i = 0; i < amountOfObjects; i++)
                {
                    dbContext.TestObjects.Add(new TestObject
                    {
                        Name = $"TestObject{i}"
                    });
                }
                dbContext.SaveChanges(true);
                return dbContext.TestObjects
                    .AsNoTracking()
                    .ToList();
            }

            return null;
        }

        /// <summary>
        /// Ensures that the database contained in the database options is seeded and returns the seed data.
        /// </summary>
        /// <param name="options">The options object containing the connection information to the database.</param>
        /// <returns>Returns the newly seeded data or null, if the database was already seeded.</returns>
        public static IList<TestObject> EnsureSeeded(this DbContextOptions<TestContext> options)
        {
            using (var context = new TestContext(options))
            {
                return context.EnsureSeeded();
            }
        }
    }
}

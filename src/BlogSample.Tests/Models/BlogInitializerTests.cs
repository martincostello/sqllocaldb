using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogSample.Models
{
    /// <summary>
    /// A class containing tests for the <see cref="BlogInitializer"/> class.
    /// </summary>
    [TestClass]
    public class BlogInitializerTests
    {
        [TestMethod]
        public void BlogInitializer_Adds_And_Persists_Data_To_Database()
        {
            // Arrange
            // Use a new database so that our results are not interfered with by other tests.
            // If this is the first test to run, the shared SQL LocalDB instance will be created now.
            string connectionString = TestSetup.GetConnectionStringForNewDatabase();

            BlogInitializer target = new BlogInitializer();

            using (BlogContext context = new BlogContext(connectionString))
            {
                // Act
                target.InitializeDatabase(context);
            }

            int actual;

            using (BlogContext context = new BlogContext(connectionString))
            {
                // Assert
                actual = context.Posts.Count();
            }

            Assert.AreEqual(2, actual);
        }
    }
}

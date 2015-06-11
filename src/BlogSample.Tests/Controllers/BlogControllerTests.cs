using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BlogSample.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogSample.Controllers
{
    /// <summary>
    /// A class containing tests for the <see cref="BlogController"/> class.
    /// </summary>
    [TestClass]
    public class BlogControllerTests
    {
        /// <summary>
        /// The name of the database used for tests that require an empty database. This field is read-only.
        /// </summary>
        /// <remarks>
        /// A specific name is used for the empty database so different tests can re-use the
        /// database to improve the overall performance of the test run. If tests in other
        /// classes used an empty database, we could expose that as a shared database in the
        /// <see cref="TestSetup"/> class as well as the shared instance to reduce repetition.
        /// </remarks>
        private static readonly string EmptyDatabaseName = TestSetup.GenerateDatabaseName();

        [TestMethod]
        public void BlogController_Default_Constructor_Does_Not_Throw()
        {
            // No Arrange - as this test does not use a database it should run quickly
            // if run by itself and not trigger the creation of the shared SQL LocalDB
            // instance or a blog database.

            // Act
            using (BlogController target = new BlogController())
            {
                // No Assert
            }
        }

        [TestMethod]
        public async Task BlogController_Archive_Returns_Empty_Model_If_There_Are_No_Posts_In_The_Database()
        {
            // Arrange
            // Use the database we know is empty and has no data in it.
            // If this is the first test to run, the shared SQL LocalDB instance will be created now.
            string connectionString = TestSetup.GetConnectionStringForDatabase(EmptyDatabaseName);

            using (BlogController target = CreateTarget(connectionString))
            {
                // Act
                ActionResult result = await target.Archive();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ViewResult));

                ViewResult view = result as ViewResult;

                Assert.IsInstanceOfType(view.Model, typeof(ICollection<BlogPostViewModel>));
                Assert.AreEqual(string.Empty, view.ViewName);

                ICollection<BlogPostViewModel> model = view.Model as ICollection<BlogPostViewModel>;

                Assert.AreEqual(0, model.Count);
            }
        }

        [TestMethod]
        public async Task BlogController_Archive_Returns_All_Posts_If_There_Are_Less_Than_Five_Posts_In_The_Database()
        {
            // Arrange
            // Use a new database so that our results are not interfered with by other tests.
            // If this is the first test to run, the shared SQL LocalDB instance will be created now.
            string connectionString = TestSetup.GetConnectionStringForNewDatabase();

            DateTime now = DateTime.UtcNow;

            using (BlogContext context = new BlogContext(connectionString))
            {
                context.Posts.Add(new BlogPost() { Body = "Body 1", Preview = "Preview 1", Title = "Title 1", PublishedAt = now });
                context.Posts.Add(new BlogPost() { Body = "Body 2", Preview = "Preview 2", Title = "Title 2", PublishedAt = now.AddDays(1) });
                await context.SaveChangesAsync();
            }

            using (BlogController target = CreateTarget(connectionString))
            {
                // Act
                ActionResult result = await target.Archive();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ViewResult));

                ViewResult view = result as ViewResult;

                Assert.IsInstanceOfType(view.Model, typeof(ICollection<BlogPostViewModel>));
                Assert.AreEqual(string.Empty, view.ViewName);

                ICollection<BlogPostViewModel> model = view.Model as ICollection<BlogPostViewModel>;

                Assert.AreEqual(2, model.Count);

                Assert.AreEqual("Title 2", model.First().Title);
                Assert.AreEqual("Title 1", model.Last().Title);
            }
        }

        [TestMethod]
        public async Task BlogController_Archive_Returns_All_Posts_If_There_Are_More_Than_Five_Posts_In_The_Database()
        {
            // Arrange
            // We can use the shared database as the test data will artificially put items far
            // enough in the future that we will still get the results we want and because our
            // expected result set has a maximum size it does not matter if there is data already present.
            // If this is the first test to run, the shared SQL LocalDB instance will be created now.
            DateTime now = DateTime.UtcNow.AddYears(3);

            using (BlogContext context = new BlogContext(TestSetup.ConnectionString))
            {
                context.Posts.Add(new BlogPost() { Body = "Body 1", Preview = "Preview 1", Title = "Future Title 1", PublishedAt = now });
                context.Posts.Add(new BlogPost() { Body = "Body 2", Preview = "Preview 2", Title = "Future Title 2", PublishedAt = now.AddDays(1) });
                context.Posts.Add(new BlogPost() { Body = "Body 3", Preview = "Preview 3", Title = "Future Title 3", PublishedAt = now.AddDays(2) });
                context.Posts.Add(new BlogPost() { Body = "Body 4", Preview = "Preview 4", Title = "Future Title 4", PublishedAt = now.AddDays(3) });
                context.Posts.Add(new BlogPost() { Body = "Body 5", Preview = "Preview 5", Title = "Future Title 5", PublishedAt = now.AddDays(4) });
                context.Posts.Add(new BlogPost() { Body = "Body 6", Preview = "Preview 6", Title = "Future Title 6", PublishedAt = now.AddDays(5) });
                context.Posts.Add(new BlogPost() { Body = "Body 7", Preview = "Preview 7", Title = "Future Title 7", PublishedAt = now.AddDays(6) });

                await context.SaveChangesAsync();
            }

            using (BlogController target = CreateTarget())
            {
                // Act
                ActionResult result = await target.Archive();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ViewResult));

                ViewResult view = result as ViewResult;

                Assert.IsInstanceOfType(view.Model, typeof(ICollection<BlogPostViewModel>));
                Assert.AreEqual(string.Empty, view.ViewName);

                ICollection<BlogPostViewModel> model = view.Model as ICollection<BlogPostViewModel>;

                Assert.AreEqual(5, model.Count);

                Assert.AreEqual("Future Title 7", model.First().Title);
                Assert.AreEqual("Future Title 3", model.Last().Title);
            }
        }

        [TestMethod]
        public async Task BlogController_Index_Returns_Null_Model_If_There_Are_No_Posts_In_The_Database()
        {
            // Arrange
            // Use the database we know is empty and has no data in it.
            // If this is the first test to run, the shared SQL LocalDB instance will be created now.
            string connectionString = TestSetup.GetConnectionStringForDatabase(EmptyDatabaseName);

            using (BlogController target = CreateTarget(connectionString))
            {
                // Act
                ActionResult result = await target.Index();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ViewResult));

                ViewResult view = result as ViewResult;

                Assert.IsNull(view.Model);
                Assert.AreEqual(string.Empty, view.ViewName);
            }
        }

        [TestMethod]
        public async Task BlogController_Index_Model_Is_For_Latest_Post_If_There_Are_Posts_In_The_Database()
        {
            // Arrange
            // Use a new database so that our results are not interfered with by other tests.
            // If this is the first test to run, the shared SQL LocalDB instance will be created now.
            string connectionString = TestSetup.GetConnectionStringForNewDatabase();

            using (BlogContext context = new BlogContext(connectionString))
            {
                context.Posts.Add(new BlogPost() { Title = "Title 1", Body = "Post 1", Preview = "Preview 1", PublishedAt = new DateTime(2015, 6, 10, 12, 00, 00, DateTimeKind.Utc) });
                context.Posts.Add(new BlogPost() { Title = "Title 2", Body = "Post 2", Preview = "Preview 2", PublishedAt = new DateTime(2015, 6, 11, 12, 00, 00, DateTimeKind.Utc) });
                context.Posts.Add(new BlogPost() { Title = "Title 3", Body = "Post 3", Preview = "Preview 3", PublishedAt = new DateTime(2015, 6, 11, 13, 00, 00, DateTimeKind.Utc) });

                await context.SaveChangesAsync();
            }

            using (BlogController target = CreateTarget(connectionString))
            {
                // Act
                ActionResult result = await target.Index();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ViewResult));

                ViewResult view = result as ViewResult;

                Assert.IsNotNull(view.Model);
                Assert.IsInstanceOfType(view.Model, typeof(HomePageViewModel));
                Assert.AreEqual(string.Empty, view.ViewName);

                HomePageViewModel model = view.Model as HomePageViewModel;

                Assert.AreEqual("Preview 3", model.LatestPostPreview);
                Assert.AreEqual("Title 3", model.LatestPostTitle);
            }
        }

        [TestMethod]
        public async Task BlogController_Latest_Returns_HttpNotFound_Result_If_There_Are_No_Posts_In_The_Database()
        {
            // Arrange
            // Use the database we know is empty and has no data in it.
            // If this is the first test to run, the shared SQL LocalDB instance will be created now.
            string connectionString = TestSetup.GetConnectionStringForDatabase(EmptyDatabaseName);

            using (BlogController target = CreateTarget(connectionString))
            {
                // Act
                ActionResult result = await target.Latest();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
            }
        }

        [TestMethod]
        public void BlogController_New_For_Get_Returns_Correct_View()
        {
            // Arrange
            using (BlogController target = new BlogController())
            {
                // Act
                ActionResult result = target.New();

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ViewResult));

                ViewResult view = result as ViewResult;

                Assert.AreEqual(string.Empty, view.ViewName);
            }
        }

        [TestMethod]
        public async Task BlogController_New_For_Post_Adds_Post_If_Valid()
        {
            // Arrange
            DateTime now = DateTime.UtcNow;
            now = now.AddMilliseconds(now.Millisecond * -1);

            NewPostViewModel model = new NewPostViewModel()
            {
                Title = "New Blog Post Title",
                Body = "This is my blog post about things.",
                Preview = "This is a preview.",
            };

            // Use the shared database as this test only inserts
            // so other tests cannot interfer with its asserts.
            using (BlogController target = CreateTarget())
            {
                // Act
                ActionResult result = await target.New(model);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ViewResult));

                ViewResult view = result as ViewResult;

                Assert.IsNotNull(view.Model);
                Assert.IsInstanceOfType(view.Model, typeof(BlogPostViewModel));
                Assert.AreEqual("Post", view.ViewName);

                BlogPostViewModel viewModel = view.Model as BlogPostViewModel;

                Assert.AreEqual(model.Body, viewModel.Body);
                Assert.AreNotEqual(0, viewModel.Id);
                Assert.AreEqual(model.Preview, viewModel.Preview);
                Assert.IsTrue(viewModel.PublishedAt >= now);
                Assert.AreEqual(model.Title, viewModel.Title);
            }
        }

        [TestMethod]
        public async Task BlogController_New_For_Post_Does_Not_Add_Post_If_Invalid()
        {
            // Arrange
            DateTime now = DateTime.UtcNow;
            now = now.AddMilliseconds(now.Millisecond * -1);

            NewPostViewModel model = new NewPostViewModel();

            using (BlogController target = new BlogController())
            {
                target.ModelState.AddModelError("Body", "A body must be specified.");

                // Act
                ActionResult result = await target.New(model);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ViewResult));

                ViewResult view = result as ViewResult;

                Assert.IsNotNull(view.Model);
                Assert.AreSame(model, view.Model);
                Assert.AreEqual(string.Empty, view.ViewName);
            }
        }

        [TestMethod]
        public async Task BlogController_Post_Returns_HttpNotFound_Result_If_Post_With_Id_Not_Found()
        {
            // Arrange
            int id = int.MaxValue;

            // Use the shared database as the Id is made up so should not find anything.
            // If this is the first test to run, the shared SQL LocalDB instance will be created now.
            using (BlogController target = CreateTarget())
            {
                // Act
                ActionResult result = await target.Post(id);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
            }
        }

        [TestMethod]
        public async Task BlogController_Post_Returns_Model_If_Post_With_Id_Was_Found()
        {
            // Arrange
            BlogPost entity = new BlogPost()
            {
                Body = "Body",
                Preview = "Preview",
                PublishedAt = DateTime.UtcNow,
                Title = "Title",
            };

            int id;

            // Use the shared database as this test only inserts
            // so other tests cannot interfer with its asserts.
            using (BlogContext context = new BlogContext(TestSetup.ConnectionString))
            {
                context.Posts.Add(entity);
                await context.SaveChangesAsync();

                id = entity.Id;
            }

            using (BlogController target = CreateTarget())
            {
                // Act
                ActionResult result = await target.Post(id);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(ViewResult));

                ViewResult view = result as ViewResult;

                Assert.IsNotNull(view.Model);
                Assert.IsInstanceOfType(view.Model, typeof(BlogPostViewModel));
                Assert.AreEqual(string.Empty, view.ViewName);

                BlogPostViewModel model = view.Model as BlogPostViewModel;

                Assert.AreEqual(entity.Body, model.Body);
                Assert.AreEqual(id, model.Id);
                Assert.AreEqual(entity.Preview, model.Preview);
                Assert.IsTrue(entity.PublishedAt >= model.PublishedAt.AddMilliseconds(model.PublishedAt.Millisecond * -1));
                Assert.AreEqual(entity.Title, model.Title);
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="BlogController"/> that uses the shared blog database in SQL LocalDB.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="BlogController"/>.
        /// </returns>
        private static BlogController CreateTarget()
        {
            // If this is the first time this method is called, the shared
            // blog database and SQL LocalDB instance will be created now
            return CreateTarget(TestSetup.ConnectionString);
        }

        /// <summary>
        /// Creates an instance of <see cref="BlogController"/> that uses the blog database at the specified SQL connection string.
        /// </summary>
        /// <param name="connectionString">The blog connection string to use.</param>
        /// <returns>
        /// The created instance of <see cref="BlogController"/>.
        /// </returns>
        private static BlogController CreateTarget(string connectionString)
        {
            return new BlogController(() => new BlogContext(connectionString));
        }
    }
}

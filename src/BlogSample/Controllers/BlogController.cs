using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BlogSample.Models;

namespace BlogSample.Controllers
{
    /// <summary>
    /// A class representing the blog controller.
    /// </summary>
    public class BlogController : Controller
    {
        /// <summary>
        /// A delegate to a method to use to create instances of <see cref="BlogContext"/>.
        /// </summary>
        /// <remarks>
        /// Used to allow for dependency injection from tests.
        /// </remarks>
        private readonly Func<BlogContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogController"/> class.
        /// </summary>
        public BlogController()
            : this(contextFactory: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlogController"/> class.
        /// </summary>
        /// <param name="contextFactory">An optional delegate to a method to use to create instances of <see cref="BlogContext"/>.</param>
        /// <remarks>
        /// Used to allow for dependency injection from tests.
        /// </remarks>
        internal BlogController(Func<BlogContext> contextFactory)
        {
            _contextFactory = contextFactory ?? BlogContext.Create;
        }

        public async Task<ActionResult> Archive()
        {
            List<BlogPostViewModel> model = new List<BlogPostViewModel>();

            using (var context = _contextFactory())
            {
                // Show up to the last five posts with the latest post first
                var posts = await context.Posts
                    .OrderByDescending((p) => p.PublishedAt)
                    .Take(5)
                    .ToListAsync();

                foreach (BlogPost entity in posts)
                {
                    var item = new BlogPostViewModel()
                    {
                        Body = entity.Body,
                        Id = entity.Id,
                        Preview = entity.Preview,
                        PublishedAt = entity.PublishedAt,
                        Title = entity.Title,
                    };

                    model.Add(item);
                }
            }

            return View(model);
        }

        public async Task<ActionResult> Index()
        {
            HomePageViewModel model = null;

            using (var context = _contextFactory())
            {
                // Only need to get the title and preview text of the latest post (if there is one)
                var latestPost = await context.Posts
                    .OrderByDescending((p) => p.PublishedAt)
                    .Select((p) => new { Title = p.Title, Preview = p.Preview })
                    .FirstOrDefaultAsync();

                if (latestPost != null)
                {
                    model = new HomePageViewModel()
                    {
                        LatestPostPreview = latestPost.Preview,
                        LatestPostTitle = latestPost.Title,
                    };
                }
            }

            return View(model);
        }

        public async Task<ActionResult> Latest()
        {
            BlogPostViewModel model;

            using (var context = _contextFactory())
            {
                // Get the latest post
                BlogPost latestPost = await context.Posts
                    .OrderByDescending((p) => p.PublishedAt)
                    .FirstOrDefaultAsync();

                if (latestPost == null)
                {
                    return HttpNotFound();
                }

                model = new BlogPostViewModel()
                {
                    Body = latestPost.Body,
                    Id = latestPost.Id,
                    Preview = latestPost.Preview,
                    PublishedAt = latestPost.PublishedAt,
                    Title = latestPost.Title,
                };
            }

            return View("Post", model);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> New(NewPostViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            BlogPost entity = new BlogPost()
            {
                Body = model.Body,
                Preview = model.Preview,
                PublishedAt = DateTime.UtcNow,
                Title = model.Title,
            };

            using (var context = _contextFactory())
            {
                context.Posts.Add(entity);
                await context.SaveChangesAsync();
            }

            return await Latest();
        }

        public async Task<ActionResult> Post(int id)
        {
            BlogPostViewModel model;

            using (var context = _contextFactory())
            {
                // Get the first post with the specified Id
                BlogPost post = await context.Posts
                    .Where((p) => p.Id == id)
                    .FirstOrDefaultAsync();

                if (post == null)
                {
                    return HttpNotFound();
                }

                model = new BlogPostViewModel()
                {
                    Body = post.Body,
                    Id = post.Id,
                    Preview = post.Preview,
                    PublishedAt = post.PublishedAt,
                    Title = post.Title,
                };
            }

            return View(model);
        }
    }
}

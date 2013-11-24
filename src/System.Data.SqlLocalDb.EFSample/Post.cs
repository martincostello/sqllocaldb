// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Post.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2013
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   Post.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace System.Data.SqlLocalDb
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime PostedAt { get; set; }

        public int BlogId { get; set; }

        public virtual Blog Blog { get; set; }
    }
}
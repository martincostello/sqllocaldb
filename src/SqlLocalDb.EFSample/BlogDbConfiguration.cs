// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlogDbConfiguration.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   Blog.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.Entity;

namespace System.Data.SqlLocalDb
{
    public sealed class BlogDbConfiguration : DbConfiguration
    {
        public BlogDbConfiguration()
            : base()
        {
        }
    }
}
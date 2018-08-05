// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Xunit;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a fact that should be run by the test runner
    /// if the user account running the test has administrative privileges. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RunAsAdminFactAttribute : FactAttribute
    {
        public RunAsAdminFactAttribute()
            : base()
        {
            Skip = IsCurrentUserAdmin(out string name) ? string.Empty : $"The current user '{name}' does not have administrative privileges.";
        }

        /// <summary>
        /// Returns whether the current user has Administrative privileges.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current user has Administrative
        /// privileges; otherwise <see langword="false"/>.
        /// </returns>
        internal static bool IsCurrentUserAdmin()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return false;
            }

            return IsCurrentUserAdmin(out string name);
        }

        /// <summary>
        /// Returns whether the current user has Administrative privileges.
        /// </summary>
        /// <param name="name">When the method returns, contains the name of the current user.</param>
        /// <returns>
        /// <see langword="true"/> if the current user has Administrative
        /// privileges; otherwise <see langword="false"/>.
        /// </returns>
        private static bool IsCurrentUserAdmin(out string name)
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                name = identity.Name;
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}

// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using AdvancedDLSupport;
using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb.Interop
{
    /// <summary>
    /// A class representing an <see cref="ILibraryPathResolver"/> implementation for the
    /// SQL Server LocalDB Instance API. This class cannot be inherited.
    /// </summary>
    internal sealed class LocalDbInstanceApiPathResolver : ILibraryPathResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDbInstanceApiPathResolver"/> class.
        /// </summary>
        /// <param name="apiVersion">The version of the SQL LocalDB Instance API to load.</param>
        /// <param name="registry">The <see cref="IRegistry"/> to use.</param>
        /// <param name="logger">The logger to use.</param>
        internal LocalDbInstanceApiPathResolver(string apiVersion, IRegistry registry, ILogger<LocalDbInstanceApiPathResolver> logger)
        {
            ApiVersion = apiVersion;
            Registry = registry;
            Logger = logger;
        }

        /// <summary>
        /// Gets the path of the library that was loaded.
        /// </summary>
        internal string LibraryPath { get; private set; }

        /// <summary>
        /// Gets the version of the SQL LocalDB native API loaded, if any.
        /// </summary>
        internal Version NativeApiVersion { get; private set; }

        /// <summary>
        /// Gets the API version to use.
        /// </summary>
        private string ApiVersion { get; }

        /// <summary>
        /// Gets the <see cref="ILogger"/> to use.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Gets the <see cref="IRegistry"/> to use.
        /// </summary>
        private IRegistry Registry { get; }

        /// <inheritdoc />
        public ResolvePathResult Resolve(string library)
        {
            if (TryGetLocalDbApiPath(out string fileName, out string errorReason))
            {
                LibraryPath = fileName;
                return ResolvePathResult.FromSuccess(fileName);
            }

            return ResolvePathResult.FromError(errorReason);
        }

        /// <summary>
        /// Tries to obtaining the path to the latest version of the SQL LocalDB
        /// native API DLL for the currently executing process.
        /// </summary>
        /// <param name="fileName">
        /// When the method returns, contains the path to the SQL Local DB API
        /// to use, if found; otherwise <see langword="null"/>.
        /// </param>
        /// <param name="errorReason">
        /// When the method returns, contains a string containing the reason the
        /// SQL Local DB API was not found if unsuccessful; otherwise <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the native API path was successfully found;
        /// otherwise <see langword="false"/>.
        /// </returns>
        internal bool TryGetLocalDbApiPath(out string fileName, out string errorReason)
        {
            fileName = null;
            errorReason = null;

            string keyName = DeriveLocalDbRegistryKey();
            IRegistryKey key = Registry.OpenSubKey(keyName);

            if (key == null)
            {
                Logger.RegistryKeyNotFound(keyName);
                errorReason = "RegistryKeyNotFound"; // TODO Proper message
                return false;
            }

            Version latestVersion = null;
            Version overrideVersion = null;
            string path = null;

            try
            {
                // Is there a setting overriding the version to load?
                string overrideVersionString = ApiVersion;

                foreach (string versionString in key.GetSubKeyNames())
                {
                    if (!Version.TryParse(versionString, out Version version))
                    {
                        Logger.InvalidRegistryKey(versionString);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(overrideVersionString) &&
                        overrideVersion == null &&
                        string.Equals(versionString, overrideVersionString, StringComparison.OrdinalIgnoreCase))
                    {
                        Logger.NativeApiVersionOverriddenByUser(version);
                        overrideVersion = version;
                    }

                    if (latestVersion == null ||
                        latestVersion < version)
                    {
                        latestVersion = version;
                    }
                }

                if (!string.IsNullOrEmpty(overrideVersionString) && overrideVersion == null)
                {
                    Logger.NativeApiVersionOverrideNotFound(overrideVersionString);
                }

                Version versionToUse = overrideVersion ?? latestVersion;

                if (versionToUse != null)
                {
                    using (IRegistryKey subkey = key.OpenSubKey(versionToUse.ToString()))
                    {
                        path = subkey.GetValue("InstanceAPIPath");
                    }

                    NativeApiVersion = versionToUse;
                }
            }
            finally
            {
                key.Dispose();
            }

            if (string.IsNullOrEmpty(path))
            {
                Logger.NativeApiNotFound();
                errorReason = "NativeApiNotFound"; // TODO Proper message
                return false;
            }

            if (!File.Exists(path))
            {
                Logger.NativeApiLibraryNotFound(path);
                errorReason = "NativeApiLibraryNotFound"; // TODO Proper message
                return false;
            }

            fileName = Path.GetFullPath(path);
            return true;
        }

        /// <summary>
        /// Derives the name of the Windows registry key name to use to locate the SQL LocalDB Instance API.
        /// </summary>
        /// <returns>
        /// The registry key name to use for the current process.
        /// </returns>
        private static string DeriveLocalDbRegistryKey()
        {
            // Open the appropriate Registry key if running as a 32-bit process on a 64-bit machine
            bool isWow64Process = Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess;

            return string.Format(
                CultureInfo.InvariantCulture,
                @"SOFTWARE\{0}Microsoft\Microsoft SQL Server Local DB\Installed Versions",
                isWow64Process ? @"Wow6432Node\" : string.Empty);
        }
    }
}

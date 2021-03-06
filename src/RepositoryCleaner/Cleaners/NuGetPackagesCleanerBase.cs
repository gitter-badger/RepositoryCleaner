﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackagesInLibFolderCleaner.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Cleaners
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel.Logging;
    using Models;

    public abstract class NuGetPackagesCleanerBase : CleanerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected abstract string GetPackagesDirectory(Repository repository);

        protected override bool CanCleanRepository(Repository repository)
        {
            var libPath = GetPackagesDirectory(repository);

            if (!Directory.Exists(libPath))
            {
                return false;
            }

            foreach (var directory in Directory.GetDirectories(libPath))
            {
                if (IsCleanablePath(directory))
                {
                    return true;
                }
            }

            return false;
        }

        protected override long CalculateCleanableSpaceForRepository(Repository repository)
        {
            var space = 0L;

            foreach (var directory in GetNuGetPackageDirectories(repository))
            {
                space += GetDirectorySize(directory);
            }

            return space;
        }

        protected override void CleanRepository(Repository repository, bool isFakeClean)
        {
            foreach (var directory in GetNuGetPackageDirectories(repository))
            {
                if (IsCleanablePath(directory))
                {
                    DeleteDirectory(directory, isFakeClean);
                }
            }
        }

        private IEnumerable<string> GetPossibleNuGetPackageDirectories(string rootPath)
        {
            return Directory.GetDirectories(rootPath);
        }

        private IEnumerable<string> GetNuGetPackageDirectories(Repository repository)
        {
            var packagesDirectory = GetPackagesDirectory(repository);

            return (from directory in GetPossibleNuGetPackageDirectories(packagesDirectory)
                    where IsCleanablePath(directory)
                    select directory);
        }

        private bool IsCleanablePath(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            return Directory.GetFiles(path, "*.nupkg", SearchOption.AllDirectories).Any();
        }
    }
}
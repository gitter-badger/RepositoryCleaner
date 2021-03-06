﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntermediateFilesCleaner.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2015 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace RepositoryCleaner.Cleaners
{
    using System.IO;
    using System.Linq;
    using Catel.Logging;
    using Models;

    [Cleaner("OutputDirectoryCleaner", Description = "Delete output directories such as the bin\\debug directory")]
    public class OutputDirectoryCleaner : MsProjectsCleanerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected override bool CanCleanRepository(Repository repository)
        {
            var projects = GetAllProjects(repository);
            return projects.Count() > 0;
        }

        protected override long CalculateCleanableSpaceForRepository(Repository repository)
        {
            var size = 0L;

            var projects = GetAllProjects(repository);

            foreach (var project in projects)
            {
                var intermediateDirectory = project.GetTargetDirectory();

                size += GetDirectorySize(intermediateDirectory);
            }

            return size;
        }

        protected override void CleanRepository(Repository repository, bool isFakeClean)
        {
            var projects = GetAllProjects(repository);

            foreach (var project in projects)
            {
                var intermediateDirectory = project.GetTargetDirectory();

                DeleteDirectory(intermediateDirectory, isFakeClean);
            }
        }
    }
}
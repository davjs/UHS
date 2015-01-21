﻿namespace Cycles
{
    using System;
    using Microsoft.VisualStudio.VCProjectEngine;
    using Cycles.Utils;
    using EnvDTE;

    public class ProjectHolder
    {
        public EnvDTE.Project dteproj;
        public VCProject vcProj;
        public VCFilter headers;
        public VCFilter sources;
        public VCFilter unifiles;


	    public ProjectHolder(EnvDTE.DTE enviro)
	    {
            tryWhileFail.execute(() =>
            {
                dteproj = enviro.ActiveDocument.ProjectItem.ContainingProject;
                vcProj = (VCProject)dteproj.Object;
            });
            load();
	    }

        internal void load()
        {
            headers = findFilter("Header Files");
            sources = findFilter("Source Files");
            unifiles = findFilter("Unified Files");
            if (unifiles == null)
            {
                unifiles = vcProj.AddFilter("Unified Files");
            }
        }
        public ProjectHolder(EnvDTE80.DTE2 enviro,String projectname)
        {
            while (dteproj == null)
            {
                tryWhileFail.execute(() =>
                {
                    foreach (Project project in enviro.Solution.Projects)//GetObject("VCProjects"))
                    {
                        if (project.Name == projectname)
                        {
                            this.dteproj = project;
                            vcProj = (VCProject)project.Object;
                            break;
                        }
                    }
                });
            }

            load();
        }
        public VCFilter findFilter(String fname, VCFilter filter = null)
        {
            dynamic filters;

            if (filter == null)
            {
                filters = vcProj.Filters;
            }
            else
            {
                filters = filter.Filters;
            }

            foreach (VCFilter f in filters)
            {
                if (f.Name == fname)
                    return f;
                VCFilter found = findFilter(fname, f);
                if (found != null)
                    return found;
            }

            return null;
        }
        public VCFile findFile(String fname,VCFilter filter = null)
        {
            dynamic files;
            dynamic filters;

            if (filter == null)
            {
                files = vcProj.Files;
                filters = vcProj.Filters;
            }
            else
            {
                files = filter.Files;
                filters = filter.Filters;
            }

            foreach(VCFile f in files)
            {
                if (fname == f.FullPath)
                    return f;
            }

            foreach (VCFilter f in filters)
            {
                VCFile found = findFile(fname, f);
                if (found != null)
                    return found;
            }

            return null;
        }

        internal VCFile findHeader(string fname)
        {
            return findFile(fname, headers);
        }

        internal VCFile findSource(string fname)
        {
            return findFile(fname, sources);
        }
    }
}

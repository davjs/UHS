﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using Microsoft.VisualStudio.VCProjectEngine;
using Cycles;
using Cycles.converting;

namespace Cycles
{
    public class UHSGenerator
    {
        //intellicon intellisence;
        public ProjectHolder project;
        public bool converting = false;
        public UHSGenerator(EnvDTE.Project project)
        {
            this.project = new ProjectHolder((EnvDTE80.DTE2)project);
        }

        public UHSGenerator(ProjectHolder project)
        {
            this.project = project;
        }

        public void convert(VCFile file)
        {
            converting = true;

            //Create header

            VCFile h = project.findHeader(file.FullPath.Split('.')[0] + ".hpp");
            if (h == null)
                h = project.findHeader(file.FullPath.Split('.')[0] + ".h");
            VCFile s = project.findSource(file.FullPath.Split('.')[0] + ".cpp");

            ProjectItem header = h.Object as ProjectItem;
            ProjectItem source = s.Object as ProjectItem;
            
            bool hOpen = header.Document != null;
            bool sOpen = source.Document != null;
            if(hOpen)    
                header.Document.Close();
            if (sOpen)
                source.Document.Close();
            
            System.IO.File.WriteAllText(h.FullPath, String.Empty);
            System.IO.File.WriteAllText(s.FullPath, String.Empty);
            
            tryWhileFail.execute(()=>{
            project.proj.Save();
            project.dteproj.Save();
            });
            
            EnvDTE.ProjectItem uhsfile = file.Object as ProjectItem;
            VCFileCodeModel vcfile = null, vcheader, vcsource;
            tryWhileFail.execute(() =>
            {
                vcfile = uhsfile.FileCodeModel as VCFileCodeModel;
                vcheader = header.FileCodeModel as VCFileCodeModel;
                vcsource = source.FileCodeModel as VCFileCodeModel;
            });

            
            System.Collections.IEnumerator num = null;
            tryWhileFail.execute(() =>
            {
                num = vcfile.CodeElements.GetEnumerator();
            });
            while (num.MoveNext())
            {
                VCCodeElement el = num.Current as VCCodeElement;
                uhsconverter.parseitem(el, source, new CodeHolder(header.FileCodeModel as VCFileCodeModel));
            }

            (source.FileCodeModel as VCFileCodeModel).AddInclude("\"" + header.Name + "\"");
            

            project.dteproj.Save();

            //tryWhileFail.execute(()=>{
            //if ((source.FileCodeModel as VCFileCodeModel).Includes.Count == 0)
            
                //(source.FileCodeModel as VCFileCodeModel).StartPoint.CreateEditPoint().Insert("#include \"" + header.Name + "\""); 
            //});
            converting = false;
            
            //Reopen docs
            if (hOpen) header.Open();
            if (sOpen) source.Open();
        }

        
    }
}



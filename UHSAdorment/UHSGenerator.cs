﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;
using Microsoft.VisualStudio.VCCodeModel;
using Microsoft.VisualStudio.VCProjectEngine;
using Cycles;
using Cycles.Utils;
using Cycles.Converting;
using Cycles.Converting.CodeHolders;

namespace Cycles
{
    public class UHSGenerator
    {
        public ProjectHolder project;
        public bool converting = false;

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
            if (hOpen) header.Document.Close();
            if (sOpen) source.Document.Close();

            System.IO.File.WriteAllText(h.FullPath, String.Empty);
            (header.FileCodeModel as VCFileCodeModel).Synchronize();
            System.IO.File.WriteAllText(s.FullPath, String.Empty);
            (source.FileCodeModel as VCFileCodeModel).Synchronize();
            (source.FileCodeModel as VCFileCodeModel).StartPoint.CreateEditPoint().Delete(
               (source.FileCodeModel as VCFileCodeModel).EndPoint);

            tryWhileFail.execute(() =>
            {
                project.vcProj.Save();
                project.dteproj.Save();
            }, false);

            EnvDTE.ProjectItem uhsfile = file.Object as ProjectItem;
            VCFileCodeModel vcfile = null, vcheader = null, vcsource = null;
            tryWhileFail.execute(() =>
            {
                vcfile = uhsfile.FileCodeModel as VCFileCodeModel;
                vcheader = header.FileCodeModel as VCFileCodeModel;
                vcsource = source.FileCodeModel as VCFileCodeModel;
            });

            if (vcfile == null)
            {
                System.Windows.MessageBox.Show("The UHS format needs to be interpreted as C++ code by visual studio. "
                    + "Currently you have to set this manually by going TOOLS->Options->Text Editor->File Extensions and adding the extension uhs using editor C++. "
                    + "The UHS format wont even work partly otherwise.\n\n If you still get this error it means visual studio is stuck parsing your solution, delete *.suo and *.sdf and restart");
                throw new Exceptions.UHSNotCppException();
            }

            System.Collections.IEnumerator num = null;
            tryWhileFail.execute(() =>
            {
                num = vcfile.CodeElements.GetEnumerator();
            });
            while (num.MoveNext())
            {
                VCCodeElement el = num.Current as VCCodeElement;
                UHSConverter.parseitem(el, source, CodeHolder.newHolder(header.FileCodeModel as VCFileCodeModel));
            }

            vcheader.StartPoint.CreateEditPoint().Insert("#pragma once\r\n");
            bool hasHeader = false;
            foreach (VCCodeInclude inc in vcsource.Includes)
            {
                if (inc.Name == header.Name)
                {
                    hasHeader = true; break;
                }
            }
            if (!hasHeader)
                vcsource.AddInclude("\"" + header.Name + "\"");

            project.dteproj.Save();
            converting = false;

            //Reopen docs
            if (hOpen) header.Open();
            if (sOpen) source.Open();
        }


    }
}



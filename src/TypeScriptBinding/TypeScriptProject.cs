﻿// 
// TypeScriptProject.cs
// 
// Author:
//   Matt Ward <ward.matt@gmail.com>
// 
// Copyright (C) 2013 Matthew Ward
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptProject
	{
		IProject project;
		
		public TypeScriptProject(IProject project)
		{
			this.project = project;
		}
		
		public string Name {
			get { return project.Name; }
		}
		
		public void AddMissingFiles(IEnumerable<GeneratedTypeScriptFile> filesGenerated)
		{
			foreach (GeneratedTypeScriptFile file in filesGenerated) {
				AddMissingFile(file);
			}
			project.Save();
		}
		
		void AddMissingFile(GeneratedTypeScriptFile file)
		{
			if (IsFileInProject(file.FileName) || !File.Exists(file.FileName)) {
				return;
			}
			
			var projectItem = new FileProjectItem(project, ItemType.None);
			projectItem.FileName = file.FileName;
			projectItem.DependentUpon = file.GetDependentUpon();
			
			ProjectService.AddProjectItem(project, projectItem);
		}
		
		public bool IsFileInProject(FileName fileName)
		{
			return project.IsFileInProject(fileName);
		}
		
		public bool HasTypeScriptFiles()
		{
			return project
				.Items
				.Any(item => TypeScriptParser.IsTypeScriptFileName(item.FileName));
		}
		
		public IEnumerable<FileName> GetTypeScriptFileNames()
		{
			return project
				.Items
				.Where(item => TypeScriptParser.IsTypeScriptFileName(item.FileName))
				.Select(item => new FileName(item.FileName));
		}
	}
}

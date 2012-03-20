// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

using dropkick.FileSystem;

namespace dropkick.Configuration.Dsl.Files
{
	using System.Collections.Generic;
	using System.Text;
	using System.Text.RegularExpressions;
	using DeploymentModel;
	using Tasks;
	using Tasks.Files;	
	
	public class ProtoFilePokeTask : BaseProtoTask, FilePokeOptions
	{
		private readonly string _filePath;
		private readonly Encoding _encoding;
		private IList<FilePokeReplacement> _replacements = new List<FilePokeReplacement>();

		public ProtoFilePokeTask(string filePath, Encoding encoding)
		{
			_filePath = filePath;
			_encoding = encoding;
		}

		public override void RegisterRealTasks(PhysicalServer server)
		{
			server.AddTask(new FilePokeTask(
				server.MapPath(ReplaceTokens(_filePath)),
				_encoding,
				_replacements,
				new DotNetPath()));
		}

		public FilePokeOptions Replace(string pattern, string replacement)
		{
			return Replace(pattern, replacement, RegexOptions.None);
		}

		public FilePokeOptions Replace(string pattern, string replacement, RegexOptions regexOptions)
		{
			_replacements.Add(new FilePokeReplacement(
				ReplaceTokens(pattern),
				ReplaceTokens(replacement),
				regexOptions));
			return this;
		}
	}
}
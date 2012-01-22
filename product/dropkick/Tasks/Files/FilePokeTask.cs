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
namespace dropkick.Tasks.Files
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using DeploymentModel;

	public class FilePokeTask
		: BaseIoTask
	{
		private readonly string _filePath;
		private readonly Encoding _encoding = Encoding.Default;
		private readonly IList<FilePokeReplacement> _replacements;

		public FilePokeTask(string filePath, IList<FilePokeReplacement> replacements, FileSystem.Path path)
			: base(path)
		{
			_filePath = path.GetFullPath(filePath);
			_replacements = replacements;
		}

		public FilePokeTask(string filePath, Encoding encoding, IList<FilePokeReplacement> replacements, FileSystem.Path path)
			: this(filePath, replacements, path)
		{
			_encoding = encoding;
		}

		public override string Name
		{
			get { return string.Format("Updated values in '{0}'", _filePath); }
		}

		public override DeploymentResult VerifyCanRun()
		{
			var result = new DeploymentResult();
			ValidateIsFile(result, _filePath);
			result.AddGood(Name);
			return result;
		}

		public override DeploymentResult Execute()
		{
			var result = VerifyCanRun();

			if (!result.ContainsError())
				PerformReplacements(result);

			return result;
		}

		private void PerformReplacements(DeploymentResult result)
		{
			var contents = File.ReadAllText(_filePath, _encoding);
			foreach (var replacement in _replacements)
				contents = replacement.Replace(contents, result);
			File.WriteAllText(_filePath, contents, _encoding);
		}
	}
}
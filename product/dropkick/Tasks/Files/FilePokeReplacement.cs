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
	using System.Text.RegularExpressions;
	using DeploymentModel;
	using log4net;

	public class FilePokeReplacement
	{
		private readonly ILog _log = Logging.WellKnown.FileChanges;

		private readonly string _pattern;
		private readonly string _replacement;
		private readonly RegexOptions _regexOptions;

		public FilePokeReplacement(string pattern, string replacement)
		{
			_pattern = pattern;
			_replacement = replacement;
		}

		public FilePokeReplacement(string pattern, string replacement, RegexOptions regexOptions)
			: this(pattern, replacement)
		{
			_regexOptions = regexOptions;
		}

		public string Replace(string contents, DeploymentResult result)
		{
			var newContents = Regex.Replace(contents, _pattern, _replacement, _regexOptions);

			if (newContents == contents)
				result.AddAlert("[filepoke] No replacement made for pattern '{0}'.", _pattern);
			else
				_log.InfoFormat("[filepoke] Pattern '{0}' replaced with '{1}' using options {2}.", _pattern, _replacement, _regexOptions);

			return newContents;
		}
	}
}
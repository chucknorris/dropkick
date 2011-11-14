// Copyright 2007-2008 The Apache Software Foundation.
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
namespace dropkick.Tasks.CommandLine
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using DeploymentModel;
    using Magnum.Extensions;
    using Path = FileSystem.Path;

    public class LocalCommandLineTask :
        Task
    {
        readonly Path _path;

        public LocalCommandLineTask(Path path, string command)
        {
            _path = path;
            WorkingDirectory = Environment.CurrentDirectory;
            Command = command;
            ExecutableIsLocatedAt = FindThePathToTheCommand(command);
        }


        public string Command { get; set; }
        public string Args { get; set; }
        public string ExecutableIsLocatedAt { get; set; }
        public string WorkingDirectory { get; set; }

        #region Task Members

        public string Name
        {
            get
            {
                string name = string.IsNullOrEmpty(ExecutableIsLocatedAt) ? "'{0} {1}' using PATH" : "{0} {1} in {2}";
                return "COMMAND LINE: " + string.Format(name, Command, Args, ExecutableIsLocatedAt);
            }
        }

        public DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            if (!Directory.Exists(ExecutableIsLocatedAt))
                result.AddAlert(string.Format("Can't find the executable '{0}'", _path.Combine(ExecutableIsLocatedAt, Command)));

            if (IsTheExeInThisDirectory(ExecutableIsLocatedAt, Command))
                result.AddGood(string.Format("Found command '{0}' in '{1}'", Command, ExecutableIsLocatedAt));

            return result;
        }

        public DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var psi = new ProcessStartInfo(Command, Args);

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;

            if (!string.IsNullOrEmpty(WorkingDirectory)) psi.WorkingDirectory = WorkingDirectory;

            psi.FileName = _path.Combine(WorkingDirectory, Command);
            
            if (!File.Exists(psi.FileName))
                psi.FileName = _path.Combine(ExecutableIsLocatedAt, Command); // this should be the correct path checked in VerifyCanRun

            string output;
            try
            {
                using (Process p = Process.Start(psi))
                {
                    //what to do here?
                    p.WaitForExit(30.Seconds().Milliseconds);
                    output = p.StandardOutput.ReadToEnd();
                    result.AddNote(output);
                }

                result.AddGood("Command Line Executed");
            }
            catch (Win32Exception ex)
            {
                result.AddError(
                    "An exception occured while attempting to execute the following remote command.  Working Directory:'{0}' Command:'{1}' Args:'{2}'{3}{4}"
                        .FormatWith(WorkingDirectory, Command, Args, Environment.NewLine, ex));
            }

            return result;
        }

        #endregion

        string FindThePathToTheCommand(string command)
        {
            var result = WorkingDirectory;

            var path = Environment.GetEnvironmentVariable("PATH");
            foreach (var dir in path.Split(';'))
            {
                if (Directory.Exists(dir) && IsTheExeInThisDirectory(dir, command))
                {
                    result = dir;
                    break;
                }
            }
            return result;
        }

        bool IsTheExeInThisDirectory(string dir, string command)
        {
            if (!Directory.Exists(dir))
            {
                return false;
            }


            var files = Directory.GetFiles(dir);
            foreach (var file in files)
            {
                var name = _path.GetFileNameWithoutExtension(file);
                if (name.Equals(command, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
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
using System.Text;

namespace dropkick.Tasks.Security.Certificate
{
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using CommandLine;
    using System.Security.AccessControl;
    using DeploymentModel;
    using FileSystem;
    using System.Linq;


    public class GrantReadCertificatePrivateKeyTask :
        BaseSecurityCertificatePermissionsTask
    {
        private readonly string _thumbprint;
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;
        private readonly IList<string> _groups;
        private readonly Path _dotNetPath;
        private readonly PhysicalServer _server;

        public GrantReadCertificatePrivateKeyTask(PhysicalServer server, IList<string> groups, string thumbprint, StoreName storeName, StoreLocation storeLocation, Path dotNetPath)
        {
            _dotNetPath = dotNetPath;
            _server = server;
            _thumbprint = thumbprint;
            _storeName = storeName;
            _storeLocation = storeLocation;
            _groups = groups;
        }

        public override string Name
        {
            get { return "Grant Read for private key of X509 Certificate for thumbprint '{0}'".FormatWith(_thumbprint); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();
            base.VerifyInAdministratorRole(result);

            if (_server.IsLocal)
            {
                var cert = FindCertificateBy(_thumbprint, _storeName, _storeLocation, _server, result);
                if (cert == null)
                {
                    result.AddError("Certificate with thumbprint '{0}' was not found in the '{1}' \\ '{2}' store.".FormatWith(_thumbprint, _storeLocation, _storeName));
                }
            }
            else
            {
                result.AddAlert("Cannot verify if '{0}' exists because '{1}' is a remote server".FormatWith(_thumbprint, _server.Name));
            }


            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (_server.IsLocal) ExecuteLocalTask(result);
            else ExecuteRemoteTask(result);

            return result;
        }

        public void ExecuteLocalTask(DeploymentResult result)
        {
            var cert = FindCertificateBy(_thumbprint, _storeName, _storeLocation, _server, result);

            if (cert == null)
            {
                result.AddError("Certificate with thumbprint '{0}' was not found in the '{1}' \\ '{2}' store.".FormatWith(_thumbprint, _storeLocation, _storeName));

                return;
            }

            foreach (string group in _groups)
            {
                AddAccessToPrivateKey(cert, group, FileSystemRights.Read, _storeLocation, _dotNetPath, _server, result);
                LogSecurity("[security][cert] Granted READ to X509 Certificate's private key to '{0}' for thumbprint '{1}'", group, _thumbprint);
            }

            result.AddGood(Name);
        }

        public void ExecuteRemoteTask(DeploymentResult result)
        {
            LogSecurity("[security][cert][remote] Granting READ to X509 Certificate's private key for thumbprint '{0}' on '{1}'.", _thumbprint,_server);

            using (var remote = new RemoteDropkickExecutionTask(_server))
            {
                StringBuilder sentGroups = new StringBuilder();
                sentGroups.Append("|");
                foreach (string group in _groups)
                {
                    sentGroups.Append("{0}|".FormatWith(group));
                }

                var vresult = remote.GrantReadCertificatePrivateKey(sentGroups.ToString(), _thumbprint, _storeName.ToString(), _storeLocation.ToString());
                foreach (var r in vresult) result.Add(r);
            }
        }
    }
}
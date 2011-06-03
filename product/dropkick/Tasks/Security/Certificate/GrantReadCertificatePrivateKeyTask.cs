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
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace dropkick.Tasks.Security.Certificate
{
    using System.Security.AccessControl;
    using DeploymentModel;
    using FileSystem;

    public class GrantReadCertificatePrivateKeyTask :
        BaseSecurityCertificatePermissionsTask
    {
        private readonly string _thumbprint;
        private readonly StoreName _storeName;
        private readonly StoreLocation _storeLocation;
        private readonly IList<string> _groups;
        private readonly Path _dotNetPath;

        public GrantReadCertificatePrivateKeyTask(IList<string> groups, string thumbprint, StoreName storeName, StoreLocation storeLocation, Path dotNetPath)
        {
            _dotNetPath = dotNetPath;
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

            var cert = FindCertificateBy(_thumbprint, _storeName, _storeLocation);
            if (cert == null)
            {
                result.AddError("Certificate with thumbprint '{0}' was not found in the '{1}' \\ '{2}' store.".FormatWith(_thumbprint, _storeLocation, _storeName));
            }

            return result;
        }

        public override DeploymentResult Execute()
        {

            var result = new DeploymentResult();

            var cert = FindCertificateBy(_thumbprint, _storeName, _storeLocation);
            if (cert != null)
            {
                foreach (string group in _groups)
                {
                    AddAccessToPrivateKey(cert, group, FileSystemRights.Read, _storeLocation, _dotNetPath);
                    LogSecurity("[security][cert] Granted READ to X509 Certificate's private key to '{0}' for thumbprint '{1}'", group, _thumbprint);
                }

                result.AddGood(Name);
            }

            return result;
        }

    }
}
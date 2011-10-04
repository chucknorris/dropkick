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
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration;
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.Security;

    public class SecurityDeployment :
        Deployment<SecurityDeployment, SecuritySettings>
    {
        public SecurityDeployment()
        {
            Define((settings) =>
                {
                    DeploymentStepsFor(File, s =>
                    {
                        s.Security(o =>
                        {
                            o.LocalPolicy(p => p.LogOnAsService(settings.AppAccount));
                            o.ForPath(settings.InstallPath, p =>
                            {
                               
                                p.GrantRead(settings.UserGroupA);
                                p.GrantReadWrite(settings.UserGroupB);
                                p.Grant(Rights.ReadWrite, settings.UserGroupA);
                                p.Grant(Rights.ReadWrite, WindowsIdentity.GetCurrent().Name);
                                
                                p.RemoveInheritance();
                                p.Clear()
                                    .Preserve(settings.UserGroupA,settings.UserGroupB)
                                    .Preserve(WindowsIdentity.GetCurrent().Name)
                                    .RemoveAdministratorsGroup()
                                    .RemoveSystemAccount()
                                    .RemoveUsersGroup();
                            });

                            o.ForCertificate(settings.CertificateThumbPrint, c =>
                            {
                                c.GrantReadPrivateKey()
                                    .To(settings.UserGroupB)
                                    .InStoreLocation(StoreLocation.LocalMachine)
                                    .InStoreName(StoreName.My);
                            });
                                
                            o.ForQueue(settings.Queue, q =>
                            {
                                q.GrantRead(settings.TheRest);
                                q.GrantReadWrite(settings.AppAccount);
                            });

                            o.ForSqlServer(settings.Database, d =>
                            {
                                d.CreateALoginFor(settings.AppAccount);
                                d.CreateUserFor(settings.AppAccount)
                                    .PutInRole(settings.AppRole);

                                d.GrantDataReader(settings.AppRole);
                                d.GrantDataWriter(settings.AppRole);
                            });
                        });

                    });
                });
        }

        public static Role File { get; set; }
    }

    public class SecuritySettings :
        DropkickConfiguration
    {
        public string AppAccount { get; set; }
        public string InstallPath { get; set; }
        public string UserGroupA { get; set; }
        public string UserGroupB { get; set; }
        public string CertificateThumbPrint { get; set; }
        public string Queue { get; set; }
        public string TheRest { get; set; }
        public string Database { get; set; }

        public string AppRole { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using dropkick.DeploymentModel;
using dropkick.FileSystem;
using dropkick.Tasks;
using dropkick.Tasks.Security.Certificate;

namespace dropkick.Configuration.Dsl.Security.Certificate
{
    public class ProtoGrantReadX509CertificatePrivateKeyTask : BaseProtoTask, GrantReadCertificateOptions
    {
        private readonly string _thumbprint;
        private readonly IList<string> _groups = new List<string>();
        private StoreName _storeName;
        private StoreLocation _storeLocation;
        private Path _dotnetPath = new DotNetPath();

        public ProtoGrantReadX509CertificatePrivateKeyTask(string thumbprint)
        {
            _thumbprint = thumbprint;
        }

        public GrantReadCertificateOptions To(params string[] groupAndOrAccountNames)
        {
            foreach (string name in groupAndOrAccountNames)
            {
                _groups.Add(name);
            }

            return this;
        }

        public GrantReadCertificateOptions InStoreName(StoreName name)
        {
            _storeName = name;

            return this;
        }

        public GrantReadCertificateOptions InStoreLocation(StoreLocation location)
        {
            _storeLocation = location;

            return this;
        } 
        
        public override void RegisterRealTasks(PhysicalServer server)
        {
            var task = new GrantReadCertificatePrivateKeyTask(_groups, _thumbprint, _storeName, _storeLocation, _dotnetPath);
            server.AddTask(task);
        }
    }
}
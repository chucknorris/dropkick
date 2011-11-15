using System;
using Magnum;
using dropkick.DeploymentModel;
using dropkick.Tasks;
using dropkick.Tasks.Iis;

namespace dropkick.Configuration.Dsl.Iis
{
    public class IisSiteOperationProtoTask : BaseProtoTask, IisSiteOperation
    {
        string _siteName;
        Iis7SiteOperation _operation;

        public IisSiteOperationProtoTask(string siteName)
        {
            Guard.AgainstNull(siteName, "SiteName");
            _siteName = siteName;
        }

        public IisVersion Version { get; set; }

        public override void RegisterRealTasks(PhysicalServer server)
        {
            if (_operation == null) throw new InvalidOperationException("Site Operation not specified.");
            if (Version == IisVersion.Six) throw new NotSupportedException("Site Operations not supported on IIS 6.");

            server.AddTask(new Iis7SiteOperationTask
            {
                SiteName = _siteName,
                Operation = _operation,
                ServerName = server.Name
            });
        }

        public void Delete()
        {
            _operation = Iis7SiteOperation.DeleteSite;
        }
    }
}
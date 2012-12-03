using dropkick.DeploymentModel;
using dropkick.Tasks.CommandLine;

namespace dropkick.Tasks.Security.Certificate
{
    public static class RemoteExtension
    {
        public static DeploymentResult GrantReadCertificatePrivateKey(this RemoteDropkickExecutionTask remoteTask, string group, string thumbprint, string storeName, string storeLocation)
        {
            var t = remoteTask.SetUpRemote("grant_cert r \"{0}\" \"{1}\" \"{2}\" \"{3}\"".FormatWith(group, thumbprint.Trim().Replace(" ", ""), storeName, storeLocation));
            return remoteTask.ExecuteAndGetResults(t);
        } 
    }
}
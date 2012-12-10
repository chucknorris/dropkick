using dropkick.Configuration.Dsl.Msmq;
using dropkick.DeploymentModel;
using dropkick.Tasks.CommandLine;

namespace dropkick.Tasks.Msmq
{
    using System.Messaging;

    public static class RemoteExtension
    {
        public static DeploymentResult VerifyQueueExists(this RemoteDropkickExecutionTask remoteTask, QueueAddress path)
        {
            var t = remoteTask.SetUpRemote("verify_queue {0}".FormatWith(path.ActualUri));
            return remoteTask.ExecuteAndGetResults(t);
        }

        public static DeploymentResult CreateQueue(this RemoteDropkickExecutionTask remoteTask, QueueAddress path, bool transactional = false)
        {
            var t = remoteTask.SetUpRemote("create_queue {0} {1}".FormatWith(path.ActualUri, transactional));
            return remoteTask.ExecuteAndGetResults(t);
        }

        public static DeploymentResult GrantMsmqPermission(this RemoteDropkickExecutionTask remoteTask, QueuePermission permission, QueueAddress address, string @group)
        {
            string perm;
            switch (permission)
            {
                case QueuePermission.Read:
                    perm = "r";
                    break;
                case QueuePermission.Write:
                    perm = "w";
                    break;
                case QueuePermission.ReadWrite:
                    perm = "rw";
                    break;
                case QueuePermission.SetSensibleDefaults:
                    perm = "default";
                    break;
                default:
                    perm = "r";
                    break;
            }

            var t = remoteTask.SetUpRemote("grant_queue {0} {1} {2}".FormatWith(perm, @group, address.ActualUri));
            return remoteTask.ExecuteAndGetResults(t);
        }

        public static DeploymentResult GrantMsmqAccessRights(this RemoteDropkickExecutionTask remoteTask, MessageQueueAccessRights accessRights, QueueAddress address, string @group)
        {
            var t = remoteTask.SetUpRemote("grant_queue {0} {1} {2}".FormatWith((int)accessRights, @group, address.ActualUri));
            return remoteTask.ExecuteAndGetResults(t);
        }

        public static DeploymentResult GrantMsmqNServiceBusPermissions(this RemoteDropkickExecutionTask remoteTask, QueueAddress address, string user)
        {
            var t = remoteTask.SetUpRemote("grant_queue NServiceBus {1} {2}".FormatWith(user, address.ActualUri));
            return remoteTask.ExecuteAndGetResults(t);
        }
    }

    public enum QueuePermission
    {
        Read,
        Write,
        ReadWrite,
        SetSensibleDefaults
    }
}
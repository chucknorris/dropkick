namespace dropkick.Configuration.Dsl.NServiceBusHost
{
    using DeploymentModel;
    using Msmq;
    using Security.Msmq;

    public class NServiceBusHostQueues
    {
        bool _createRetriesQueue;
        bool _createErrorsQueue;
        bool _createSubscriptionsQueue;
        bool _createAuditQueue;
        bool _createTimeoutQueues;

        public NServiceBusHostQueues CreateRetriesQueue()
        {
            _createRetriesQueue = true;
            return this;
        }

        public NServiceBusHostQueues CreateErrorsQueue()
        {
            _createErrorsQueue = true;
            return this;
        }
        public NServiceBusHostQueues CreateSubscriptionsQueue()
        {
            _createSubscriptionsQueue = true;
            return this;
        }
        public NServiceBusHostQueues CreateAuditQueue()
        {
            _createAuditQueue = true;
            return this;
        }
        public NServiceBusHostQueues CreateTimeoutQueues()
        {
            _createTimeoutQueues = true;
            return this;
        }

        public void RegisterRealTasks(PhysicalServer site, string serviceName, string username)
        {
            RegisterMsmqQueueCreation(site, username, serviceName);

            if (_createRetriesQueue)
                RegisterMsmqQueueCreation(site, username, serviceName + ".retries");
            if (_createErrorsQueue)
                RegisterMsmqQueueCreation(site, username, serviceName + ".errors");
            if (_createSubscriptionsQueue)
                RegisterMsmqQueueCreation(site, username, serviceName + ".subscriptions");
            if (_createAuditQueue)
                RegisterMsmqQueueCreation(site, username, serviceName + ".audit");
            if (_createTimeoutQueues)
            {
                RegisterMsmqQueueCreation(site, username, serviceName + ".timeouts");
                RegisterMsmqQueueCreation(site, username, serviceName + ".timeoutsdispatcher");
            }
        }

        void RegisterMsmqQueueCreation(PhysicalServer site, string username, string queueName)
        {
            var protoMsmqTask = new ProtoMsmqTask();
            protoMsmqTask.PrivateQueue(queueName).Transactional();
            protoMsmqTask.RegisterRealTasks(site);

            var msmqSecurity = new ProtoMsmqNServiceBusPermissionsTask(queueName, username);
            msmqSecurity.RegisterRealTasks(site);
        }
    }
}
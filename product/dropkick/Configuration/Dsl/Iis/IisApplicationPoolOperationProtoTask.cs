﻿using System;
using Magnum;
using dropkick.DeploymentModel;
using dropkick.Tasks;
using dropkick.Tasks.Iis;

namespace dropkick.Configuration.Dsl.Iis
{
    public class IisApplicationPoolOperationProtoTask : BaseProtoTask, IisApplicationPoolOperation
    {
        readonly string _applicationPoolName;
        Iis7ApplicationPoolOperation _operation;

        public IisApplicationPoolOperationProtoTask(string applicationPoolName)
        {
            Guard.AgainstNull(applicationPoolName, "ApplicationPoolName");
            _applicationPoolName = applicationPoolName;
        }

        public IisVersion Version { get; set; }

        public void Start()
        {
            _operation = Iis7ApplicationPoolOperation.StartApplicationPool;
        }

        public void Stop()
        {
            _operation = Iis7ApplicationPoolOperation.StopApplicationPool;
        }

        public override void RegisterRealTasks(PhysicalServer server)
        {
            if (_operation == Iis7ApplicationPoolOperation.Unspecified) throw new InvalidOperationException("Application Pool Operation not specified.");
            if (Version == IisVersion.Six) throw new NotSupportedException("Application Pool Operations not supported on IIS 6.");

            server.AddTask(new Iis7ApplicationPoolOperationTask
            {
                ApplicationPool = _applicationPoolName,
                Operation = _operation,
                ServerName = server.Name
            });
        }
    }
}

using System;
using dropkick.DeploymentModel;

namespace dropkick.tests.TestObjects
{
    public class TestPhysicalServer : PhysicalServer
    {
        public string MappedPath { get; private set; }
        public Task Task { get; private set; }

        string PhysicalServer.Name
        {
            get { throw new NotImplementedException(); }
        }

        bool PhysicalServer.IsLocal
        {
            get { throw new NotImplementedException(); }
        }

        void PhysicalServer.AddTask(Task task)
        {
            Task = task;
        }

        string PhysicalServer.MapPath(string path)
        {
            MappedPath = path;
            return path;
        }
    }
}
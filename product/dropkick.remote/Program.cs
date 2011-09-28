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
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using dropkick.DeploymentModel;
using dropkick.FileSystem;
using dropkick.Tasks.Msmq;
using dropkick.Tasks.Security.Certificate;
using log4net.Config;

namespace dropkick.remote
{
    using System;
    using System.IO;
    using System.Messaging;
    using Configuration.Dsl.Msmq;
    using Tasks.Security.Msmq;

    internal class Program
    {
        private static PhysicalServer _server = new DeploymentServer("localhost");

        //dropkick.remote create_queue msmq://servername/dk_remote
        //dropkick.remote verify_queue msmq://servername/dk_remote
        //dropkick.remote grant [r|w|rw] username msmq://servername/dk_remote
        static void Main(string[] args)
        {
            try
            {
                var logpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dk.log4net.xml");
                XmlConfigurator.Configure(new FileInfo(logpath));
                Logging.SetRunAppender();

                if (args == null || args.Length == 0)
                {
                    Logging.Coarse("Remote needs arguments passed to it to run anything");
                    Environment.Exit(-1);
                }

                //TODO:remote needs to become as awesome as the regular console

                DeploymentResult result = new DeploymentResult();

                switch (args[0])
                {
                    case "create_queue":
                        result = CreateMsmq(args);
                        break;
                    case "verify_queue":
                        result =VerifyMsmqExists(args);
                        break;
                    case "grant_queue":
                        result =GrantMsmqPermissions(args);
                        break;
                    case "grant_cert":
                        result = GrantCertificatePermissions(args);
                        break;
                }

                DisplayResults(result);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.txt", ex.ToString());
                Logging.Coarse(LogLevel.Error, "[Error] {0}", ex);
                Environment.Exit(21);
            }
        }

        private static void DisplayResults(DeploymentResult results)
        {
            foreach (var result in results)
            {
                if (result.Status == DeploymentItemStatus.Error) Logging.Coarse(LogLevel.Error, "[{0,-5}] {1}", result.Status, result.Message);
                if (result.Status == DeploymentItemStatus.Alert) Logging.Coarse(LogLevel.Warn, "[{0,-5}] {1}", result.Status, result.Message);
                if (result.Status == DeploymentItemStatus.Good) Logging.Coarse(LogLevel.Info, "[{0,-5}] {1}", result.Status, result.Message);
                if (result.Status == DeploymentItemStatus.Note) Logging.Fine(LogLevel.Info, "[{0,-5}] {1}", result.Status, result.Message);
                if (result.Status == DeploymentItemStatus.Verbose) Logging.Fine(LogLevel.Debug, "[{0,-5}] {1}", result.Status, result.Message);
            }
        }

        private static DeploymentResult VerifyMsmqExists(string[] args)
        {
            DeploymentResult result = new DeploymentResult();
            var queuename = args[1];
            var queueAddress = new QueueAddress(queuename);
            var formattedName = queueAddress.LocalName;
            var exists = MessageQueue.Exists(formattedName);
            if (exists) result.AddGood("{0} exists on {1}.".FormatWith(queueAddress.LocalName),_server);
            Logging.Coarse("exists");

            return result;
        }

        private static DeploymentResult CreateMsmq(string[] args)
        {
            DeploymentResult result = new DeploymentResult();

            var queuename = args[1];
            var queueAddress = new QueueAddress(queuename);
            var transactional = false;
            if (args.Length > 2)
                bool.TryParse(args[2], out transactional);

            result = new CreateLocalMsmqQueueTask(_server, queueAddress, transactional).Execute();

            return result;
        }

        private static DeploymentResult GrantMsmqPermissions(string[] args)
        {
            DeploymentResult result = new DeploymentResult();

            var perm = args[1];
            var user = args[2];
            var queue = args[3];

            var queueAddress = new QueueAddress(queue);

            switch (perm)
            {
                case "r":
                    result = new LocalMsmqGrantReadTask(queueAddress, user).Execute();
                    break;
                case "w":
                    result = new MsmqGrantWriteTask(queueAddress, user).Execute();
                    break;
                case "rw":
                    result = new LocalMsmqGrantReadWriteTask(queueAddress, user).Execute();
                    break;
                case "default":
                    result = new SetSensibleMsmqDefaults(queueAddress).Execute();
                    break;
            }

            return result;
        }

        private static DeploymentResult GrantCertificatePermissions(string[] args)
        {
            DeploymentResult result = new DeploymentResult();

            var perm = args[1];
            var groupArray = args[2];
            var thumbprint = args[3];
            string s_storeName = args[4];
            var s_storeLocation = args[5];

            var groups = groupArray.Split(new[]{"|"},StringSplitOptions.RemoveEmptyEntries);

            StoreName storeName = (StoreName)Enum.Parse(typeof(StoreName), s_storeName,true);
            StoreLocation storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), s_storeLocation,true);

            switch (perm)
            {
                case "r":
                    result = new GrantReadCertificatePrivateKeyTask(_server, groups, thumbprint, storeName, storeLocation, new DotNetPath()).Execute();
                    break;
            }

            return result;
        }
    }
}
using dropkick.DeploymentModel;
using dropkick.Tasks.WinService;
using dropkick.Wmi;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace dropkick.tests.Tasks.WinService
{
    [TestFixture]
    [Category("Integration")]
    public class WinTestsWithAuthentication
    {
        public class WmiAuthenticationInfo
        {
            public string MachineName { get; set; }
            public string WmiUserName { get; set; }
            public string WmiPassword { get; set; }
            public string ServiceUserName { get; set; }
            public string ServicePassword { get; set; }
        }

        private WmiAuthenticationInfo GetAuthenticationInfo()
        {
            string path = System.IO.Path.GetFullPath("WmiAuthenticationInfo.xml");
            if (!System.IO.File.Exists(path))
            {
                throw new Exception("Please create a settings file first at: " + path);
            }
            var serializer = new XmlSerializer(typeof(WmiAuthenticationInfo));
            using (var reader = new System.IO.StreamReader(path))
            {
                return (WmiAuthenticationInfo)serializer.Deserialize(reader);
            }
        }  

        [Test]
        [Explicit]
        [Category("Integration")]
        public void Start()
        {
            var authInfo = GetAuthenticationInfo();

            var t = new WinServiceStopTask(authInfo.MachineName, "IISADMIN", authInfo.WmiUserName, authInfo.WmiPassword);
            var verifyStopResult = t.VerifyCanRun();
            Log(verifyStopResult);
            AssertSuccess(verifyStopResult);
            
            var stopResult = t.Execute();
            Log(stopResult);
            AssertSuccess(stopResult);

            var t2 = new WinServiceStartTask(authInfo.MachineName, "IISADMIN", authInfo.WmiUserName, authInfo.WmiPassword);
            var verifyStartResult = t2.VerifyCanRun();
            Log(verifyStartResult);
            AssertSuccess(verifyStartResult);

            var startResult = t2.Execute();
            Log(startResult);
            AssertSuccess(startResult);
        }

        [Test]
        [Explicit]
        public void RemoteCreate()
        {
            var authInfo = GetAuthenticationInfo();

            var t = new WinServiceCreateTask(authInfo.MachineName, "DropKicKTestService", authInfo.WmiUserName, authInfo.WmiPassword);

            t.ServiceLocation = "C:\\Test\\TestService.exe";
            t.StartMode = ServiceStartMode.Automatic;
            t.UserName = authInfo.ServiceUserName;
            t.Password = authInfo.ServicePassword;

            DeploymentResult o = t.VerifyCanRun();
            AssertSuccess(o);
            var result = t.Execute();
            Log(result);
            AssertSuccess(result);
        }

        [Test]
        [Explicit]
        [Category("Integration")]
        public void RemoteDelete()
        {
            var authInfo = GetAuthenticationInfo();

            var t = new WinServiceDeleteTask(authInfo.MachineName, "DropkicKTestService", authInfo.ServiceUserName, authInfo.ServicePassword);

            DeploymentResult o = t.VerifyCanRun();
            Log(o);
            AssertSuccess(o);
            var result = t.Execute();
            Log(result);
            AssertSuccess(result);
        }

        private void AssertSuccess(DeploymentResult result)
        {
            Assert.IsFalse(result.Any(i => i.Status == DeploymentItemStatus.Alert || i.Status == DeploymentItemStatus.Error));
        }

        private void Log(DeploymentResult result)
        {
            if(result != null)
            {
                foreach(var item in result)
                {
                    Debug.WriteLine(item.Message);
                }
            }
        }

    }
}

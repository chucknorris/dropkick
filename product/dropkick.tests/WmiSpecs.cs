namespace dropkick.tests
{
    using System;
    using System.Management;
    using NUnit.Framework;
    using Wmi;

    public class WmiSpecs
    {
        [Explicit]
        public void Bob()
        {
            var connOptions = new ConnectionOptions
                                  {
                                      EnablePrivileges = true,
                                      //Username = "username",
                                      //Password = "password"
                                  };

            string server = "SrvTestWeb01";
            ManagementScope manScope = new ManagementScope(String.Format(@"\\{0}\ROOT\CIMV2", server), connOptions);
            manScope.Connect();
            

            var managementPath = new ManagementPath("Win32_Process");
            var processClass = new ManagementClass(manScope, managementPath, new ObjectGetOptions());

            var inParams = processClass.GetMethodParameters("Create");
            inParams["CommandLine"] = @"C:\Temp\dropkick.remote\dropkick.remote.exe create_queue msmq://localhost/dk_test";

            var outParams = processClass.InvokeMethod("Create", inParams, null);

            var rtn = System.Convert.ToUInt32(outParams["returnValue"]);
            var processID = System.Convert.ToUInt32(outParams["processId"]);
        }

        [Explicit]
        public void Bill()
        {
            var p = WmiProcess.Run("SrvTestWeb01", "ping", "www.google.com", "");
        }
    }
}
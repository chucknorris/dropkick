namespace dropkick.Wmi
{
    using System;
    using System.Diagnostics;
    using System.Management;
    using System.Threading;

    //Reference http://msdn.microsoft.com/en-us/library/aa389388(v=VS.85).aspx
    //Reference http://www.dalun.com/blogs/05.09.2007.htm

    public class WmiProcess
    {
        const string CLASSNAME = "Win32_Process";
        //private char NULL_VALUE = char(0);


        public static ProcessReturnCode Run(string machineName, string commandLine, string args, string currentDirectory)
        {
                var connOptions = new ConnectionOptions
                                      {
                                          EnablePrivileges = true
                                      };


                var scope = new ManagementScope(@"\\{0}\ROOT\CIMV2".FormatWith(machineName), connOptions);
                scope.Connect();
                var managementPath = new ManagementPath(CLASSNAME);
                using(var processClass = new ManagementClass(scope, managementPath, new ObjectGetOptions()))
                {
                    var inParams = processClass.GetMethodParameters("Create");
                    commandLine = System.IO.Path.Combine(currentDirectory, commandLine);
                    inParams["CommandLine"] = "{0} {1}".FormatWith(commandLine, args);

                    var outParams = processClass.InvokeMethod("Create", inParams, null);

                    var rtn = Convert.ToUInt32(outParams["returnValue"]);
                    var pid = Convert.ToUInt32(outParams["processId"]);

                    WaitForPidToDie(machineName, pid);
                    return (ProcessReturnCode)rtn;
                }
            }


        static void WaitForPidToDie(string machineName, uint pid)
        {
            const int sleepInterval = 200;
            const int totalAttemptsAllowed = 5;
            var numberOfAttempts = 0;

            while (PidExists(machineName, pid))
            {
                numberOfAttempts++;
                Logging.Fine("[wmi] waiting for pid '{0}' to die", pid);

                var sleep = sleepInterval + numberOfAttempts;
                Logging.Fine("[wmi] sleeping for '{0}' milliseconds",sleep);
                Thread.Sleep(sleep);

                if (numberOfAttempts > totalAttemptsAllowed)
                {
                    Logging.Fine("Tried waiting for '{0}' times, pid stil alive.", numberOfAttempts);
                    break;
                }
            }
        }

        static bool PidExists(string machineName, uint pid)
        {
            try
            {
                var p = Process.GetProcessById((int) pid, machineName);
                return true;

            }
            catch (ArgumentException ex)
            {
                return false;
            }
        }
    }
}

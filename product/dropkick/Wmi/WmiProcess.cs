using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dropkick.Wmi
{
    class WmiProcess
    {
        const string CLASSNAME = "Win32_Process";
        //private char NULL_VALUE = char(0);

       // http://msdn.microsoft.com/en-us/library/aa389388(v=VS.85).aspx
        //http://www.dalun.com/blogs/05.09.2007.htm
        public static ProcessReturnCode Run(string machineName, string commandLine, string currentDirectory,
                                              string serviceLocation, ServiceStartMode startMode, string userName,
                                              string password, string[] dependencies)
        {
            if (userName != null && userName.IndexOf('\\') < 0)
            {
                userName = ".\\" + userName;
            }

            try
            {
                const string methodName = "Create";
                Int32 processId=0;

                var parameters = new object[]
                                     {
                                         commandLine, // CommandLine
                                         currentDirectory, // CurrentDirectory
                                         null, //Win32_ProcessStartup ProcessStartupInformation
                                         processId //out ProcessId
                                     };
                return (ProcessReturnCode)WmiHelper.InvokeStaticMethod(machineName, CLASSNAME, methodName, parameters);
            }
            catch {
                //throw;
                return ProcessReturnCode.UnknownFailure;
            }
        }

    }
}

namespace dropkick.Wmi
{
    using System;
    using System.Management;

    //Reference http://msdn.microsoft.com/en-us/library/aa389388(v=VS.85).aspx
    //Reference http://www.dalun.com/blogs/05.09.2007.htm

    public class WmiProcess
    {
        const string CLASSNAME = "Win32_Process";
        //private char NULL_VALUE = char(0);
        

        public static ProcessReturnCode Run(string machineName, string commandLine, string args, string currentDirectory)
        {
 
                const string methodName = "Create";
                Int32 processId = 0;

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
                    return (ProcessReturnCode)rtn;
                }

                return ProcessReturnCode.ERROR;

//                var parameters = new object[]
//                                     {
//                                         commandLine + " " + args, // CommandLine
//                                         currentDirectory, // CurrentDirectory
//                                         null, //Win32_ProcessStartup ProcessStartupInformation
//                                         processId //out ProcessId
//                                     };
//                return (ProcessReturnCode)WmiHelper.InvokeStaticMethod(machineName, CLASSNAME, methodName, parameters);
//            }
//            catch
//            {
                //throw;
//                return ProcessReturnCode.UnknownFailure;
//            }
            }

    }
}

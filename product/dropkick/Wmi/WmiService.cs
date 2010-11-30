namespace dropkick.Wmi
{
    using System;

    //Reference: http://msdn2.microsoft.com/en-us/library/aa389390(VS.85).aspx


    public class WmiService
    {
        const string CLASSNAME = "Win32_Service";
        //private char NULL_VALUE = char(0);

        public static ServiceReturnCode Create(string machineName, string serviceName, string serviceDisplayName,
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


                var parameters = new object[]
                                     {
                                         serviceName, // Name
                                         serviceDisplayName, // Display Name
                                         serviceLocation, // Path Name | The Location "E:\somewhere\something"
                                         Convert.ToInt32(ServiceType.OwnProcess), // ServiceType
                                         Convert.ToInt32(ErrorControl.UserNotified), // Error Control
                                         startMode.ToString(), // Start Mode
                                         false, // Desktop Interaction
                                         userName, // StartName | Username
                                         password, // StartPassword |Password
                                         null, // LoadOrderGroup | Service Order Group
                                         null, // LoadOrderGroupDependencies | Load Order Dependencies
                                         dependencies //  ServiceDependencies
                                     };
                return (ServiceReturnCode) WmiHelper.InvokeStaticMethod(machineName, CLASSNAME, methodName, parameters);
            }
            catch
            {
                return ServiceReturnCode.UnknownFailure;
            }
        }

        public static ServiceReturnCode Delete(string machineName, string serviceName)
        {
            try
            {
                string methodName = "Delete";
                return
                    (ServiceReturnCode) WmiHelper.InvokeInstanceMethod(machineName, CLASSNAME, serviceName, methodName);
            }
            catch
            {
                return ServiceReturnCode.UnknownFailure;
            }
        }

        public static ServiceReturnCode Start(string machineName, string serviceName)
        {
            try
            {
                string methodName = "StartService";
                return
                    (ServiceReturnCode) WmiHelper.InvokeInstanceMethod(machineName, CLASSNAME, serviceName, methodName);
            }
            catch
            {
                return ServiceReturnCode.UnknownFailure;
            }
        }

        public static ServiceReturnCode Stop(string machineName, string serviceName)
        {
            try
            {
                string methodName = "StopService";
                return
                    (ServiceReturnCode) WmiHelper.InvokeInstanceMethod(machineName, CLASSNAME, serviceName, methodName);
            }
            catch
            {
                return ServiceReturnCode.UnknownFailure;
            }
        }

        public static ServiceReturnCode Pause(string machineName, string serviceName)
        {
            try
            {
                string methodName = "PauseService";
                return
                    (ServiceReturnCode) WmiHelper.InvokeInstanceMethod(machineName, CLASSNAME, serviceName, methodName);
            }
            catch
            {
                return ServiceReturnCode.UnknownFailure;
            }
        }

        public static ServiceReturnCode QueryService(string machineName, string serviceName)
        {
            try
            {
                string methodName = "InterrogateService";
                return
                    (ServiceReturnCode) WmiHelper.InvokeInstanceMethod(machineName, CLASSNAME, serviceName, methodName);
            }
            catch
            {
                return ServiceReturnCode.UnknownFailure;
            }
        }

        public static ServiceReturnCode Resume(string machineName, string serviceName)
        {
            try
            {
                string methodName = "ResumeService";
                return
                    (ServiceReturnCode) WmiHelper.InvokeInstanceMethod(machineName, CLASSNAME, serviceName, methodName);
            }
            catch
            {
                return ServiceReturnCode.UnknownFailure;
            }
        }
    }
}
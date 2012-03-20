namespace dropkick.Wmi
{
    using System;
    using System.Management;

    public class WmiHelper
    {
        public static ManagementScope Connect(string machineName)
        {
            var scope = new ManagementScope(@"\\{0}\root\cimv2".FormatWith(machineName))
            {
                Options =
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    EnablePrivileges = true
                }
            };

            try
            {
                scope.Connect();
            }
            catch (Exception exc)
            {
                throw new SystemException("Problem connecting WMI scope on " + machineName + ".", exc);
            }

            return scope;
        }

        static ManagementObject GetInstanceByName(string machineName, string className, string name) {
            var query = "SELECT * FROM " + className + " WHERE Name = '" + name + "'";
            foreach (ManagementObject manObject in Query(machineName,query)) {
                return manObject;
            }

            return null;
        }

        public static ManagementObjectCollection Query(string machineName, string query)
        {
            ManagementScope scope = Connect(machineName);
            var queryObj = new ObjectQuery(query);
            var searcher = new ManagementObjectSearcher(scope, queryObj);
            ManagementObjectCollection results = searcher.Get();

            return results;
        }



        static ManagementClass GetStaticByName(string machineName, string className)
        {
            ManagementScope scope = Connect(machineName);
            var getOptions = new ObjectGetOptions();
            var path = new ManagementPath(className);
            var manClass = new ManagementClass(scope, path, getOptions);
            
            return manClass;
        }

        public static int InvokeInstanceMethod(string machineName, string className, string name, string methodName)
        {
            return InvokeInstanceMethod(machineName, className, name, methodName, null);
        }

        public static int InvokeInstanceMethod(string machineName, string className, string name, string methodName,
                                               object[] parameters)
        {
            try
            {
                ManagementObject manObject = GetInstanceByName(machineName, className, name);
                object result = manObject.InvokeMethod(methodName, parameters);
                return Convert.ToInt32(result);
            }
            catch
            {
                return -1;
            }
        }

        public static int InvokeStaticMethod(string machineName, string className, string methodName)
        {
            return InvokeStaticMethod(machineName, className, methodName, null);
        }

        public static int InvokeStaticMethod(string machineName, string className, string methodName,
                                             object[] parameters)
        {
            try
            {
                using (var managementClass = GetStaticByName(machineName, className))
                {
                    object result = managementClass.InvokeMethod(methodName, parameters);
                    return Convert.ToInt32(result);
                }
            }
            catch
            {
                return -1;
            }
        }
    }
}
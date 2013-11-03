namespace dropkick.Wmi
{
    using System;
    using System.Management;

    public class WmiHelper
    {
        public static ManagementScope Connect(string machineName, string userName=null, string password=null)
        {
            var scope = new ManagementScope(@"\\{0}\root\cimv2".FormatWith(machineName))
            {
                Options =
                {
                    Impersonation = ImpersonationLevel.Impersonate,
                    EnablePrivileges = true,
                    Username = userName,
                    Password = password
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

        static ManagementObject GetInstanceByName(string machineName, string className, string name, string userName=null, string password=null) {
            var query = "SELECT * FROM " + className + " WHERE Name = '" + name + "'";
            foreach (ManagementObject manObject in Query(machineName,query, userName, password)) {
                return manObject;
            }

            return null;
        }

        public static ManagementObjectCollection Query(string machineName, string query, string userName=null, string password=null)
        {
            ManagementScope scope = Connect(machineName, userName, password);
            var queryObj = new ObjectQuery(query);
            var searcher = new ManagementObjectSearcher(scope, queryObj);
            ManagementObjectCollection results = searcher.Get();

            return results;
        }



        static ManagementClass GetStaticByName(string machineName, string className, string userName=null, string password=null)
        {
            ManagementScope scope = Connect(machineName, userName, password);
            var getOptions = new ObjectGetOptions();
            var path = new ManagementPath(className);
            var manClass = new ManagementClass(scope, path, getOptions);
            
            return manClass;
        }

        public static int InvokeInstanceMethod(string machineName, string className, string name, string methodName)
        {
            return InvokeInstanceMethod(machineName, className, name, methodName, null);
        }

        public static int InvokeInstanceMethodWithAuthentication(string machineName, string className, string name, string methodName, string userName, string password)
        {
            return InvokeInstanceMethodWithAuthentication(machineName, className, name, methodName, userName, password, null);
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

        public static int InvokeInstanceMethodWithAuthentication(string machineName, string className, string name, string methodName,
                                               string userName, string password, object[] parameters)
        {
            try
            {
                ManagementObject manObject = GetInstanceByName(machineName, className, name, userName, password);
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

        public static int InvokeStaticMethodWithAuthentication(string machineName, string className, string methodName,
                                             object[] parameters, string wmiUserName, string wmiPassword)
        {
            try
            {
                using (var managementClass = GetStaticByName(machineName, className, wmiUserName, wmiPassword))
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
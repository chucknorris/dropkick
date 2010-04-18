namespace dropkick.Wmi
{
    using System;
    using System.Management;

    public class WmiHelper
    {
        static ManagementScope Connect(string machineName)
        {
            var options = new ConnectionOptions();

            string path = "\\\\{0}\\root\\cimv2";
            path = String.Format(path, machineName);
            var scope = new ManagementScope(path, options);
            scope.Connect();
            return scope;
        }

        static ManagementObject GetInstanceByName(string machineName, string className, string name)
        {
            ManagementScope scope = Connect(machineName);
            var query = new ObjectQuery("SELECT * FROM " + className + " WHERE Name = '" + name + "'");
            var searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection results = searcher.Get();
            foreach (ManagementObject manObject in results)
                return manObject;

            return null;
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
                ManagementClass manClass = GetStaticByName(machineName, className);
                object result = manClass.InvokeMethod(methodName, parameters);
                return Convert.ToInt32(result);
            }
            catch
            {
                return -1;
            }
        }
    }
}
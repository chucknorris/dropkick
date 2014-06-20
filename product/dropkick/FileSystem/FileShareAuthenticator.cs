using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dropkick.FileSystem
{
    public static class FileShareAuthenticator
    {
        public static FileShareAuthenticationContext BeginFileShareAuthentication(string remoteUnc, string userName, string password)
        {
            string error = PinvokeWindowsNetworking.connectToRemote(remoteUnc, userName, password);
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception("Error calling PinvokeWindowsNetworking.connectToRemote: " + error);
            }
            return new FileShareAuthenticationContext(remoteUnc, userName, password);
        }

        public class FileShareAuthenticationContext : IDisposable
        {
            private readonly string _remoteUnc;
            private readonly string _userName;
            private readonly string _password;
            private bool _active;

            public FileShareAuthenticationContext(string remoteUnc, string userName, string password)
            {
                _remoteUnc = remoteUnc;
                _userName = userName;
                _password = password;
                _active = true;
            }

            public void Dispose()
            {
                if (_active)
                {
                    var error = PinvokeWindowsNetworking.disconnectRemote(_remoteUnc);
                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception("PinvokeWindowsNetworking.disconnectRemote failed: " + error);
                    }
                    _active = false;
                }
            }
        }
    }
}

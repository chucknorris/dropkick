using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dropkick.Wmi;
using dropkick.StringInterpolation;

namespace dropkick.Configuration.Dsl.Authentication
{
    public static class Extension
    {
        public static ProtoServer WithAuthentication(this ProtoServer server, string remoteUserName, string remotePassword)
        {
            var interpolator = new CaseInsensitiveInterpolator();
            remoteUserName = interpolator.ReplaceTokens(HUB.Settings, remoteUserName);
            remotePassword = interpolator.ReplaceTokens(HUB.Settings, remotePassword);
            WmiService.WithAuthentication(remoteUserName, remotePassword);
            return server;
        }
    }
}

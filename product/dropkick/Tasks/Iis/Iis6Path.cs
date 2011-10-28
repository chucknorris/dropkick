namespace dropkick.Tasks.Iis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.DirectoryServices;
    using System.Text;
    using DeploymentModel;

    [DebuggerDisplay("{ServerName}->{WebsiteName}->{VirtualDirectoryName}")]
    public class Iis6Path
    {
        public string ServerName { get; set; }
        public string WebsiteName { get; set; }
        public string VirtualDirectoryName { get; set; }

        public Iis6Path(string serverName, string webSiteName, string vDirName)
        {
            ServerName = serverName;
            WebsiteName = webSiteName;
            VirtualDirectoryName = vDirName;
        }

        public Iis6Path(string serverName, string webSiteName)
        {
            WebsiteName = webSiteName;
            ServerName = serverName;
        }

        public DirectoryEntry ToDirectoryEntry()
        {
            if (VirtualDirectoryName == null) return new DirectoryEntry(BuildIisSitePath(WebsiteName));

            return new DirectoryEntry(BuildIisSiteAndVDirPath(WebsiteName, VirtualDirectoryName));
        }

        string BuildIisSiteAndVDirPath(int siteNumber, string vDirPath)
        {
            return "IIS://{0}/w3svc/{1}/Root/{2}".FormatWith(ServerName, siteNumber, vDirPath);
        }
        string BuildIisSiteAndVDirPath(string siteName, string vDirPath)
        {
            return BuildIisSiteAndVDirPath(ConvertSiteNameToSiteNumber(siteName), vDirPath);
        }
        string BuildIisSitePath(int siteNumber)
        {
            return "IIS://{0}/w3svc/{1}/Root".FormatWith(ServerName, siteNumber);
        }
        string BuildIisSitePath(string siteName)
        {
            return BuildIisSitePath(ConvertSiteNameToSiteNumber(siteName));
        }

        int ConvertSiteNameToSiteNumber(string name)
        {
            var foundWebsites = new List<string>();

            using (var e = new DirectoryEntry("IIS://{0}/W3SVC".FormatWith(ServerName)))
            {
                e.RefreshCache();
                foreach (DirectoryEntry entry in e.Children)
                {
                    if (entry.SchemaClassName != "IIsWebServer")
                    {
                        entry.Close();
                        continue;
                    }

                    foundWebsites.Add(entry.Properties["ServerComment"].Value.ToString());
                    if (!entry.Properties["ServerComment"].Value.Equals(name))
                        continue;


                    var siteNumber = entry.Name;
                    entry.Close();
                    return int.Parse(siteNumber);
                }

            }

            //didn't find anything, gonna hurl now
            var sb = new StringBuilder();
            sb.AppendFormat("Couldn't find the website '{0}'", name);
            sb.AppendLine();
            sb.AppendLine("Found the following web sites:");
            foreach (var site in foundWebsites) sb.AppendFormat("  '{0}'{1}", site, Environment.NewLine);

            throw new Exception(sb.ToString());
        }

        public bool DoesSiteExist(DeploymentResult result)
        {
            var doesExist = ConvertSiteNameToSiteNumber(WebsiteName) > 0;

            return doesExist;
        }
    }
}
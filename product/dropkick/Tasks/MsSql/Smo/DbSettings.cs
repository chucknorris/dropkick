using Microsoft.SqlServer.Management.Smo;

namespace dropkick.Tasks.MsSql.Smo
{
    public class DbSettings
    {
        public bool AutoCreateStatisticsEnabled { get; set; }
        public string Collation { get; set; }
        public RecoveryModel RecoveryModel { get; set; }
        public CompatibilityLevel CompatibilityLevel { get; set; }
        public bool AutoClose { get; set; }
        public bool AutoShrink { get; set; }
        public bool AutoUpdateStatisticsEnabled { get; set; }
        public bool AutoUpdateStatisticsAsync { get; set; }
    }
}
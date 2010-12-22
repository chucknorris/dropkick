namespace dropkick.Configuration.Dsl.MsSrss
{
    using System;

    public static class Extensions
    {
        public static void SqlReports(this ProtoServer server, Action<ReportOptions> action)
        {
            
        }
    }

    public interface ReportOptions
    {
        void PublishTo(string address);
        void PublishAllIn(string folder);
        void Publish(string name);
    }
}
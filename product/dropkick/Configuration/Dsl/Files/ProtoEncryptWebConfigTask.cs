namespace dropkick.Configuration.Dsl.Files
{
    using System;
    using CommandLine;
    using DeploymentModel;
    using FileSystem;
    using Tasks;

    public class ProtoEncryptWebConfigTask :
        BaseTask
    {
        readonly Path _path;
        readonly string _where;

        public ProtoEncryptWebConfigTask(Path path, string whereIsTheConfig)
        {
            _where = whereIsTheConfig;
            _path = path;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            var task = new ProtoCommandLineTask(@"aspnet_regiis");
            task.Args(@" -pe ""connectionStrings"" -app ""/MachineDPAPI"" -prov ""DataProtectionConfigurationProvider""");
            string winDir = Environment.GetEnvironmentVariable("WINDIR");
            task.ExecutableIsLocatedAt(_path.Combine(winDir, @"Microsoft.NET\Framework\v2.0.50727"));
            task.WorkingDirectory(_where);

            task.RegisterRealTasks(site);
        }
    }

    public class ProtoEncryptAppConfigTask :
    BaseTask
    {
        readonly Path _path;
        readonly string _where;

        public ProtoEncryptAppConfigTask(Path path, string whereIsTheConfig)
        {
            _where = whereIsTheConfig;
            _path = path;
        }

        public override void RegisterRealTasks(PhysicalServer site)
        {
            throw new NotImplementedException("fix this");
        }
    }
}
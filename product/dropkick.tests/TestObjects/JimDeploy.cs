namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration;
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.WinService;
    using dropkick.Configuration.Dsl.Files;

    public class JimDeploy : 
        Deployment<JimDeploy, JimSettings>
    {
        public JimDeploy()
        {
            Define(settings =>
            {
                int i = 0; //shut up R#

                DeploymentStepsFor(Service, targetServer=>
                {
                    targetServer.WinService(settings.ServiceName).Stop();

                    targetServer.CopyDirectory(@".\DllsToCopyOut")
                        .To(settings.TargetDir);

                    targetServer.CopyFile(@".\ConfigFiles\{{Environment}}.app.config")
                        .ToDirectory(settings.TargetDir);

                    targetServer.RenameFile(@"{{Environment}}.app.config").To("app.config");

//                    targetServer.Security(sec=>
//                    {
//                        sec.LocalPolicy(lp=>
//                        {
//                            lp.LogOnAsBatch("Account");
//                            lp.LogOnAsService("Account");
//                        });
//                    } );

                    targetServer.WinService(settings.ServiceName).Start();
                });
            });
        }

        public static Role Service { get; set; }
    }

    public class JimSettings :
        DropkickConfiguration
    {
        public string ServiceName { get; set; }
        public string TargetDir { get; set; }
    }
}
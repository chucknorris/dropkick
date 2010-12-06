namespace dropkick.tests.TestObjects
{
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.CommandLine;
    using dropkick.Configuration.Dsl.Dsn;
    using dropkick.Configuration.Dsl.Files;
    using dropkick.Configuration.Dsl.Msmq;
    using dropkick.Configuration.Dsl.MsSql;
    using dropkick.Configuration.Dsl.NetworkShare;
    using dropkick.Configuration.Dsl.WinService;

    public class TestDeployment :
        Deployment<TestDeployment, SampleConfiguration>
    {
        public TestDeployment()
        {
            Define((settings) =>
            {
                DeploymentStepsFor(Web, server =>
                {
                    server.ShareFolder("bob").PointingTo(@"E:\Tools");

                    server.CreateDSN("NAME").ForDatabase("Enterprise");

                    server.CommandLine("ping")
                        .Args("www.google.com")
                        .ExecutableIsLocatedAt("");

                    server.Msmq()
                        .PrivateQueue("bob");


                    server.CopyDirectory(o =>
                    {
                        o.Include(@"\\someserver\bob\bill");
                    }).To(@"E:\FHLBApplications\atlas");


                    server.WinService("MSMQ").Do(s =>
                    {
                        //service stops

                        //do stuff

                        //service starts
                    });


                });

                DeploymentStepsFor(Db, server =>
                {
                    server.SqlInstance(".")
                        .Database("Enterprise");

                });

                DeploymentStepsFor(Service, server =>
                {

                    server.WinService("FlamesHost")
                        .Do(s => //auto-stop
                        {
                            server.CopyDirectory(o =>
                            {
                                o.Include(@"\\srvtopeka00\whatever");
                            }).To(@".\code_drop\flameshost");

                            
                            //TODO file actions
//                            server.File.AppConfig
//                                        .ReplaceIdentityTokensWithPrompt()
//                                        .EncryptIdentity();
                                
                        }); //auto-start   
                });
            });
        }

        public static Role Web { get; set; }
        public static Role Db { get; set; }
        public static Role Service { get; set; }
    }

    public class TestSettings
    {
        public string Name { get; set; }
    }
}
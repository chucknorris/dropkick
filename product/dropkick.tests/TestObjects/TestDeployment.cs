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
        Deployment<TestDeployment, object>
    {
        public TestDeployment()
        {
            Define(() =>
            {
                DeploymentStepsFor(Web, server =>
                {
                    server.ShareFolder("bob").PointingTo(@"E:\Tools")
                        .CreateIfNotExist();

                    server.CreateDSN("NAME").ForDatabase("Enterprise");

                    server.CommandLine("ping")
                        .Args("www.google.com")
                        .ExecutableIsLocatedAt("");

                    server.Msmq()
                        .PrivateQueueNamed("bob")
                        .CreateIfItDoesntExist();

                    server.CopyTo(@"E:\FHLBApplications\atlas")
                        .From(@"\\someserver\bob\bill");

                    server.CopyTo(@"\\srvtopeka19\exchecquer\flames\")
                        .From(@".\code_drop\flamesweb\")
                        .With(f => f.WebConfig
                                       .ReplaceIdentityTokensWithPrompt()
                                       .EncryptIdentity());


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
                            server.CopyTo(@".\code_drop\flameshost").From(@"\\srvtopeka00\whatever")
                                .With(f =>
                                {
                                    f.AppConfig
                                        .ReplaceIdentityTokensWithPrompt()
                                        .EncryptIdentity();
                                });
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
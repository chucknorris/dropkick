namespace dropkick.tests.TestObjects
{
    using System;
    using dropkick.Configuration.Dsl;
    using dropkick.Configuration.Dsl.Msmq;

    public class MsmqTestDeploy :
        Deployment<MsmqTestDeploy, object>
    {
        public MsmqTestDeploy()
        {
            Define(() =>
                   DeploymentStepsFor(Web, server => server.Msmq()
                                                      .PrivateQueueNamed("dk_test"))
                );
        }

        public static Role Web { get; set; }
    }
}
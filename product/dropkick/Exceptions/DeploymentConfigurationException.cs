namespace dropkick.Configuration.Dsl
{
    using System;

    public class DeploymentConfigurationException :
        Exception
    {
        public DeploymentConfigurationException()
        {
        }

        public DeploymentConfigurationException(string message) : base(message)
        {
        }
    }
}
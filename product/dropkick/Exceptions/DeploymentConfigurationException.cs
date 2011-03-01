namespace dropkick.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class DeploymentConfigurationException :
        Exception
    {
        public DeploymentConfigurationException()
        {
        }

        public DeploymentConfigurationException(string message) : base(message)
        {
        }

        public DeploymentConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeploymentConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
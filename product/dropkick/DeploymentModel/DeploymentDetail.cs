namespace dropkick.DeploymentModel
{
    using System;

    public class DeploymentDetail
    {
        readonly Func<string> _name;
        readonly Func<DeploymentResult> _verify;
        readonly Func<DeploymentResult> _execute;

        public DeploymentDetail(Func<string> name, Func<DeploymentResult> verify, Func<DeploymentResult> execute)
        {
            _name = name;
            _verify = verify;
            _execute = execute;
        }

        public string Name
        {
            get { return _name(); }
        }

        public DeploymentResult Verify()
        {
            return _verify();
        }

        public DeploymentResult Execute()
        {
            return _execute();
        }
    }
}
namespace dropkick.DeploymentModel
{
    public interface Task
    {

        string Name { get; }

        //Change to Func<DeploymentResult>
        DeploymentResult VerifyCanRun();

        //Change to Func<DeploymentResult>
        DeploymentResult Execute();
    }
}
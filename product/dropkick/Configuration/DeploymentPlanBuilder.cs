namespace dropkick.Configuration
{
    using DeploymentModel;
    using Dsl;
    using Engine;

    public interface DeploymentPlanBuilder
    {
        //deployment is a part of the DSL - this interface shouldn't know about it.
        DeploymentPlan GetPlan(Deployment deployment, DeploymentArguments args);
    }
}
namespace dropkick.tests.TestObjects
{
    using System;
    using dropkick.Configuration.Dsl;
    using dropkick.DeploymentModel;


    public class TestTask :
        Task
    {
        public bool WasRun { get; set; }

        public TestTask()
        {
            WasRun = false;
        }

        public DeploymentResult Execute()
        {
            WasRun = true;
            return new DeploymentResult();
        }

        public void InspectWith(DeploymentInspector inspector)
        {
            Console.WriteLine("task inspection");
            inspector.Inspect(this);
        }

        public string Name
        {
            get { return "test"; }
        }

        public DeploymentResult VerifyCanRun()
        {
            return new DeploymentResult();
        }
    }
}
namespace dropkick.Tasks.UserInteraction
{
    using System;
    using DeploymentModel;

    public class PauseTask : Task
    {
        private readonly string _messageToDisplay;

        public PauseTask(string messageToDisplay)
        {
            _messageToDisplay = messageToDisplay;
        }

        public string Name
        {
            get { return "Paused: {0}".FormatWith(_messageToDisplay); }
        }

        public DeploymentResult VerifyCanRun()
        {
            return new DeploymentResult();
        }

        public DeploymentResult Execute()
        {
            Console.WriteLine();
            Console.WriteLine(Name);
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();

            var result = new DeploymentResult();
            result.AddGood("User pressed enter to continue deployment.");
            return result;
        }
    }
}
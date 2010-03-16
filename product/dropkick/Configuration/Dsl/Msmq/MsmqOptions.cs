namespace dropkick.Configuration.Dsl.Msmq
{
    public interface QueueOptions
    {
        void CreateIfItDoesntExist();
    }

    public interface MsmqOptions
    {
        QueueOptions PrivateQueueNamed(string name);
    }
}
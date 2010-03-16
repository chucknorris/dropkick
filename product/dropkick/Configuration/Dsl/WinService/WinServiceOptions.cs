namespace dropkick.Configuration.Dsl.WinService
{
    using System;

    public interface WinServiceOptions
    {
        void Start();
        void Stop();
        WinServiceOptions Do(Action<Server> registerAdditionalActions);
    }
}
namespace dropkick.Wmi
{
    public enum ErrorControl
    {
        UserNotNotified = 0,
        UserNotified = 1,
        SystemRestartedWithLastKnownGoodConfiguration = 2,
        SystemAttemptsToStartWithAGoodConfiguration = 3
    }
}
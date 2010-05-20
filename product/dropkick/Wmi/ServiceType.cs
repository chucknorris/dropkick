namespace dropkick.Wmi
{
    public enum ServiceType
    {
        KernalDriver = 1,
        FileSystemDriver = 2,
        Adapter = 4,
        RecognizerDriver = 8,
        OwnProcess = 16,
        ShareProcess = 32,
        InteractiveProcess = 256,
    }
}
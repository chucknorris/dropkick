namespace dropkick.Wmi
{
    public enum ServiceReturnCode
    {
        Success = 0,
        NotSupported = 1,
        AccessDenied = 2,
        DependentServicesRunning = 3,
        InvalidServiceControl = 4,
        ServiceCannotAcceptControl = 5,
        ServiceNotActive = 6,
        ServiceRequestTimeout = 7,
        UnknownFailure = 8,
        PathNotFound = 9,
        ServiceAlreadyRunning = 10,
        ServiceDatabaseLocked = 11,
        ServiceDependencyDeleted = 12,
        ServiceDependencyFailure = 13,
        ServiceDisabled = 14,
        ServiceLogonFailure = 15,
        ServiceMarkedForDeletion = 16,
        ServiceNoThread = 17,
        StatusCircularDependency = 18,
        StatusDuplicateName = 19,
        StatusInvalidName = 20,
        StatusInvalidParameter = 21,
        StatusInvalidServiceAccount = 22,
        StatusServiceExists = 23,
        ServiceAlreadyPaused = 24,
    }
}
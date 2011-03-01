namespace dropkick.Wmi
{
    public enum ProcessReturnCode
    {
        ERROR = -1,
        Success = 0,
        AccessDenied = 2,
        InsufficentPrivileges = 3,
        UnknownFailure = 8,
        PathNotFound = 9,
        InvalidParameter = 21,
    }
}
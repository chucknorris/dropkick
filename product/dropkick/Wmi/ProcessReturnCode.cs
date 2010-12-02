namespace dropkick.Wmi
{
    public enum ProcessReturnCode
    {
        Success = 0,
        AccessDenied = 2,
        InsufficentPrivileges = 3,
        UnknownFailure = 8,
        PathNotFound = 9,
        InvalidParameter = 21,
    }
}
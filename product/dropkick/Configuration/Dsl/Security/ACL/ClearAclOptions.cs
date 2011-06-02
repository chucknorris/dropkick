namespace dropkick.Configuration.Dsl.Security.ACL
{
    public interface ClearAclOptions
    {
        ClearAclOptions Preserve(string groupName);
        //ClearAclOptions PreserveCurrentUser();
        ClearAclOptions RemoveSystemAccount();
        ClearAclOptions RemoveAdministratorsGroup();
        ClearAclOptions RemoveUsersGroup();
    }
}
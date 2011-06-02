namespace dropkick.Configuration.Dsl.Security.ACL
{
    public interface ClearAclOptions
    {
        ClearAclOptions Preserve(params string[] groupAndOrAccountNames);
        ClearAclOptions RemoveSystemAccount();
        ClearAclOptions RemoveAdministratorsGroup();
        ClearAclOptions RemoveUsersGroup();
    }
}
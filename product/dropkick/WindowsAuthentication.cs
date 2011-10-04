namespace dropkick
{
	public static class WindowsAuthentication
	{
		public static bool IsBuiltInUsername(string username)
		{
			const string ntAuthorityPrefix = @"NT AUTHORITY\";

			return string.Compare(ntAuthorityPrefix, 0, username, 0, ntAuthorityPrefix.Length, true) == 0;			
		}
	}
}

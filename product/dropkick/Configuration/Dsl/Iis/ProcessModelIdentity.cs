using System;
using Microsoft.Web.Administration;

namespace dropkick.Configuration.Dsl.Iis
{
	public enum ProcessModelIdentity
	{
		ApplicationPoolIdentity,
		LocalService,
		LocalSystem,
		NetworkService,		
		SpecificUser
	}

	/// <summary>
	/// Translate Dsl.Iis.ProcessModelIdentity to Microsoft.Web.Administration.ProcessModelIdentityType
	/// </summary>
	public static class ProcessModelIdentityTypeExtension
	{
		public static ProcessModelIdentityType ToProcessModelIdentityType(this ProcessModelIdentity identity)
		{
			switch (identity)
			{
				case ProcessModelIdentity.LocalService:
					return ProcessModelIdentityType.LocalService;
				case ProcessModelIdentity.LocalSystem:
					return ProcessModelIdentityType.LocalSystem;
				case ProcessModelIdentity.NetworkService:
					return ProcessModelIdentityType.NetworkService;
				case ProcessModelIdentity.SpecificUser:
					return ProcessModelIdentityType.SpecificUser;
				case ProcessModelIdentity.ApplicationPoolIdentity:
					// New for IIS 7.5
					return (ProcessModelIdentityType)4;
				default:
					throw new NotImplementedException(String.Format("ProcessModelIdentity [{0}] has not been implemented.", identity));
			}

		}
	}
}

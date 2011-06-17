using dropkick.Configuration.Dsl.Iis;
using Microsoft.Web.Administration;
using NUnit.Framework;

namespace dropkick.tests.Configuration.Dsl
{
	[TestFixture]
	public class ProcessModelIdentityTranslation
	{
		[Test]
		public void NetworkService_translates_to_WebAdministration_NetworkService()
		{
			Assert.AreEqual(ProcessModelIdentityType.NetworkService, ProcessModelIdentity.NetworkService.ToProcessModelIdentityType());
		}

		[Test]
		public void LocalService_translates_to_WebAdministration_LocalService()
		{
			Assert.AreEqual(ProcessModelIdentityType.LocalService, ProcessModelIdentity.LocalService.ToProcessModelIdentityType());
		}

		[Test]
		public void LocalSystem_translates_to_WebAdministration_LocalSystem()
		{
			Assert.AreEqual(ProcessModelIdentityType.LocalSystem, ProcessModelIdentity.LocalSystem.ToProcessModelIdentityType());
		}

		[Test]
		public void SpecificUser_translates_to_WebAdministration_SpecificUser()
		{
			Assert.AreEqual(ProcessModelIdentityType.SpecificUser, ProcessModelIdentity.SpecificUser.ToProcessModelIdentityType());
		}

		[Test]
		public void ApplicationPoolIdentity_translates_to_IIS75_ApplicationPoolIdentity_value()
		{
			Assert.AreEqual((ProcessModelIdentityType)4, ProcessModelIdentity.ApplicationPoolIdentity.ToProcessModelIdentityType());
		}
	}
}

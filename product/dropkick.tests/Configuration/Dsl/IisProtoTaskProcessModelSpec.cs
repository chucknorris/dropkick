using System;
using dropkick.Configuration.Dsl.Iis;
using dropkick.FileSystem;
using NUnit.Framework;

namespace dropkick.tests.Configuration.Dsl
{
	public class IisProtoTaskProcessModelSpec
	{
		[ConcernFor("IisProtoTask")]
		public class when_setting_process_model_identity_to_a_builtin_user : TinySpec
		{
			[Fact]
			public void should_flag_that_process_model_identity_has_been_specified()
			{
				Assert.IsTrue(sut.ProcessModelIdentityTypeSpecified);
			}

			[Fact]
			public void should_set_the_specified_builtin_user()
			{
				Assert.AreEqual(expectedIdentity, sut.ProcessModelIdentityType);
			}

			public override void Because()
			{
				sut.SetProcessModelIdentity(expectedIdentity);
			}

			public override void Context()
			{
				expectedIdentity = ProcessModelIdentity.LocalSystem;
				sut = new IisProtoTask("test", new DotNetPath());
			}

			IisProtoTask sut;
			ProcessModelIdentity expectedIdentity;
		}

		[ConcernFor("IisProtoTask")]
		public class when_setting_process_model_identity_to_a_specific_user : TinySpec
		{
			[Fact]
			public void should_flag_that_process_model_identity_has_been_specified()
			{
				Assert.IsTrue(sut.ProcessModelIdentityTypeSpecified);
			}

			[Fact]
			public void should_set_the_username()
			{
				Assert.AreEqual(expectedUsername, sut.ProcessModelUsername);
			}

			[Fact]
			public void should_set_the_password()
			{
				Assert.AreEqual(expectedPassword, sut.ProcessModelPassword);
			}

			[Fact]
			public void should_specify_a_specific_user_identity_type()
			{
				Assert.AreEqual(ProcessModelIdentity.SpecificUser, sut.ProcessModelIdentityType);
			}

			public override void Because()
			{
				sut.SetProcessModelIdentity(expectedUsername, expectedPassword);
			}

			public override void Context()
			{
				sut = new IisProtoTask("test", new DotNetPath());
			}

			IisProtoTask sut;
			string expectedUsername = "test username";
			string expectedPassword = "expected password";
		}
	}
}

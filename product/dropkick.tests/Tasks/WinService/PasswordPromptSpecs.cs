using System;
using System.Linq;
using NUnit.Framework;
using dropkick.DeploymentModel;
using dropkick.Tasks.WinService;

namespace dropkick.tests.Tasks.WinService
{
	[Category("WinService")]
	public class When_installing_a_service_with_builtin_credentials : TinySpec
	{
		private WinServiceCreateTask _task;
		private DeploymentResult _result;

		[Observation]
		public void It_should_not_prompt_for_password()
		{
			_result.Results.Where(x => x.Message.Contains("prompt for a password")).Count().ShouldBeEqualTo(0);
		}
		
		public override void Context()
		{
			_task = new WinServiceCreateTask("localhost", "test");
		}

		public override void Because()
		{
			_task.UserName = @"NT AUTHORITY\NetworkService";
			_task.Password = String.Empty;
			_result = _task.VerifyCanRun();
		}
	}

	[Category("WinService")]
	public class When_installing_a_service_with_credentials_without_specifying_password : TinySpec
	{
		private WinServiceCreateTask _task;
		private DeploymentResult _result;

		[Observation]
		public void It_should_prompt_for_password()
		{
			_result.Results.Where(x => x.Message.Contains("prompt for a password")).Count().ShouldBeEqualTo(1);
		}

		public override void Context()
		{
			_task = new WinServiceCreateTask("localhost", "test");
		}

		public override void Because()
		{
			_task.UserName = "noone";
			_task.Password = String.Empty;
			_result = _task.VerifyCanRun();
		}
	}

	[Category("WinService")]
	public class When_installing_a_service_with_credentials_and_password : TinySpec
	{
		private WinServiceCreateTask _task;
		private DeploymentResult _result;

		[Observation]
		public void It_should_not_prompt_for_password()
		{
			_result.Results.Where(x => x.Message.Contains("prompt for a password")).Count().ShouldBeEqualTo(0);
		}

		public override void Context()
		{
			_task = new WinServiceCreateTask("localhost", "test");
		}

		public override void Because()
		{
			_task.UserName = "noone";
			_task.Password = "password";
			_result = _task.VerifyCanRun();
		}
	}
}

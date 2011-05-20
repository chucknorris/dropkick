using System;
using dropkick.Engine;
using NUnit.Framework;

namespace dropkick.tests.Engine
{
    public class DeploymentCommandLineParserSpecs
    {
        public abstract class DeploymentCommandLineParserSpecsBase : TinySpec
        {
            protected DeploymentArguments arguments;

            public override void Context()
            {
            }
        }

        [ConcernFor("DeploymentCommandLineParser")]
        public class when_the_DeploymentCommandLineParser_is_given_trace_as_the_arguments : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "trace";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Command_set_to_Trace()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Trace);
            }
        }

        [ConcernFor("DeploymentCommandLineParser")]
        public class when_the_DeploymentCommandLineParser_is_given_verify_as_the_arguments : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "verify";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Command_set_to_Verify()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Verify);
            }
        }

        [ConcernFor("DeploymentCommandLineParser")]
        public class when_the_DeploymentCommandLineParser_is_given_execute_as_the_arguments : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "execute";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Command_set_to_Execute()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Execute);
            }
        }

        [ConcernFor("DeploymentCommandLineParser")]
        public class when_the_DeploymentCommandLineParser_is_given_no_arguments : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Role_set_to_ALL()
            {
                arguments.Role.ShouldBeEqualTo("ALL");
            }

            [Fact]
            public void should_have_Environment_set_to_LOCAL()
            {
                arguments.Environment.ShouldBeEqualTo("LOCAL");
            }

            [Fact]
            public void should_have_Command_set_to_Trace()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Trace);
            }

            [Fact]
            public void should_have_Silent_set_to_false()
            {
                arguments.Silent.ShouldBeEqualTo(false);
            }

            [Fact]
            public void should_have_Deployment_set_to_SEARCH()
            {
                arguments.Deployment.ShouldBeEqualTo("SEARCH");
            }

            [Fact]
            public void should_have_SettingsDirectory_set_to_currentworkingdirectory_slash_settings()
            {
                arguments.SettingsDirectory.ShouldBeEqualTo(".\\settings");
            }

            [Fact]
            public void should_have_PathToServerMapsFile_set_to_currentworkingdirectory_slash_settings_slash_LOCAL_dot_servermaps()
            {
                arguments.PathToServerMapsFile.ShouldBeEqualTo(".\\settings\\LOCAL.servermaps");
            }

            [Fact]
            public void should_have_PathToSettingsFile_set_to_currentworkingdirectory_slash_settings_slash_LOCAL_dot_js()
            {
                arguments.PathToSettingsFile.ShouldBeEqualTo(".\\settings\\LOCAL.js");
            }
        }

        [ConcernFor("DeploymentCommandLineParser")]
        public class when_the_DeploymentCommandLineParser_is_given_arguments_that_specify_the_environment : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "execute /environment:DEV";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Role_set_to_ALL()
            {
                arguments.Role.ShouldBeEqualTo("ALL");
            }

            [Fact]
            public void should_have_Environment_set_to_the_specified_environment()
            {
                arguments.Environment.ShouldBeEqualTo("DEV");
            }

            [Fact]
            public void should_have_Command_set_to_Execute()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Execute);
            }

            [Fact]
            public void should_have_PathToServerMapsFile_set_to_currentworkingdirectory_slash_settings_slash_the_specified_environment_dot_servermaps()
            {
                arguments.PathToServerMapsFile.ShouldBeEqualTo(".\\settings\\DEV.servermaps");
            }

            [Fact]
            public void should_have_PathToSettingsFile_set_to_currentworkingdirectory_slash_settings_slash_the_specified_environment_dot_js()
            {
                arguments.PathToSettingsFile.ShouldBeEqualTo(".\\settings\\DEV.js");
            }

        }

        [ConcernFor("DeploymentCommandLineParser")]
        public class when_the_DeploymentCommandLineParser_is_given_arguments_that_specify_the_settings_folder : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "execute /settings:..\\overhere";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Command_set_to_Execute()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Execute);
            }

            [Fact]
            public void should_have_SettingsDirectory_set_to_the_specified_directory()
            {
                arguments.SettingsDirectory.ShouldBeEqualTo("..\\overhere");
            }

            [Fact]
            public void should_have_PathToServerMapsFile_set_to_the_specified_settings_directory_slash_LOCAL_dot_servermaps()
            {
                arguments.PathToServerMapsFile.ShouldBeEqualTo("..\\overhere\\LOCAL.servermaps");
            }

            [Fact]
            public void should_have_PathToSettingsFile_set_to_the_specified_settings_directory_slash_LOCAL_dot_js()
            {
                arguments.PathToSettingsFile.ShouldBeEqualTo("..\\overhere\\LOCAL.js");
            }
        }

        [ConcernFor("DeploymentCommandLineParser")]
        public class when_the_DeploymentCommandLineParser_is_given_arguments_that_specify_the_deployment_location : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "execute /deployment:..\\deployments\\somedeploy.dll";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Command_set_to_Execute()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Execute);
            }

            [Fact]
            public void should_have_Deployment_set_to_the_specified_deployment()
            {
                arguments.Deployment.ShouldBeEqualTo("..\\deployments\\somedeploy.dll");
            }
        }
   
        [ConcernFor("DeploymentCommandLineParser")]
        public class when_the_DeploymentCommandLineParser_is_given_arguments_that_specify_the_roles : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "execute /roles:Web,Db,Host";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Command_set_to_Execute()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Execute);
            }

            [Fact]
            public void should_have_Role_set_to_the_specified_role_or_roles()
            {
                arguments.Role.ShouldBeEqualTo("Web,Db,Host");
            }
        }

        [ConcernFor("DeploymentCommandLineParser")]
        public class when_the_DeploymentCommandLineParser_is_given_arguments_that_specify_silent : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "execute /silent";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Command_set_to_Execute()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Execute);
            }

            [Fact, Category("NotWorking")]
            public void should_have_Silent_set_to_true()
            {
                arguments.Silent.ShouldBeEqualTo(true);
            }
        }

        [ConcernFor("DeploymentCommandLineParser")]
        [Category("NotWorking")]
        public class when_the_DeploymentCommandLineParser_is_given_a_full_set_of_arguments : DeploymentCommandLineParserSpecsBase
        {
            public string commandline = "execute /environment:DEV /deployment:..\\deployments\\somedeploy.dll /settings:..\\overhere /roles:Web,Db,Host /silent";

            public override void Because()
            {
                arguments = DeploymentCommandLineParser.Parse(commandline);
            }

            [Fact]
            public void should_have_Command_set_to_Execute()
            {
                arguments.Command.ShouldBeEqualTo(DeploymentCommands.Execute);
            }

            [Fact]
            public void should_have_Environment_set_to_the_specified_environment()
            {
                arguments.Environment.ShouldBeEqualTo("DEV");
            }

            [Fact]
            public void should_have_Deployment_set_to_the_specified_deployment()
            {
                arguments.Deployment.ShouldBeEqualTo("..\\deployments\\somedeploy.dll");
            }

            [Fact]
            public void should_have_SettingsDirectory_set_to_the_specified_directory()
            {
                arguments.SettingsDirectory.ShouldBeEqualTo("..\\overhere");
            }

            [Fact]
            public void should_have_PathToServerMapsFile_set_to_the_specified_settings_directory_slash_the_specified_environment_dot_servermaps()
            {
                arguments.PathToServerMapsFile.ShouldBeEqualTo("..\\overhere\\DEV.servermaps");
            }

            [Fact]
            public void should_have_PathToSettingsFile_set_to_the_specified_settings_directory_slash_the_specified_environment_dot_js()
            {
                arguments.PathToSettingsFile.ShouldBeEqualTo("..\\overhere\\DEV.js");
            }

            [Fact]
            public void should_have_Role_set_to_the_specified_role_or_roles()
            {
                arguments.Role.ShouldBeEqualTo("Web,Db,Host");
            }

            [Fact]
            public void should_have_Silent_set_to_true()
            {
                arguments.Silent.ShouldBeEqualTo(true);
            }
        }

    }
}
using System;

namespace dropkick.tests.DeploymentModel
{
    using dropkick.DeploymentModel;
    using NUnit.Framework;

    public class DeploymentPlanSpecs
    {
        public abstract class DeploymentPlanSpecsBase : TinySpec
        {
            public DeploymentPlan Plan { get; set; }
            public DeploymentDetail Detail { get; set; }
            public DeploymentResult Result { get; set; }

            public override void Context()
            {

            }
        }

        public abstract class with_a_good_model : DeploymentPlanSpecsBase
        {
            public override void Context()
            {
                base.Context();

                Detail = new DeploymentDetail(() => "test detail", () => new DeploymentResult()
                                                                    {
                                                                        new DeploymentItem(DeploymentItemStatus.Good, "verify")
                                                                    },
                                                                   () => new DeploymentResult()
                                                                    {
                                                                              new DeploymentItem(DeploymentItemStatus.Good, "execute")
                                                                    }
                                                                   , () => new DeploymentResult());

                var webRole = new DeploymentRole("Web");
                webRole.AddServer(new DeploymentServer("SrvWeb1"));
                webRole.AddServer(new DeploymentServer("SrvWeb2"));
                webRole.ForEachServerMapped(s => s.AddDetail(Detail));
                var dbRole = new DeploymentRole("Db");
                dbRole.AddServer("SrvDb");
                dbRole.ForEachServerMapped(s => s.AddDetail(Detail));
                Plan = new DeploymentPlan();
                Plan.AddRole(webRole);
                Plan.AddRole(dbRole);
            }

        }

        public abstract class with_a_failing_model : DeploymentPlanSpecsBase
        {
            public override void Context()
            {
                base.Context();

                var successDetail = new DeploymentDetail(() => "success detail", () => new DeploymentResult()
                                                                                       {
                                                                                           new DeploymentItem(DeploymentItemStatus.Good, "verify")
                                                                                       }, () => new DeploymentResult()
                                                                                                    {
                                                                                                        new DeploymentItem(DeploymentItemStatus.Good, "execute")
                                                                                                    }, () => new DeploymentResult());

                var failDetail = new DeploymentDetail(() => "fail detail", () => new DeploymentResult()
                                                                                 {
                                                                                     new DeploymentItem(DeploymentItemStatus.Error, "fail verify")
                                                                                 }, () => new DeploymentResult()
                                                                                              {
                                                                                                  new DeploymentItem(DeploymentItemStatus.Good, "execute")
                                                                                              }, () => new DeploymentResult());


                var webRole = new DeploymentRole("Web");
                webRole.AddServer(new DeploymentServer("SrvWeb1"));
                webRole.AddServer(new DeploymentServer("SrvWeb2"));
                webRole.ForEachServerMapped(s => s.AddDetail(successDetail));
                var dbRole = new DeploymentRole("Db");
                dbRole.AddServer("SrvDb");
                dbRole.ForEachServerMapped(s => s.AddDetail(failDetail));
                Plan = new DeploymentPlan();
                Plan.AddRole(webRole);
                Plan.AddRole(dbRole);
            }

        }

        [ConcernFor("DeploymentPlan")]
        public class when_verifying_a_good_deployment_plan_with_one_task_across_three_servers_in_two_roles : with_a_good_model
        {
            public override void Because()
            {
                Result = Plan.Verify();
                System.Console.WriteLine(Result);
            }

            [Fact]
            public void should_have_three_results()
            {
                Assert.AreEqual(3, Result.ResultCount);
            }
        }

        [ConcernFor("DeploymentPlan")]
        public class when_executing_a_good_deployment_plan_with_one_verify_task_and_one_execute_task_across_three_servers_in_two_roles : with_a_good_model
        {
            public override void Because()
            {
                Result = Plan.Execute();
                System.Console.WriteLine(Result);
            }

            [Fact]
            public void should_have_six_results_due_to_the_verify_tasks_being_called()
            {
                Assert.AreEqual(6, Result.ResultCount);
            }
        }        
        
        [ConcernFor("DeploymentPlan")]
        public class when_executing_a_bad_deployment_plan_with_two_verify_tasks_and_two_execute_tasks_across_three_servers_in_two_roles : with_a_failing_model
        {
            public override void Because()
            {
                Result = Plan.Execute();
                System.Console.WriteLine(Result);
            }

            [Fact]
            public void should_have_five_results_due_to_the_verify_tasks_being_called_and_one_verify_failing()
            {
                Assert.AreEqual(5, Result.ResultCount);
            }
        }

    }
}
namespace dropkick.Tasks.WinService
{
    using System;
    using System.ServiceProcess;
    using DeploymentModel;
    using TimeoutException = System.ServiceProcess.TimeoutException;


    public class WinServiceStartTask :
        BaseServiceTask
    {
        public WinServiceStartTask(string machineName, string serviceName)
            : base(machineName, serviceName)
        {
        }

        public override string Name
        {
            get { return string.Format("Starting service '{0}'", ServiceName); }
        }

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            if (ServiceExists())
            {
                result.AddGood(string.Format("Found service '{0}'", ServiceName));
            }
            else
            {
                result.AddError(string.Format("Can't find service '{0}'", ServiceName));
            }

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            if (ServiceExists())
            {
                if(!dropkick.Wmi.WmiService.AuthenticationSpecified)
                {
                    using (var c = new ServiceController(ServiceName, MachineName))
                    {
                        Logging.Coarse("[svc] Starting service '{0}'", ServiceName);
                        try
                        {
                            c.Start();
                            LogCoarseGrain("[svc] Waiting up to 60 seconds because Windows can be silly");
                            c.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
                        }
                        catch (InvalidOperationException ex)
                        {
                            result.AddError("The service '{0}' did not start, most likely due to a logon issue.".FormatWith(ServiceName), ex);
                            LogCoarseGrain("The service '{0}' did not start, most likely due to a logon issue.{1}{2}", ServiceName, Environment.NewLine, ex);
                            return result;
                        }
                        catch (TimeoutException)
                        {
                            result.AddAlert("Service '{0}' did not finish starting during the specified timeframe.  You will need to manually verify if the service started successfully.", ServiceName);
                            LogCoarseGrain("Service '{0}' did not finish starting during the specified timeframe.  You will need to manually verify if the service started successfully.", ServiceName);
                            return result;
                        }
                    }
                }
                else 
                {
                    Logging.Coarse("[svc] Starting service '{0}'", ServiceName);
                    if (ServiceIsRunning())
                    {
                        LogCoarseGrain("[svc] Service '{0}' is already running".FormatWith(ServiceName));
                    }
                    else
                    {
                        try 
                        {
                            var returnCode = dropkick.Wmi.WmiService.Start(MachineName, ServiceName);
                            switch(returnCode)
                            {
                                case Wmi.ServiceReturnCode.Success:
                                    if(!ServiceIsRunning())
                                    {
                                        result.AddError("Failed to start service '{0}', most likely due to a logon issue.".FormatWith(ServiceName));
                                        LogCoarseGrain("The service '{0}' did not start, most likely due to a logon issue.", ServiceName);
                                        break;
                                    }
                                    else 
                                    {
                                        //good!
                                    }
                                    break;
                                default:
                                    result.AddError("Failed to start service '{0}', most likely due to a logon issue, result: {1}".FormatWith(ServiceName, returnCode));
                                    LogCoarseGrain("The service '{0}' did not start, most likely due to a logon issue.", ServiceName);
                                    break;
                            }
                        }
                        catch(Exception ex)
                        {
                            result.AddError("The service '{0}' did not start, most likely due to a logon issue.".FormatWith(ServiceName), ex);
                            LogCoarseGrain("The service '{0}' did not start, most likely due to a logon issue.{1}{2}", ServiceName, Environment.NewLine, ex);
                        }
                    }
                }
                result.AddGood("Started the service '{0}'", ServiceName);
            }
            else
            {
                result.AddAlert("Service '{0}' does not exist so it cannot be started", ServiceName);
            }

            return result;
        }
    }
}
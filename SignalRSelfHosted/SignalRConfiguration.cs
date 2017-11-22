using log4net;
using SimpleInjector;
using Topshelf;
using Topshelf.SimpleInjector;

namespace SignalRSelfHosted
{
    public class SignalRConfiguration
    {
        private readonly ILog _logger;

        public SignalRConfiguration(ILog logger)
        {
            this._logger = logger;
        }

        public void Run()
        {
            Container container = ConfigureService();

            HostFactory.Run(x =>
            {
                x.UseLinuxIfAvailable();

                x.UseLog4Net();

                // Pass it to Topshelf
                x.UseSimpleInjector(container);

                x.Service<SignalRService>(vs =>
                {
                    //vs.ConstructUsing(() => new SignalRService(_logger));//sem Ioc
                    vs.ConstructUsingSimpleInjector();
                    vs.WhenStarted((s, hostControl) => s.Start(hostControl));
                    vs.WhenStopped((s, hostControl) => s.Stop(hostControl));
                });
                x.RunAsLocalSystem();
                x.EnablePauseAndContinue();
                x.OnException(ex =>
                {
                    _logger.Error(ex);
                });
                x.SetDescription("SignalR SelfHosted Using TopShelf");
                x.SetDisplayName("SignalR SelfHosted");
                x.SetServiceName("SignalR.Server.SelfHosted");
            });
        }

        private static Container ConfigureService()
        {
            // Create a new Simple Injector container
            Container container = new Container();

            // Configure the Container
            ConfigureContainer(container);

            // Optionally verify the container's configuration to check for configuration errors.
            container.Verify();

            return container;
        }

        /// <summary>
        /// Register services here
        /// </summary>
        /// <param name="container"></param>
        private static void ConfigureContainer(Container container)
        {
            ////Register the service

            //container.Register<IInterface, ConcreteClass>();
            container.RegisterSingleton<ILog>(LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));
        }
    }
}

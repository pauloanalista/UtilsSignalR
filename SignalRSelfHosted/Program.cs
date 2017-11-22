using log4net;

namespace SignalRSelfHosted
{
    class Program
    {
        private static ILog _logger;

        static void Main(string[] args)
        {
            new SignalRConfiguration(_logger).Run();
        }
    }
}

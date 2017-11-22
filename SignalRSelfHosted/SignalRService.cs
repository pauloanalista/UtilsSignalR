using log4net;
using Microsoft.Owin.Hosting;
using System;
using System.Configuration;
using Topshelf;

namespace SignalRSelfHosted
{
    public class SignalRService : ServiceControl
    {
        private readonly string _uriService = ConfigurationManager.AppSettings["URI_SERVICE"];
        private readonly ILog _logger;

        IDisposable SignalR { get; set; }

        public SignalRService(ILog logger)
        {
            this._logger = logger;
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                _logger.Info("Starting Service SignalR...");

                SignalR = WebApp.Start(_uriService);

                _logger.Info("SignalR Service Started and Running in: " + _uriService);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            try
            {
                _logger.Info("SignalR Service Stopped.");
                SignalR.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }
        }
    }
}

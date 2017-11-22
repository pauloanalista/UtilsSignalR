using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Reflection;

namespace SignalRSelfHosted
{
    public class Startup
    {
        private ILog _logger;

        public Startup(ILog logger)
        {
            this._logger = logger;
        }

        public void Configuration(IAppBuilder app)
        {
            try
            {
                app.Map("/signalr", map =>
                {
                    map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration()
                    {
                        EnableDetailedErrors = true,
                        EnableJSONP = true
                    };

                    map.RunSignalR(hubConfiguration);
                });

                ConfigureJson();

                GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null;

            }
            catch (Exception ex)
            {
                this._logger.Error(ex);
                throw;
            }
        }

        private void ConfigureJson()
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);
        }
    }



    internal class SignalRContractResolver : IContractResolver
    {
        private readonly Assembly _assembly;
        private readonly IContractResolver _camelCaseContractResolver;
        private readonly IContractResolver _defaultContractSerializer;

        public SignalRContractResolver()
        {
            _defaultContractSerializer = new DefaultContractResolver();
            _camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
            _assembly = typeof(Hub).Assembly;
        }

        #region IContractResolver Members

        public JsonContract ResolveContract(Type type)
        {
            if (type.Assembly.Equals(_assembly))
                return _defaultContractSerializer.ResolveContract(type);

            return _camelCaseContractResolver.ResolveContract(type);
        }

        #endregion
    }
}

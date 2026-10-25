using Microsoft.Xrm.Sdk;
using System;

namespace Plugins.Warehouse
{
    public abstract class PluginBase : IPlugin
    {
        private readonly Type pluginType;

        protected PluginBase(Type pluginType)
        {
            this.pluginType = pluginType;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var localPluginContext = new LocalPluginContext(serviceProvider);

            try
            {
                ExecuteDataversePlugin(localPluginContext);
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception exception)
            {
                localPluginContext.TracingService.Trace("{0}: {1}", pluginType.FullName, exception);
                throw;
            }
        }

        protected abstract void ExecuteDataversePlugin(ILocalPluginContext localPluginContext);
    }
}

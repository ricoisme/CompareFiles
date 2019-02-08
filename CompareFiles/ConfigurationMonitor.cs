using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompareFiles
{
    public interface IConfigurationMonitor
    {
        byte[] ConfigSectionHash { get; set; }
    }

    public sealed class ConfigurationMonitor : IConfigurationMonitor
    {
        byte[] IConfigurationMonitor.ConfigSectionHash { get; set; }
        public ConfigurationMonitor(IConfiguration config, Action<IConfiguration> InvokeChanged)
        {
            //_env = env;
            //_config = config;        
            ChangeToken.OnChange(
                () => config.GetReloadToken(),
                InvokeChanged, config);
        }
    }
}

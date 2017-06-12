using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Storm.ImageProcessor.Web.Config;
using Storm.ImageProcessor.Web.Extensions;

namespace Storm.ImageProcessor.Web.Plugins.AzureBlobCache
{
    public class FromConfigAzureBlobCache : global::ImageProcessor.Web.Plugins.AzureBlobCache.AzureBlobCache
    {
        public FromConfigAzureBlobCache(string requestPath, string fullPath, string querystring) : base(requestPath, fullPath, querystring)
        {
        }

        protected sealed override void AugmentSettings(Dictionary<string, string> settings)
        {
            var prefix = GetConfigurationPrefix();
            var removePrefix = new Func<string, string>(key => key.Replace(prefix, string.Empty));

            var matchingKeys = ReadConfigurationKeys(prefix);
            foreach (var key in matchingKeys)
            {
                AddOrUpdateConfigValue(settings, removePrefix(key), key.FromAppSettings());
            }
        }

        protected string GetConfigurationPrefix()
        {
            return $"{nameof(global::ImageProcessor.Web.Plugins.AzureBlobCache.AzureBlobCache)}.";
        }

        protected virtual IEnumerable<string> ReadConfigurationKeys(string prefix)
        {
            return ConfigurationManager.AppSettings.AllKeys.Where(
                key => key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        protected virtual void AddOrUpdateConfigValue(Dictionary<string, string> settings, string key, string value)
        {
            settings.AddOrUpdate(key, value);
        }
    }
}
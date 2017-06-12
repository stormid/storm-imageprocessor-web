using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessor.Web.Helpers;
using ImageProcessor.Web.Services;
using Storm.ImageProcessor.Web.Config;

namespace Storm.ImageProcessor.Web.Services
{
    public class FromConfigCloudImageService : IImageService
    {
        private const string AppSettingsPrefix = nameof(CloudImageService);

        private readonly IDictionary<string, string> defaultSettings = new Dictionary<string, string>
        {
            { "MaxBytes", "8194304" },
            { "Timeout", "30000" },
            { "AppSettingsPrefix", AppSettingsPrefix }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FromConfigCloudImageService"/> class.
        /// </summary>
        public FromConfigCloudImageService()
        {
            Settings = new Dictionary<string, string>(defaultSettings);
        }

        /// <summary>
        /// Gets or sets the prefix for the given implementation.
        /// <remarks>
        /// This value is used as a prefix for any image requests that should use this service.
        /// </remarks>
        /// </summary>
        public string Prefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the image service requests files from
        /// the locally based file system.
        /// </summary>
        public bool IsFileLocalService => false;

        //private readonly Dictionary<string, string> settings;

        /// <summary>
        /// Gets or sets any additional settings required by the service.
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Gets or sets the white list of <see cref="System.Uri"/>.
        /// </summary>
        public Uri[] WhiteList { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current request passes sanitizing rules.
        /// </summary>
        /// <param name="path">
        /// The image path.
        /// </param>
        /// <returns>
        /// <c>True</c> if the request is valid; otherwise, <c>False</c>.
        /// </returns>
        public bool IsValidRequest(string path)
        {
            return ImageHelpers.IsValidImageExtension(path);
        }

        private Dictionary<string, string> ExpandSettings(Dictionary<string, string> settings)
        {
            var settingsPrefix = $"{(Settings.ContainsKey(nameof(AppSettingsPrefix)) ? Settings[nameof(AppSettingsPrefix)] : AppSettingsPrefix)}.";

            var appSettings = defaultSettings.Union(ConfigurationManager.AppSettings.AllKeys
                .Where(x => x.StartsWith(settingsPrefix))
                .ToDictionary(k => k.Replace(settingsPrefix, ""), v => v.FromAppSettings()));

            return settings.Union(appSettings).ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>
        /// Delegating to the cloud image service still, just to ensure as much consistency as possible (until this is merged and I can switch to using RemoteFile)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="settings"></param>
        /// <param name="prefix"></param>
        /// <param name="whitelist"></param>
        /// <returns></returns>
        private static async Task<byte[]> GetImage(object id, Dictionary<string, string> settings, string prefix, Uri[] whitelist)
        {
            var cloudImageService = new CloudImageService
            {
                Settings = settings,
                Prefix = prefix,
                WhiteList = whitelist
            };
            return await cloudImageService.GetImage(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the image using the given identifier.
        /// </summary>
        /// <param name="id">
        /// The value identifying the image to fetch.
        /// </param>
        /// <returns>
        /// The <see cref="System.Byte"/> array containing the image data.
        /// </returns>
        public async Task<byte[]> GetImage(object id)
        {
            var settings = ExpandSettings(Settings);

            return await GetImage(id, settings, Prefix, WhiteList).ConfigureAwait(false);
        }
    }
}
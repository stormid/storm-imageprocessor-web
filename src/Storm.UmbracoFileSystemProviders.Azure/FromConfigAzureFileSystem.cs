using Our.Umbraco.FileSystemProviders.Azure;
using Storm.Config;

namespace Storm.UmbracoFileSystemProviders.Azure
{
    public class FromConfigAzureFileSystem : AzureBlobFileSystem
    {
        private const string ContainerName = "containerName";
        private const string RootUrl = "rootUrl";
        private const string ConnectionString = "connectionString";
        private const string MaxDays = "maxDays";
        private const string UseDefaultRoute = "useDefaultRoute";

        private static readonly string Prefix = $"{nameof(AzureBlobFileSystem)}.";

        public FromConfigAzureFileSystem() : base(
            ContainerName.FromAppSettingsWithPrefix(Prefix, Constants.DefaultMediaRoute), 
            RootUrl.FromAppSettingsWithPrefix(Prefix), 
            ConnectionString.FromAppSettingsWithPrefix(Prefix), 
            MaxDays.FromAppSettingsWithPrefix(Prefix, "365"), 
            UseDefaultRoute.FromAppSettingsWithPrefix(Prefix, "true"))
        {
        }

        protected FromConfigAzureFileSystem(string prefix) : base(
            ContainerName.FromAppSettingsWithPrefix(prefix, Constants.DefaultMediaRoute),
            RootUrl.FromAppSettingsWithPrefix(prefix),
            ConnectionString.FromAppSettingsWithPrefix(prefix),
            MaxDays.FromAppSettingsWithPrefix(prefix, "365"),
            UseDefaultRoute.FromAppSettingsWithPrefix(prefix, "true"))
        { }
    }
}
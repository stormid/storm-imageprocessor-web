# Storm.ImageProcessor.Web

This library extends the Azure blob storage functionality within the excellent [ImageProcessor](http://imageprocessor.org) by [James jackson-South](https://about.me/james_south) to support management of configuration settings via the ```<appSettings/>``` configuration section.

## Purpose

The allow a project to benefit from the ImageProcessor library and its ability to offload media to external blob storage accounts while removing the need for sensitive, environment specific information to be stored within library specific configuration files.

## Implementation

The library provides derived implementations of the ```AzureBlobCache``` and ```CloudImageService``` that look to read and augment the standard configuration section settings with values provided within the ```<appSettings/>``` section.

In addition there is an Umbraco library that itself extends James' [AzureFileSystem](https://github.com/JimBobSquarePants/UmbracoFileSystemProviders.Azure) in a similar way, to support configuration via ```<appSettings/>```.

## Installation

There are 2 separate nuget packages available, one specifically for ImageProcessor and the other specific to the file system abstraction within Umbraco.  If you are using this within an Umbraco project you will need both packages installed:

```powershell
Install-Package Storm.ImageProcessor.Web

Install-Package Storm.UmbracoFileSystemProviders.Azure
```

## Configuring Storm.ImageProcessor.Web

Once installed via nuget both the ```cache.config``` and ```security.config``` files will be updated.  Each will contain specific cache and service configuration to ensure image processor uses the derived implementations found within this library.

### config/imageprocessor/cache.config

```xml
    <cache name="FromConfigAzureBlobCache" type="Storm.ImageProcessor.Web.Plugins.AzureBlobCache.FromConfigAzureBlobCache, Storm.ImageProcessor.Web" maxDays="365">
      <settings>
        <setting key="CachedBlobContainer" value="cache" />
        <setting key="UseCachedContainerInUrl" value="true" />
        <setting key="CachedCDNTimeout" value="1000" />
        <setting key="StreamCachedImage" value="false" />
        <setting key="SourceBlobContainer" value="media" />
      </settings>
    </cache>
```

The same settings keys can be set within the configuration, however additionally the ```web.config``` will be searched for updates to any supplied values, if a value is found within the ```<appSettings/>``` then its value will override any set in the ```cache.config```:

```xml
  <appSettings>
    <add key="AzureBlobCache.CachedStorageAccount" value="DefaultEndpointsProtocol=https;AccountName=[CacheAccountName];AccountKey=[CacheAccountKey]" />
    <add key="AzureBlobCache.CachedCDNRoot" value="[CdnRootUrl]" />
    <add key="AzureBlobCache.SourceStorageAccount" value="DefaultEndpointsProtocol=https;AccountName=[StorageAccountName];AccountKey=[StorageAccountName]" />

    <add key="CloudImageService.Host" value="https://[account].blob.core.windows.net/media/" />
  </appSettings>
```

### config/imageprocessor/security.config

```xml
    <service prefix="media/" name="CloudImageService" type="Storm.ImageProcessor.Web.Services.FromConfigCloudImageService, Storm.ImageProcessor.Web">
      <settings>
        <setting key="MaxBytes" value="8194304"/>
        <setting key="Timeout" value="30000"/>
      </settings>
    </service>
```

The same settings keys can be set within the configuration, however additionally the ```web.config``` will be searched for updates to any supplied values, if a value is found within the ```<appSettings/>``` then its value will override any set in the ```cache.config```:

```xml
  <appSettings>
    <add key="CloudImageService.Host" value="https://[account].blob.core.windows.net/media/" />
  </appSettings>
```

## Configuring Storm.UmbracoFileSystemProviders.Azure

Once installed via nuget the ```FileSystemProviders.config```file will be updated to use the derived library implementation:

```xml
  <Provider alias="media" type="Storm.UmbracoFileSystemProviders.Azure.FromConfigAzureFileSystem, Storm.UmbracoFileSystemProviders.Azure" />
```

The same settings keys can be set within the configuration, however additionally the ```web.config``` will be searched for updates to any supplied values, if a value is found within the ```<appSettings/>``` then its value will override any set in the ```cache.config```:

```xml
  <appSettings>
    <add key="AzureBlobFileSystem.RootUrl" value="https://[account].blob.core.windows.net/" />
    <add key="AzureBlobFileSystem.ConnectionString" value="DefaultEndpointsProtocol=https;AccountName=[AccountName];AccountKey=[AccountKey]" />
  </appSettings>

```

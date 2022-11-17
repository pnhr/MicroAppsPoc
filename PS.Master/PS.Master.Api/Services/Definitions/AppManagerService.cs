using AutoMapper;
using Microsoft.Web.Administration;
using PS.Master.Api.Services.Interfaces;
using System;
using System.IO;
using System.DirectoryServices;
using System.Collections;
using PS.Master.Data;
using PS.Master.Domain.Models;
using PS.Master.Domain;
using PS.Master.ViewModels.Models;
using static MudBlazor.CategoryTypes;
using System.Diagnostics;

namespace PS.Master.Api.Services.Definitions
{
    public class AppManagerService : IAppManagerService
    {
        private readonly IConfigRepository _configRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<SampleService> _logger;

        public AppManagerService(IConfigRepository configRepo, IMapper mapper, IConfiguration config, ILogger<SampleService> logger)
        {
            this._configRepo = configRepo;
            this._mapper = mapper;
            this._config = config;
            this._logger = logger;
        }

        public HttpRequest Request { get; set; }
        public async Task<string> DeployWebApplication(AppArtifacts appArtifacts)
        {
            MasterConfig appDeployPathConfig = await _configRepo.GetConfig(AppConstants.MasterConfigKeys.AppDeployPath);
            if (appDeployPathConfig == null)
                return "";

            string appPhysicalPath = await CreateVirtualDir(appArtifacts.AppName, 
                                                                appDeployPathConfig.Value, 
                                                                AppConstants.AppHostConstants.MasterSiteHostName, 
                                                                AppConstants.AppHostConstants.MasterSiteId);
            return appPhysicalPath;
        }
        private async Task<string> CreateVirtualDir(string appName, string rootPath, string hostName, int siteId)
        {
            
            try
            {
                string appPhysicalPath = $"{rootPath}\\{appName}";

                if (!Directory.Exists(appPhysicalPath))
                    Directory.CreateDirectory(appPhysicalPath);

                string iisUri = string.Format(AppConstants.AppHostConstants.IISPath, hostName, siteId.ToString()); //"IIS://Localhost/W3SVC/3/Root"

                CreateVDir(iisUri, appName, appPhysicalPath);

                if (File.Exists(AppConstants.AppHostConstants.SampleIndexHtmlFile))
                {
                    FileInfo fileInfo = new FileInfo(AppConstants.AppHostConstants.SampleIndexHtmlFile);
                    string fileName = fileInfo.Name;
                    fileInfo.CopyTo(Path.Combine(appPhysicalPath, fileName), true);
                }
                
                string link = $"{AppConstants.AppHostConstants.MasterSiteProtocols}://{hostName}:{AppConstants.AppHostConstants.MasterSitePort}/{appName}";

#if !DEBUG
                if (Request != null)
                {
                    string masterAppUrl = $"{Request.Scheme}://{Request.Host}:{Request.Host.Port ?? 80}";
                    if (!string.IsNullOrWhiteSpace(masterAppUrl))
                    {
                        link = $"{masterAppUrl}/{appName}";
                    }
                }
#endif


                return await Task.FromResult(link);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private static void CreateVDir(string metabasePath, string vDirName, string physicalPath)
        {
            DirectoryEntry site = new DirectoryEntry(metabasePath);
            string className = site.SchemaClassName.ToString();

            if ((className.EndsWith("Server")) || (className.EndsWith("VirtualDir")))
            {
                string appRoot = "/LM" + metabasePath.Substring(metabasePath.IndexOf("/", ("IIS://".Length)));

                DirectoryEntries vdirs = site.Children;
                DirectoryEntry newVDir = vdirs.Add(vDirName, (className.Replace("Service", "VirtualDir")));
                newVDir.Properties["Path"][0] = physicalPath;
                newVDir.Properties["AccessScript"][0] = true;
                // These properties are necessary for an application to be created.
                newVDir.Properties["AppFriendlyName"][0] = vDirName;
                newVDir.Properties["AppIsolated"][0] = "1";
                newVDir.Properties["AppRoot"][0] = appRoot;

                newVDir.CommitChanges();

                Console.WriteLine(" Done.");
            }
            else
                Console.WriteLine(" Failed. A virtual directory can only be created in a site or virtual directory node.");
        }

        
    }
}

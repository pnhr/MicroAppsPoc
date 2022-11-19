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
using PS.Master.Domain.DbModels;
using Microsoft.IdentityModel.Tokens;
using Sys = System.Drawing;
using System.Text;
using System.Security.Policy;
using PS.Master.Data.Definitions;

namespace PS.Master.Api.Services.Definitions
{
    public class AppManagerService : IAppManagerService
    {
        private readonly IAppManagerRepository _appManagerRepo;
        private readonly IConfigRepository _configRepo;
        private readonly IAppFileService _appFileService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<SampleService> _logger;

        public AppManagerService(IAppManagerRepository appManagerRepository, IConfigRepository configRepo, IAppFileService appFileService, IMapper mapper, IConfiguration config, ILogger<SampleService> logger)
        {
            this._appManagerRepo = appManagerRepository;
            this._configRepo = configRepo;
            this._appFileService = appFileService;
            this._mapper = mapper;
            this._config = config;
            this._logger = logger;
        }

        public async Task<DeployResult> DeployWebApplication(AppArtifacts appArtifacts)
        {
            if (appArtifacts == null || appArtifacts.AppFiles == null || appArtifacts.AppFiles.Count == 0)
                throw new ArgumentNullException("AppArtifacts object is either null or no files found");

            DeployResult deployResult = new DeployResult();
            AppServer appServer = null;
            if (!string.IsNullOrEmpty(appArtifacts.SelectedAppServerCode))
            {
                appServer = await _appManagerRepo.GetActiveAppServers(appArtifacts.SelectedAppServerCode);
            }
            else
            {
                appServer = await _appManagerRepo.GetActiveAppServers(AppConstants.AppHostConstants.DefaultAppServer);
            }

            if (appServer == null || string.IsNullOrEmpty(appServer.DeployRootPath))
                throw new Exception("Default app server are not configured in master app");

            deployResult.AppUrl = await CreateVirtualDir(appName: appArtifacts.AppName,
                                                                rootPath: appServer.DeployRootPath,
                                                                hostName: appServer.ServerName,
                                                                siteId: appServer.MasterWebSiteId);

            deployResult.AppLogo = GetLogo("");

            var files = Directory.GetFiles(appArtifacts.AppZipFileStageFolderPath, "*.zip", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                await _appFileService.UnZipFile(file, Path.Combine(appServer.DeployRootPath, appArtifacts.AppName));
            }
            
            ApplicationHost appHost = new ApplicationHost();
            appHost.AppName = appArtifacts.AppName;
            appHost.AppDiscription = appArtifacts.AppDiscription;
            appHost.AppRootPath = appServer.DeployRootPath;
            appHost.AppVPath = deployResult.AppUrl;
            appHost.IsActive = true;
            appHost.AppLogo = deployResult.AppLogo;
            appHost.CreatedBy = "dbo";
            appHost.CreatedDate = DateTime.Now;

            await _appManagerRepo.AddApplication(appHost);

            return deployResult;
        }

        public async Task<DeployResult> DeployWebApplication(AppArtifacts appArtifacts, string masterAppBaseUri)
        {
            if (appArtifacts == null || appArtifacts.AppFiles == null || appArtifacts.AppFiles.Count == 0)
                throw new ArgumentNullException("AppArtifacts object is either null or no files found");

            DeployResult deployResult = new DeployResult();
            AppServer appServer = null;
            if (!string.IsNullOrEmpty(appArtifacts.SelectedAppServerCode))
            {
                appServer = await _appManagerRepo.GetActiveAppServers(appArtifacts.SelectedAppServerCode);
            }
            else
            {
                appServer = await _appManagerRepo.GetActiveAppServers(AppConstants.AppHostConstants.DefaultAppServer);
            }

            if (appServer == null || string.IsNullOrEmpty(appServer.DeployRootPath))
                throw new Exception("Default app server are not configured in master app");

            deployResult.AppUrl = await CreateVirtualDir(appName: appArtifacts.AppName,
                                                                rootPath: appServer.DeployRootPath,
                                                                hostName: appServer.ServerName,
                                                                siteId: appServer.MasterWebSiteId,
                                                                masterAppBaseUri: masterAppBaseUri);

            deployResult.AppLogo = GetLogo("");

            var files = Directory.GetFiles(appArtifacts.AppZipFileStageFolderPath, "*.zip", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                await _appFileService.UnZipFile(file, Path.Combine(appServer.DeployRootPath, appArtifacts.AppName));
            }

            ApplicationHost appHost = new ApplicationHost();
            appHost.AppName = appArtifacts.AppName;
            appHost.AppDiscription = appArtifacts.AppDiscription;
            appHost.AppRootPath = appServer.DeployRootPath;
            appHost.AppVPath = appArtifacts.AppName;
            appHost.IsActive = true;
            appHost.AppLogo = deployResult.AppLogo;

            await _appManagerRepo.AddApplication(appHost);

            return deployResult;
        }
        public async Task<List<DeployResult>> GetDeployedApps()
        {
            var apps = await _appManagerRepo.GetDeployedApps();
            List<DeployResult> appsVm = apps.Select(x => new DeployResult { AppLogo = x.AppLogo, AppName = x.AppName, AppUrl = x.AppUrl }).ToList();
            return appsVm;
        }
        private async Task<string> CreateVirtualDir(string appName, string rootPath, string hostName, int siteId, string masterAppBaseUri = "")
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

            if (!string.IsNullOrWhiteSpace(masterAppBaseUri))
            {
                link = $"{masterAppBaseUri}/{appName}";
            }

            return await Task.FromResult(link);
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

        private string GetLogo(string appZipFilePath)
        {
            string logo = "";
            byte[] logoBytes;
            Sys.Image logoImg = Sys.Image.FromFile("C:\\Projects\\Logo\\ps_logo.png");

            using (var ms = new MemoryStream())
            {
                logoImg.Save(ms, logoImg.RawFormat);
                logoBytes = ms.ToArray();
            }

            if (logoBytes != null)
            {
                //logo = Encoding.Default.GetString(logoBytes);
                logo = Convert.ToBase64String(logoBytes);
            }

            return logo;
        }

        
    }
}

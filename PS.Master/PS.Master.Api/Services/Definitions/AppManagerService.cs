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
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Linq;

namespace PS.Master.Api.Services.Definitions
{
    public class AppManagerService : IAppManagerService
    {
        private readonly IAppManagerRepository _appManagerRepo;
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly IAdoRepo _adoRepo;
        private readonly IConfigRepository _configRepo;
        private readonly IAppFileService _appFileService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<SampleService> _logger;

        public AppManagerService(IAppManagerRepository appManagerRepository, IWebHostEnvironment webHostEnv, IAdoRepo adoRepo, IConfigRepository configRepo, IAppFileService appFileService, IMapper mapper, IConfiguration config, ILogger<SampleService> logger)
        {
            this._appManagerRepo = appManagerRepository;
            this._webHostEnv = webHostEnv;
            this._adoRepo = adoRepo;
            this._configRepo = configRepo;
            this._appFileService = appFileService;
            this._mapper = mapper;
            this._config = config;
            this._logger = logger;
        }

        public async Task<DeployResult> DeployWebApplication(AppArtifacts appArtifacts)
        {
            if (appArtifacts == null || appArtifacts.AppFiles == null || appArtifacts.AppFiles.Count == 0)
                throw new ArgumentNullException(AppConstants.ErrorMessage.AppArtifactsNull);

            AppServer appServer = await GetAppServer(appArtifacts);
            DeployResult deployResult = new DeployResult();
            deployResult.AppUrl = await CreateVirtualDir(appArtifacts, appServer);
            await PublishFiles(appArtifacts, appServer.DeployRootPath, deployResult);

            await SaveToDatabase(appArtifacts, appServer, deployResult);

            return deployResult;
        }

        public async Task<List<DeployResult>> GetDeployedApps()
        {
            var apps = await _appManagerRepo.GetDeployedApps();
            List<DeployResult> appsVm = apps.Select(x => new DeployResult { AppLogo = x.AppLogo, 
                                                                            AppName = x.AppName, 
                                                                            AppUrl = x.AppUrl, 
                                                                            AppDisplayName = x.AppDisplayName,
                                                                            AppDiscription = x.AppDiscription }).ToList();
            return appsVm;
        }

        public async Task<string> CreateWebsite(AppServer appServer, string siteName, bool isDefaultAppPoolNeeded = true)
        {
            try
            {
                string appPoolName = siteName;
                if (isDefaultAppPoolNeeded)
                    appPoolName = AppConstants.AppHostConstants.DefaultAppPool;

                //Initialize configuration variables
                string metabasePath = string.Format(AppConstants.AppHostConstants.IISWebPath, appServer.ServerName);
                string physicalPath = appServer.DeployRootPath;
                string defaultAppPool = appPoolName;

                object[] hosts = new object[1];
                string hostHeader = siteName.Replace("www.", string.Empty).Replace(".com", string.Empty);
                hosts[0] = $":{AppConstants.AppHostConstants.SiteDefaultPort}:" + hostHeader;

                //Gets unique site id for the new website
                int siteId = GetUniqueSiteId(metabasePath);

                //Extracts the directory entry
                DirectoryEntry objDirEntry = new DirectoryEntry(metabasePath);
                string className = objDirEntry.SchemaClassName;

                if (!className.EndsWith("Service")) 
                    return "Invalid configuration variables";

                //creates new website by specifying site name and host header
                DirectoryEntry newSite = objDirEntry.Children.Add(Convert.ToString(siteId), (className.Replace("Service", "Server")));
                newSite.Properties["ServerComment"][0] = siteName;
                newSite.Properties["ServerBindings"].Value = hosts;
                newSite.Invoke("Put", "ServerAutoStart", 1);
                newSite.Invoke("Put", "ServerSize", 1);
                newSite.CommitChanges();

                //Creates root directory by specifying the local path, default  document and permissions
                DirectoryEntry newSiteVDir = newSite.Children.Add("Root", "IIsWebVirtualDir");
                newSiteVDir.Properties["Path"][0] = physicalPath;
                newSiteVDir.Properties["EnableDefaultDoc"][0] = true;
                newSiteVDir.Properties["DefaultDoc"].Value = "default.aspx,index.html,index.htm";
                newSiteVDir.Properties["AppIsolated"][0] = 2;
                newSiteVDir.Properties["AccessRead"][0] = true;
                newSiteVDir.Properties["AccessWrite"][0] = false;
                newSiteVDir.Properties["AccessScript"][0] = true;
                newSiteVDir.Properties["AccessFlags"].Value = 513;
                newSiteVDir.Properties["AppRoot"][0] = @"/LM/W3SVC/" + Convert.ToString(siteId) + "/Root";
                newSiteVDir.Properties["AppPoolId"].Value = defaultAppPool;
                newSiteVDir.Properties["AuthNTLM"][0] = true;
                newSiteVDir.Properties["AuthAnonymous"][0] = true;
                newSiteVDir.CommitChanges();

                PropertyValueCollection lstScriptMaps = newSiteVDir.Properties["ScriptMaps"];
                ArrayList arrScriptMaps = new ArrayList();

                foreach (string scriptMap in lstScriptMaps)
                {
                    arrScriptMaps.Add(scriptMap);
                }

                newSiteVDir.Properties["ScriptMaps"].Value = arrScriptMaps.ToArray();
                newSiteVDir.CommitChanges();
                return await Task.FromResult("Website created successfully.");
            }
            catch (Exception ex)
            {
                return await Task.FromResult("Website creation failed. <br/>" + ex.Message);
            }
        }

        private async Task<AppServer> GetAppServer(AppArtifacts appArtifacts)
        {
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
                throw new Exception(AppConstants.ErrorMessage.DefaultServerNotConfigured);

            return appServer;
        }

        private async Task SaveToDatabase(AppArtifacts appArtifacts, AppServer appServer, DeployResult deployResult)
        {
            ApplicationHost appHost = new ApplicationHost();
            appHost.AppName = appArtifacts.AppName;
            appHost.AppDisplayName = appArtifacts.AppDisplayName;
            appHost.AppDiscription = appArtifacts.AppDiscription;
            appHost.AppRootPath = appServer.DeployRootPath;
            appHost.AppVPath = deployResult.AppUrl;
            appHost.IsActive = true;
            appHost.AppLogo = deployResult.AppLogo;
            appHost.CreatedBy = "dbo";
            appHost.CreatedDate = DateTime.Now;
            await _appManagerRepo.AddApplication(appHost);
        }

        private async Task<DeploySettings> GetDeployedSettings(string stageFolderPath)
        {
            string file = Path.Combine(stageFolderPath, AppConstants.AppHostConstants.DeploySettingsFileName);
            if (!File.Exists(file))
                throw new Exception("Deployment setting file not included");

            string jsonString = File.ReadAllText(file);
            DeploySettings deploy = JsonSerializer.Deserialize<DeploySettings>(jsonString)!;

            if (deploy == null)
                throw new Exception("Something went wrong while deserializing deployment requirments");

            if (deploy.IsDbNeeded && string.IsNullOrEmpty(deploy.DbZipFile))
                throw new Exception("Databse file name did not mentioned");

            return await Task.FromResult(deploy);
        }
        private async Task PublishFiles(AppArtifacts appArtifacts, string deployRootPath, DeployResult deployResult)
        {
            await _appFileService.UnZipAllFiles(appArtifacts.AppZipFileStageFolderPath);
            var deploymentSettings = await GetDeployedSettings(appArtifacts.AppZipFileStageFolderPath);

            if (deploymentSettings.IsDbNeeded)
            {
                await PublishDb(deploymentSettings, appArtifacts.AppZipFileStageFolderPath, deployRootPath);
            }

            await PublishWebApp(deploymentSettings, appArtifacts.AppZipFileStageFolderPath, deployRootPath);

            string publishFilePath = Path.Combine(deployRootPath, appArtifacts.AppName);
            deployResult.AppLogo = GetLogo(publishFilePath, deploymentSettings.AppName, deploymentSettings.LogoFileName);
        }

        private async Task PublishWebApp(DeploySettings deploymentSettings, string stageFolderPath, string deployRootPath)
        {
            var files = Directory.GetFiles(stageFolderPath, "*.zip", SearchOption.TopDirectoryOnly);
            string file = files.FirstOrDefault(x => x.ToLower().Contains(deploymentSettings.WebAppZipFile.ToLower()));
            if (file != null)
            {
                await _appFileService.UnZipFile(file, Path.Combine(deployRootPath, deploymentSettings.AppName));
            }
        }
        private async Task PublishDb(DeploySettings deploymentSettings, string stageFolderPath, string deployRootPath)
        {
            var files = Directory.GetFiles(stageFolderPath, "*.zip", SearchOption.TopDirectoryOnly);
            string file = files.FirstOrDefault(x => x.ToLower().Contains(deploymentSettings.DbZipFile.ToLower()));
            if (file != null)
            {
                await _appFileService.UnZipFile(file, stageFolderPath);
                var sqlFiles = Directory.GetFiles(stageFolderPath, "*.sql", SearchOption.TopDirectoryOnly).ToList().OrderBy(x => x).ToList();
                await _appManagerRepo.ExecuteDbScript(sqlFiles[0], deploymentSettings.DatabaseName, deploymentSettings.ConnStr);
            }
        }

        private int GetUniqueSiteId(string metabasePath)
        {
            int siteId = 1;
            DirectoryEntry objDirEntry = new DirectoryEntry(metabasePath);
            foreach (DirectoryEntry e in objDirEntry.Children)
            {
                if (e.SchemaClassName == "IIsWebServer")
                {
                    int id = Convert.ToInt32(e.Name);
                    if (id >= siteId)
                        siteId = id + 1;
                }
            }
            return siteId;
        }

        private async Task<string> CreateVirtualDir(AppArtifacts appArtifacts, AppServer appServer)
        {
            string appName = appArtifacts.AppName;
            string rootPath = appServer.DeployRootPath;
            string hostName = appServer.ServerName;
            int siteId = appServer.MasterWebSiteId;
            string masterAppBaseUri = "";

            string link = $"{AppConstants.AppHostConstants.MasterSiteProtocols}://{hostName}:{AppConstants.AppHostConstants.MasterSitePort}/{appName}";
            if (!string.IsNullOrWhiteSpace(masterAppBaseUri))
            {
                link = $"{masterAppBaseUri}/{appName}";
            }

            string appPhysicalPath = $"{rootPath}\\{appName}";
            if (!Directory.Exists(appPhysicalPath))
                Directory.CreateDirectory(appPhysicalPath);

            string iisUri = string.Format(AppConstants.AppHostConstants.IISWebSiteMetaPath, hostName, siteId.ToString()); //"IIS://Localhost/W3SVC/3/Root"

            //string appPool = CreateAppPool(appName, AppConstants.AppHostConstants.IISWebPath);
            string appPool = "";
            CreateApplication(iisUri, appName, appPhysicalPath, appPool);

            return await Task.FromResult(link);
        }
        private string CreateAppPool(string appPoolName, string metabasePath)
        {
            try
            {
                if (!metabasePath.ToLower().Contains("apppools"))
                    metabasePath = $"{metabasePath}/AppPools";

                if (metabasePath.EndsWith("/W3SVC/AppPools"))
                {
                    DirectoryEntry newpool;
                    DirectoryEntry apppools = new DirectoryEntry(metabasePath);
                    newpool = apppools.Children.Add(appPoolName, "IIsApplicationPool");
                    newpool.CommitChanges();
                }
                else
                    throw new Exception(" Failed in CreateAppPool; application pools can only be created in the */W3SVC/AppPools node.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed in CreateAppPool with the following exception: \n{ex.Message}");
            }
            return appPoolName;
        }
        private static void CreateApplication(string metabasePath, string vDirName, string physicalPath, string appPool)
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
            }
            else
                throw new Exception(AppConstants.ErrorMessage.vDirCreationFailed);
        }

        private string GetLogo(string appZipFilePath, string appName, string logoFileName = "")
        {
            string logo = "";
            byte[] logoBytes;

            string defaultLogoFilePath = Path.Combine(_webHostEnv.ContentRootPath, "StaticFiles", "ps_logo.png");

            Sys.Image logoImg = Sys.Image.FromFile(defaultLogoFilePath);

            if (!string.IsNullOrEmpty(logoFileName))
            {
                string appLogoFile = Path.Combine(appZipFilePath, logoFileName);
                if (File.Exists(appLogoFile))
                {
                    logoImg = Sys.Image.FromFile(appLogoFile);
                }
            }

            using (var ms = new MemoryStream())
            {
                logoImg.Save(ms, logoImg.RawFormat);
                logoBytes = ms.ToArray();
            }

            if (logoBytes != null)
            {
                logo = Convert.ToBase64String(logoBytes);
            }

            return logo;
        }

        public async Task<bool> ExecuteDbScript(string scriptPath)
        {
            List<string> batches = new List<string>();
            bool isCreated = false;
            if (File.Exists(scriptPath))
            {
                string script = File.ReadAllText(scriptPath);
                if (!string.IsNullOrEmpty(script))
                {
                    batches = script.Split("GO")?.ToList();

                    isCreated = await _adoRepo.CreateDatabase(batches, "");
                }
            }

            return isCreated;
        }
    }
}

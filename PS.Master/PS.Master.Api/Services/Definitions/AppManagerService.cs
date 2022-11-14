using AutoMapper;
using Microsoft.Web.Administration;
using PS.Master.Api.Services.Interfaces;
using System;
using System.IO;
using System.DirectoryServices;
using System.Collections;

namespace PS.Master.Api.Services.Definitions
{
    public class AppManagerService : IAppManagerService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<SampleService> _logger;

        public AppManagerService(IMapper mapper, IConfiguration config, ILogger<SampleService> logger)
        {
            this._mapper = mapper;
            this._config = config;
            this._logger = logger;
        }
        public async Task<bool> CreateVirtualDir(string name)
        {
            
            try
            {
                CreateVDir("IIS://Localhost/W3SVC/3/Root", name, "C:\\Projects\\Sites\\FromCodeApp");

                string dir = "C:\\Projects\\Sites\\FromCodeApp";
                string indexFile = "C:\\Projects\\Apps\\ChildApps\\index.html";

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (File.Exists(indexFile))
                {
                    FileInfo fileInfo = new FileInfo(indexFile);
                    string fileName = fileInfo.Name;
                    fileInfo.CopyTo(Path.Combine(dir, fileName), true);
                }

                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public static void CreateVDir(string metabasePath, string vDirName, string physicalPath)
        {
            //  metabasePath is of the form "IIS://<servername>/<service>/<siteID>/Root[/<vdir>]"
            //    for example "IIS://localhost/W3SVC/1/Root" 
            //  vDirName is of the form "<name>", for example, "MyNewVDir"
            //  physicalPath is of the form "<drive>:\<path>", for example, "C:\Inetpub\Wwwroot"
            Console.WriteLine("\nCreating virtual directory {0}/{1}, mapping the Root application to {2}:",
                metabasePath, vDirName, physicalPath);

            DirectoryEntry site = new DirectoryEntry(metabasePath);
            string className = site.SchemaClassName.ToString();
            if ((className.EndsWith("Server")) || (className.EndsWith("VirtualDir")))
            {
                DirectoryEntries vdirs = site.Children;
                DirectoryEntry newVDir = vdirs.Add(vDirName, (className.Replace("Service", "VirtualDir")));
                newVDir.Properties["Path"][0] = physicalPath;
                newVDir.Properties["AccessScript"][0] = true;
                // These properties are necessary for an application to be created.
                newVDir.Properties["AppFriendlyName"][0] = vDirName;
                newVDir.Properties["AppIsolated"][0] = "1";
                newVDir.Properties["AppRoot"][0] = "/LM" + metabasePath.Substring(metabasePath.IndexOf("/", ("IIS://".Length)));

                newVDir.CommitChanges();

                Console.WriteLine(" Done.");
            }
            else
                Console.WriteLine(" Failed. A virtual directory can only be created in a site or virtual directory node.");
        }

    }
}

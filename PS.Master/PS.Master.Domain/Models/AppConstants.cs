using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.Domain.Models
{
    public static class AppConstants
    {
        public static class MasterConfigKeys
        {
            public const string ZIP_STAGE_PATH = "ZIP_STAGE_PATH";
        }

        public static class AppHostConstants
        {
            public const string ZipStagePath = "";
            public const string DefaultAppServer = "APP_SERVER_DEFAULT";
            public const string IISWebSiteMetaPath = "IIS://{0}/W3SVC/{1}/Root"; //"IIS://Localhost/W3SVC/3/Root"
            public const string IISWebPath = "IIS://{0}/W3SVC"; //IIS://Localhost/W3SVC
            public const int MasterSiteId = 3;
            public const string MasterSiteProtocols = "http";
            public const string MasterSiteHostName = "Localhost";
            public const string MasterSitePort = "92";
            public const string SiteDefaultPort = "92";
            public const string DefaultAppPool = "DefaultAppPool";
            public const string SecStage = "SecStage";
            public const string DeploySettingsFileName = "deploysettings.json";
        }

        public static class ErrorMessage
        {
            public const string ZipStagePathNotExists = "Zip file stage path not configured in master application";
            public const string AppArtifactsNull = "AppArtifacts object is either null or no files found";
            public const string DefaultServerNotConfigured = "Default app server is not configured in master app";
            public const string vDirCreationFailed = "Failed. A virtual directory can only be created in a site or virtual directory node.";
        }
    }
}

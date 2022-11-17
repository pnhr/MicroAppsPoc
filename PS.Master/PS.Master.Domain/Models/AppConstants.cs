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
            public const string AppDeployPath = "AppDeployPath";
        }

        public static class AppHostConstants
        {
            public const string IISPath = "IIS://{0}/W3SVC/{1}/Root"; //"IIS://Localhost/W3SVC/3/Root"
            public const string SampleIndexHtmlFile = "C:\\Projects\\Apps\\ChildApps\\index.html";
            public const int MasterSiteId = 3;
            public const string MasterSiteProtocols = "http";
            public const string MasterSiteHostName = "Localhost";
            public const string MasterSitePort = "92";
        }
    }
}

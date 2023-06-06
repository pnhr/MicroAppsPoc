using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.ViewModels.Models
{
    public class DeploySettings
    {
        public string AppName { get; set; }
        public string AppDisplayName { get; set; }
        public string LogoFileName { get; set; }
        public bool IsDbNeeded { get; set; }
        public bool IsWinServiceNeeded { get; set; }
        public string DbZipFile { get; set; }
        public string DatabaseName { get; set; }
        public string ConnStr { get; set; }
        public string WebAppZipFile { get; set; }
        public string WinServiceZipFile { get; set; }
    }
}

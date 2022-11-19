using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.ViewModels.Models
{
    public class AppArtifacts
    {
        public AppArtifacts()
        {
            AppFiles = new List<AppFile>();
        }
        public int? AppId { get; set; }
        public string AppName { get; set; }
        public string AppDiscription { get; set; }
        public string? SelectedAppServerCode { get; set; }
        public string? AppZipFileStageFolderPath { get; set; }
        public List<AppFile> AppFiles { get; set; }
    }

}

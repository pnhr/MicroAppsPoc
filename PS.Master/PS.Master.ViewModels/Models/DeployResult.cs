using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.ViewModels.Models
{
    public class DeployResult
    {
        public string AppName { get; set; }
        public string AppDisplayName { get; set; }
        public string? AppDiscription { get; set; }
        public string AppUrl { get; set; }
        public string? AppLogo { get; set; }
    }
}

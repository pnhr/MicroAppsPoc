using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.ViewModels.Models
{
    public class AppArtifacts
    {
        public int AppId { get; set; }
        public string AppName { get; set; }
        public byte[]? Artifacts { get; set; } = default;
    }
}

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.UI.Components
{
    public partial class PsCard
    {
        [Parameter]
        public string ImgSrc { get; set; }
        [Parameter]
        public string Header { get; set; }
        [Parameter]
        public string Discription { get; set; }
    }
}

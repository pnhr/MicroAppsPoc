using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.UI.Components
{
    public partial class PsImage
    {
        [Parameter]
        public string ImgSrc { get; set; }
        [Parameter]
        public string Alt { get; set; } = "This is image";
    }
}

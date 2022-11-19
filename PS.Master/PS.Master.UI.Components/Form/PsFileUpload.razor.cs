using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.UI.Components
{
    public partial class PsFileUpload
    {
        [Parameter] public EventCallback<IBrowserFile> OnFileUpload { get; set; }

        private static string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private string DragClass = DefaultDragClass;
        private List<string> fileNames = new List<string>();

        private async Task OnInputFileChanged(IBrowserFile file)
        {
            ClearDragClass();
            fileNames.Add(file.Name);
            await OnFileUpload.InvokeAsync(file);
        }
        private async Task Clear()
        {
            fileNames.Clear();
            ClearDragClass();
            await Task.Delay(100);
        }
        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
        }
        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }
    }
}

using Microsoft.AspNetCore.Components.Forms;
using PS.Master.UI.Helpers;
using PS.Master.ViewModels.Models;
using System.Net.Http.Headers;

namespace PS.Master.UI.Pages
{
    public partial class DeployApp
    {
        public string appUri = "";
        private bool isProcessing = false;

        public AppArtifacts AppArtifactDetails { get; set; }
        public MultipartFormDataContent FileContent { get; set; }
        protected override Task OnInitializedAsync()
        {
            AppArtifactDetails = new AppArtifacts();
            FileContent = new MultipartFormDataContent();
            return base.OnInitializedAsync();
        }

        private async Task OnFileUpload(IBrowserFile file)
        {
            Logger.LogInformation("OnFileUpload button clicked");

            long maxFileSize = AppConstants.AppConfig.MaxFileUploadSize;

            if (AppArtifactDetails.AppFiles.SingleOrDefault(f => f.FileName == file.Name) is null)
            {
                try
                {
                    var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    AppArtifactDetails.AppFiles.Add(new() { FileName = file.Name, FileExtension = Path.GetExtension(file.Name), FileSize = file.Size });
                    FileContent.Add(content: fileContent, name: "\"files\"", fileName: file.Name);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Something went wrong while uploading file : {ex.ToString()}");
                    throw;
                }
            }
            Logger.LogInformation("OnFileUpload button click ended");
        }

        private async Task OnDeploy()
        {
            try
            {
                Logger.LogInformation("App Deployment Started");
                isProcessing = true;
                AppArtifactDetails.AppZipFileStageFolderPath = await appMgrServiceHandler.UploadAppFile(FileContent, AppArtifactDetails.AppName);

                if(AppArtifactDetails.AppFiles != null && AppArtifactDetails.AppFiles.Count > 0)
                {
                    foreach(var appFile in AppArtifactDetails.AppFiles)
                    {
                        appFile.FilePath = AppArtifactDetails.AppZipFileStageFolderPath;
                    }
                }

                DeployResult deployResult = await appMgrServiceHandler.Deploy(AppArtifactDetails);
                if (!string.IsNullOrWhiteSpace(deployResult.AppUrl))
                    isProcessing = false;
                Logger.LogInformation("App Deployment completed");
            }
            catch(Exception ex)
            {
                Logger.LogError($"Something went wrong while deploying application : {ex.ToString()}");
                throw;
            }
            
        }
    }
}

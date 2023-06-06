using Microsoft.AspNetCore.Components.Forms;
using PS.Master.UI.Helpers;
using PS.Master.ViewModels.Models;
using System.Buffers.Text;
using System.Net.Http.Headers;

namespace PS.Master.UI.Pages
{
    public partial class DeployApp
    {
        public string appUri = "";
        private bool isProcessing = false;
        private string confirmationMessage = "";

        public AppArtifacts AppArtifactDetails { get; set; }
        public MultipartFormDataContent FileContent { get; set; }

        public Dictionary<string, string> AppServerOptions { get; set; }
        protected override Task OnInitializedAsync()
        {
            AppArtifactDetails = new AppArtifacts();
            FileContent = new MultipartFormDataContent();
            AppServerOptions = new Dictionary<string, string>();

            AppServerOptions.Add("APP_SERVER_DEFAULT", "Localhost");
            AppServerOptions.Add("APP_SERVER_ONE", "APP_SERVER_ONE");
            AppServerOptions.Add("APP_SERVER_TWO", "APP_SERVER_TWO");
            AppServerOptions.Add("APP_SERVER_THREE", "APP_SERVER_THREE");

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
                Deploy();
                confirmationMessage = "Deployment request have been submitted. you will see the app on the home screen once it is deployed.";
                await Task.Delay(3000);
                navManager.NavigateTo("/");
            }
            catch(Exception ex)
            {
                isProcessing = false;
                Logger.LogError($"Something went wrong while deploying application : {ex.ToString()}");
                throw;
            }
            
        }

        private async Task<bool> Deploy()
        {
            var task = await Task.Run(async () =>
            {
                isProcessing = true;
                Logger.LogInformation("App Deployment Started");

                AppArtifactDetails.AppZipFileStageFolderPath = await appMgrServiceHandler.UploadAppFile(FileContent, AppArtifactDetails.AppName);

                if (AppArtifactDetails.AppFiles != null && AppArtifactDetails.AppFiles.Count > 0)
                {
                    foreach (var appFile in AppArtifactDetails.AppFiles)
                    {
                        appFile.FilePath = AppArtifactDetails.AppZipFileStageFolderPath;
                    }
                }

                DeployResult deployResult = await appMgrServiceHandler.Deploy(AppArtifactDetails);
                if (!string.IsNullOrWhiteSpace(deployResult.AppUrl))
                {
                    isProcessing = false;
                }
                Logger.LogInformation("App Deployment completed");
                return isProcessing;
            });

            return task;
        }
    }
}

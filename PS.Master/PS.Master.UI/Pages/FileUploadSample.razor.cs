using Microsoft.AspNetCore.Components.Forms;
using static MudBlazor.CategoryTypes;
using System.Net.Http.Json;
using PS.Master.ViewModels.Models;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;
using PS.Master.UI.Helpers;

namespace PS.Master.UI.Pages
{
    public partial class FileUploadSample
    {
        private List<File> files = new();
        private List<SampleUploadResult> uploadResults = new();
        private int maxAllowedFiles = 3;
        private bool shouldRender;

        protected override bool ShouldRender() => shouldRender;

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            shouldRender = false;
            long maxFileSize = AppConstants.AppConfig.MaxFileUploadSize;
            var upload = false;

            using var content = new MultipartFormDataContent();

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                if (uploadResults.SingleOrDefault(f => f.FileName == file.Name) is null)
                {
                    try
                    {
                        var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        files.Add(new() { Name = file.Name });
                        content.Add(content: fileContent, name: "\"files\"", fileName: file.Name);
                        upload = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogInformation("{FileName} not uploaded (Err: 6): {Message}", file.Name, ex.Message);

                        uploadResults.Add(
                            new()
                            {
                                FileName = file.Name,
                                ErrorCode = 6,
                                Uploaded = false
                            });
                    }
                }
            }

            if (upload)
            {
                Http.BaseAddress = new Uri("https://localhost:7263/");
                var response = await Http.PostAsync("/AppFile/uploadfile", content);
                
                var newUploadResults = await response.Content.ReadFromJsonAsync<IList<SampleUploadResult>>();

                if (newUploadResults is not null)
                {
                    uploadResults = uploadResults.Concat(newUploadResults).ToList();
                }
            }
            shouldRender = true;
        }

        private static bool FileUpload(IList<SampleUploadResult> uploadResults,
            string? fileName, ILogger<FileUploadSample> logger, out SampleUploadResult result)
        {
            result = uploadResults.SingleOrDefault(f => f.FileName == fileName) ?? new();

            if (!result.Uploaded)
            {
                logger.LogInformation("{FileName} not uploaded (Err: 5)", fileName);
                result.ErrorCode = 5;
            }

            return result.Uploaded;
        }

        private class File
        {
            public string? Name { get; set; }
        }
    }
}

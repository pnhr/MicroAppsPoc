namespace PS.Master.Api.Services.Interfaces
{
    public interface IAppFileService
    {
        Task<string> StageAppFile(IFormFile file, string appName);
        Task UnZipFile(string filePath, string extractPath);
    }
}

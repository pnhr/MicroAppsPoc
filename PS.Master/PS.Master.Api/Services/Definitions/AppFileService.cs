using AutoMapper;
using PS.Master.Api.Services.Interfaces;
using PS.Master.Data;
using PS.Master.Domain.DbModels;
using PS.Master.Domain.Models;
using PS.Master.ViewModels.Models;
using System.IO;

namespace PS.Master.Api.Services.Definitions
{
    public class AppFileService : IAppFileService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<SampleService> _logger;
        private readonly IConfigRepository _configRepo;

        public AppFileService(IMapper mapper, ILogger<SampleService> _logger, IConfigRepository configRepo)
        {
            this._mapper = mapper;
            this._logger = _logger;
            this._configRepo = configRepo;
        }

        public async Task<string> StageAppFile(IFormFile file, string appName)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            MasterConfig zipStagePathConfig = await _configRepo.GetConfig(AppConstants.MasterConfigKeys.ZIP_STAGE_PATH);

            if (zipStagePathConfig == null)
                throw new Exception(AppConstants.ErrorMessage.ZipStagePathNotExists);

            string stageFolder = Path.Combine(zipStagePathConfig.Value, appName);

            if (!Directory.Exists(stageFolder))
                Directory.CreateDirectory(stageFolder);

            string path = Path.Combine(stageFolder, file.FileName);

            if (File.Exists(path))
                File.Delete(path);

            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            return stageFolder;
        }
        public async Task UnZipAllFiles(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.zip", SearchOption.TopDirectoryOnly);

            List<string> fileNames = new List<string>();

            foreach (var file in files)
            {
                await UnZipFile(file, folderPath);
                fileNames.Add(file);
            }

            foreach(string file in files)
            {
                File.Delete(file);
            }
        }
         
        public async Task UnZipFile(string filePath, string extractPath)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(filePath, extractPath);
            await Task.FromResult(0);
        }
    }
}

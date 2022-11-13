using AutoMapper;
using PS.Master.Api.Services.Interfaces;
using PS.Master.Data;
using PS.Master.ViewModels.Models;

namespace PS.Master.Api.Services.Definitions
{
    public class SampleService : ISampleService
    {
        private readonly ISampleRepository sampleRepository;
        private readonly IMapper mapper;
        private readonly ILogger<SampleService> logger;

        public SampleService(ISampleRepository sampleRepository, IMapper mapper, ILogger<SampleService> logger)
        {
            this.sampleRepository = sampleRepository;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task<List<UserVM>> GetUsers()
        {
            List<UserVM> users = new List<UserVM>();
            var empList = await sampleRepository.GetEmployeesAsync();
            mapper.Map(empList, users);
            return users;
        }
    }
}

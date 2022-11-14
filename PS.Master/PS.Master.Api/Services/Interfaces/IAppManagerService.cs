namespace PS.Master.Api.Services.Interfaces
{
    public interface IAppManagerService
    {
        Task<bool> CreateVirtualDir(string name);
    }
}

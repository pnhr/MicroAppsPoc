using System.ServiceProcess;

namespace PS.Master.WinServices
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var service = new FileWriteService())
            {
                ServiceBase.Run(service);
                //service.OnDebug();
            }
        }
    }
}
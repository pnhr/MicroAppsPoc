using System.ServiceProcess;
using System.Threading;

namespace PS.Master.WinServices
{
    public class FileWriteService : ServiceBase
    {
        public Thread Worker = null;
        public FileWriteService()
        {
            ServiceName = "PsLogService";
        }

        protected override void OnStart(string[] args)
        {
            ThreadStart start = new ThreadStart(Working);
            Worker = new Thread(start);
            Worker.Start();
        }

        public void Working()
        {
            int nsleep = 1; //interval as 1 minute
            while (true)
            {
                string FileName = @"E:\NewLaptop\PocProjects\dotnet\core\MASTER_APP_POC\WinServices\CoreWinServiceLog.txt";
                using(StreamWriter sw = new StreamWriter(FileName, true))
                {
                    sw.WriteLine($"Windows Service called on {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}");
                    sw.Close();
                }
                Thread.Sleep(nsleep * 60 * 1000);
            }
        }
        protected override void OnStop()
        {
            try
            {
                if ((Worker != null) & Worker.IsAlive)
                {
                    string FileName = @"E:\NewLaptop\PocProjects\dotnet\core\MASTER_APP_POC\WinServices\CoreWinServiceLog.txt";
                    using (StreamWriter sw = new StreamWriter(FileName, true))
                    {
                        sw.WriteLine($"Windows Service stopped on {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}");
                        sw.Close();
                    }
                    Worker.Abort();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void OnDebug()
        {
            OnStart(null);
        }
    }
}

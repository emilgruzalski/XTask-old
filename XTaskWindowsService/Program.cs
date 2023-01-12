using System.ServiceProcess;

namespace XTaskWindowsService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new XTaskWindowsService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

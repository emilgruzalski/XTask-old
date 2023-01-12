using System.ServiceModel;
using System.ServiceProcess;

namespace XTaskWindowsService
{
    public partial class XTaskWindowsService : ServiceBase
    {
        public ServiceHost serviceHost = null;

        public XTaskWindowsService()
        {
            InitializeComponent();
        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            serviceHost?.Close();

            // Create a ServiceHost for the XTaskService type and
            // provide the base address.
            serviceHost = new ServiceHost(typeof(XTaskService));

            // Open the ServiceHostBase to create listeners and start
            // listening for messages.
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }
}

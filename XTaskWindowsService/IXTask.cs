using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace XTaskWindowsService
{
    [ServiceContract(Namespace = "http://XTaskService")]
    public interface IXTask
    {
        [OperationContract]
        void InsertLog(string process, string action);
    }
}

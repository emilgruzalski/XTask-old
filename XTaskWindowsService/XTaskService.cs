using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTaskWindowsService
{
    public class XTaskService : IXTask
    {
        public void InsertLog(string process, string action)
        {
            XTaskModelContainer container = new XTaskModelContainer();
            var log = new Log()
            {
                Process = process,
                Action = action,
                Date = DateTime.Now
            };
            container.LogSet.Add(log);
            container.SaveChanges();
        }
    }
}

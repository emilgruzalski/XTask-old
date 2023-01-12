using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTaskWpfApp
{
    public class ExpandedProcess
    {
        public int Id => Process.Id;
        public string ProcessName => Process.ProcessName;
        public Process Process { get; }

        private long _Memory = 0;
        private bool? _IsResponding = null;

        public long Memory
        {
            set
            {
                if (_Memory != value)
                {
                    _Memory = value;
                }
            }
        }

        public string MemoryInMB
        {
            get
            {
                string ret = "n/a";
                decimal value = 0;

                if (_Memory > 0)
                {
                    value = Convert.ToDecimal(_Memory) / 1024 / 1024;
                    ret = value.ToString("###,###,##0.0") + " MB";
                }

                return ret;
            }
        }

        public bool? IsResponding
        {
            get { return _IsResponding; }
            set
            {
                if (_IsResponding != value)
                {
                    _IsResponding = value;
                }
            }
        }

        public override string ToString()
        {
            return ProcessName + " (" + MemoryInMB + ")";
        }

        public ExpandedProcess(Process process, long memory, bool isResponding) 
        {
            Process= process;
            Memory= memory;
            IsResponding= isResponding;
        }
    }
}

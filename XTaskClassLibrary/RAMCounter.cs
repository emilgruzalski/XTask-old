﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTaskClassLibrary
{
    public class RAMCounter
    {
        private const string PERF_COUNTER_MEMORY = "Working Set - Private";
        public virtual long GetMemory(Process prc)
        {
            PerformanceCounter pc = null;
            long ret = -1;

            try
            {
                pc = new PerformanceCounter("Process", PERF_COUNTER_MEMORY, prc.ProcessName);

                if (pc != null)
                    ret = pc.RawValue;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (pc != null)
                {
                    pc.Close();
                    pc.Dispose();
                }
            }
            return ret;
        }

        public virtual Task<long> GetMemoryTask(Process prc)
        {
            Task<long> @long = new Task<long>(() => GetMemory(prc));
            @long.Start();
            return @long;
        }
    }
}

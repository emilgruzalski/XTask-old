using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace XTaskWpfApp
{
    public class ViewModel : INotifyPropertyChanged
    {
        private const string PERF_COUNTER_MEMORY = "Working Set - Private";
        private DispatcherTimer aTimer;

        public ViewModel() 
        {
            aTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            aTimer.Tick += UpdateProcesses;
            aTimer.IsEnabled = true;
        }

        private Process _selectedProcess;
        public Process SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ExpandedProcess> Processes { get; } = new ObservableCollection<ExpandedProcess>();

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void UpdateProcesses(object sender, EventArgs e)
        {
            aTimer.IsEnabled = false;
            List<int> currentIds = Processes.Select(p => p.Id).ToList();
            Process[] processes = Process.GetProcesses();
            List<int> deletedIds = new List<int>();

            foreach (int pid in currentIds) // delete nonexistent processes
            {
                if (!processes.Any(p => p.Id == pid))
                {
                    ExpandedProcess process = Processes.First(p => p.Id == pid);
                    Processes.Remove(process);
                    deletedIds.Add(pid);
                }
            }

            foreach (int pid in deletedIds)
            {
                currentIds.Remove(pid);
            }

            foreach (Process p in processes) // add new processes
            {
                if (!currentIds.Remove(p.Id))
                {
                    ExpandedProcess expandedProcess = new ExpandedProcess(p, GetMemory(p), p.Responding);
                    Processes.Add(expandedProcess);
                }
            }

            foreach (Process p in processes) // update old processes
            {
                if (currentIds.Remove(p.Id))
                {
                    Processes.First(cp => cp.Id == p.Id).Memory = await GetMemoryTask(p);
                    Processes.First(cp => cp.Id == p.Id).IsResponding = p.Responding;
                }
            }

            aTimer.IsEnabled = true;
        }

        protected virtual long GetMemory(Process prc)
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

        protected virtual Task<long> GetMemoryTask(Process prc)
        {
            Task<long> @long = new Task<long>(() => GetMemory(prc));
            @long.Start();
            return @long;
        }
    }
}

using JetBrains.Annotations;
using Microsoft.Win32;
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
using XTaskClassLibrary;

namespace XTaskWpfApp
{
    public class ViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer aTimer;
        private RAMCounter aCounter = new RAMCounter();

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
                    ExpandedProcess expandedProcess = new ExpandedProcess(p, aCounter.GetMemory(p), p.Responding);
                    Processes.Add(expandedProcess);
                }
            }

            foreach (Process p in processes) // update old processes
            {
                if (currentIds.Remove(p.Id))
                {
                    
                    Processes.First(cp => cp.Id == p.Id).Memory = await aCounter.GetMemoryTask(p);
                    Processes.First(cp => cp.Id == p.Id).IsResponding = p.Responding;
                }
            }

            aTimer.IsEnabled = true;
        }

        public void ChangePriority(ProcessPriorityClass priority)
        {
            SelectedProcess.PriorityClass = priority;
        }

        public void KillProcess()
        {
            XTaskServiceReference.XTaskClient client = new XTaskServiceReference.XTaskClient();
            client.InsertLogAsync(SelectedProcess.ProcessName, "Kill");
            SelectedProcess.Kill();
        }

        public void OpenProcess()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Browse Application Files",
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "exe files (*.exe)|*.exe",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                XTaskServiceReference.XTaskClient client = new XTaskServiceReference.XTaskClient();
                client.InsertLogAsync(openFileDialog.SafeFileName, "Open");
                Process.Start(openFileDialog.FileName);
            }
        }
    }
}

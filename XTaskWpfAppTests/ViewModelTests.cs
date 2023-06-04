using Microsoft.VisualStudio.TestTools.UnitTesting;
using XTaskWpfApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace XTaskWpfApp.Tests
{
    [TestClass()]
    public class ViewModelTests
    {
        [TestInitialize()]
        public void Init() 
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["testFilePath"];

            File.Delete(path);
        }

        [TestMethod()]
        public void OpenProcessTest()
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["testFilePath"];

            Assert.ThrowsException<Win32Exception>(() => Process.Start(path), "Nie można odnaleźć określonego pliku");
        }

        [TestMethod()]
        public void KillProcessTest() 
        {
            Assert.ThrowsException<ArgumentException>(() => Process.GetProcessById(9999).Kill());
        }

        [TestMethod()]
        public void Test()
        {
            int expectedpid = 0;
            Process[] processeslist = Process.GetProcessesByName("idle");
            int actualpid = processeslist[0].Id;

            Assert.AreEqual(expectedpid, actualpid, "Podane procesy są różne");
        }

        [TestMethod()]
        public void Test1()
        {
            Assert.IsInstanceOfType(Process.GetCurrentProcess(), typeof(Process), "Obiekt nie jest klasy Process");
        }

        [TestCleanup()]
        public void Cleanup()
        {
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Application = TestStack.White.Application;

namespace DynamicConfig.Test.UI
{
    [TestClass()]
    public class TestDynamicConfigUI
    {
        private ProcessStartInfo config_process;

        private Application app;
        
        [TestInitialize()]
        public void Setup()
        {
            //Find exe
            config_process = new ProcessStartInfo(
                @"C:\Code\C#\DynamicConfig\DynamicCOnfig.ServiceExample\bin\Release\DynamicCOnfig.ServiceExample.exe"
                );
            app = Application.AttachOrLaunch(config_process);
        }

        [TestCleanup()]
        public void TearDown()
        {
            
        }

        [TestMethod()]
        public void AssertTitle()
        {
            Assert.AreEqual(true, true);
        }
        
    }
}

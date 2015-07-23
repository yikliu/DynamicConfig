using System;
using System.ComponentModel;
using System.IO;
using DynamicConfig.ConfigTray;
using DynamicConfig.ConfigTray.JSONConfig;
using DynamicConfig.ConfigTray.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicConfig.Test.DeveloperView
{
    [TestClass()]
    public class TestConfigDaemonDynamicObject
    {
        [TestInitialize]
        public void SetUp()
        {
            //restore config.json content before each test
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string src = Path.Combine(path, "config-backup.json");
            string dest = Path.Combine(path, "config.json");
            File.Copy(src, dest, true);

            ConfigDaemon.LoadConfig();
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        [TestMethod]
        public void ReadPlainLeafNode()
        {
            string name = ConfigDaemon.Root.ServiceName;
            Assert.AreEqual("172.19.133.73", name);

            int numberSecondsToRefresh = ConfigDaemon.Root.NumberSecondsToRefresh;
            Assert.AreEqual(15, numberSecondsToRefresh);

            //also can be read as string
            string numberSecondsToRefreshInString = ConfigDaemon.Root.NumberSecondsToRefresh;
            Assert.AreEqual("15", numberSecondsToRefreshInString);

            bool isFun = ConfigDaemon.Root.IsFun;
            Assert.AreEqual(false, isFun);

            //also can be read as string
            string isFunInString = ConfigDaemon.Root.IsFun;
            Assert.AreEqual("false", isFunInString, true);
        }

        [TestMethod]
        public void RaiseEventWhenWritten()
        {
            ConfigLeafNode nameNode = ConfigDaemon.Root.ServiceName;
            Assert.AreEqual("172.19.133.73", nameNode);

            var newNameNode = (string) nameNode;
            nameNode.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                newNameNode = ConfigDaemon.Root.ServiceName;
            };

            ConfigDaemon.Root.ServiceName = "NewServiceName";
            

        }

        [TestMethod]
        public void ReadArray()
        {
            dynamic icservers = ConfigDaemon.Root.ICServers;
            Assert.IsNotNull(icservers);
            Assert.IsInstanceOfType(icservers, typeof(ConfigListNode));
        }

        [TestMethod]
        public void ReadEncryptedLeafNode()
        {
            try
            {
                string password = ConfigDaemon.Root.Password; 
                Assert.AreEqual("1231431", password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [TestMethod]
        public void WritePlainLeafNode()
        {
            ConfigDaemon.Root.Username = "yikliu";
            string name = ConfigDaemon.Root.Username;
            Assert.AreEqual("yikliu", name);

            ConfigDaemon.Root.simpleArray[2] = "4";
            string four = ConfigDaemon.Root.simpleArray[2];
            Assert.AreEqual("4", four);

            ConfigDaemon.Root.DoubleTest = 10.00;
            int ten = ConfigDaemon.Root.DoubleTest;
            Assert.AreEqual(10, ten);

            ConfigDaemon.Root.IsFun = true;
            bool is_fun = ConfigDaemon.Root.IsFun;
            Assert.AreEqual(true, is_fun);

            ConfigDaemon.Root.ICServers[0].endpoint[0].name = "newName";
            string newName = ConfigDaemon.Root.ICServers[0].endpoint[0].name;
            Assert.AreEqual(newName, "newName");
        }

        [TestMethod]
        public void WriteEncryptedNode()
        {
            ConfigDaemon.Root.Password = "hi";
            string pw = ConfigDaemon.Root.Password;
            Assert.AreEqual("hi", pw);
        }

        [TestMethod]
        public void ReadNonExistingNode()
        {
            var notExistKvp = ConfigDaemon.Root.Foo.Bar; //access non-existent node with key
            var notExistedArrayElement = ConfigDaemon.Root[0]; //access non-existent node with index
            var deepButStillReturnsNep = ConfigDaemon.Root.Foo.Bar.Foo1.Bar1.Foo2.bar2; //deep in hierarchy

            string nepInString = (string)deepButStillReturnsNep; // converts to ""
            int nepInInteger = (int)deepButStillReturnsNep; //convert to 0

            Console.WriteLine(notExistKvp.GetType().Name); //output "NullExceptionPreventer"
            Console.WriteLine(notExistedArrayElement.GetType().Name);//output "NullExceptionPreventer"
            Console.WriteLine(deepButStillReturnsNep.GetType().Name);//output "NullExceptionPreventer"

            Console.WriteLine(nepInString);  //output ""
            Console.WriteLine(nepInInteger); //output "0"
            Console.WriteLine(nepInInteger.GetType().Name); //output "Int32"

            Assert.IsInstanceOfType(notExistKvp, typeof(NullExceptionPreventer));
            Assert.IsInstanceOfType(notExistedArrayElement, typeof(NullExceptionPreventer));
            Assert.IsInstanceOfType(deepButStillReturnsNep, typeof(NullExceptionPreventer));
        }

        [TestMethod]
        public void InsertNewProperty()
        {
            ConfigDaemon.Root.Foo = "bar"; //setting new property to existing node is OK
            string foo = ConfigDaemon.Root.Foo;
            Assert.AreEqual("bar", foo);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void InsertDeepNewProperty()
        {
            ConfigDaemon.Root.Foo.Bar = "bar"; //setting new property to an NullExceptionPreventor throws exception
            string foo = ConfigDaemon.Root.Foo.Bar;
            Assert.AreEqual("bar", foo);
        }

        [TestMethod]
        public void ReadArrayNodeInObjectNode()
        {
            object n = ConfigDaemon.Root.ServiceName[0]; //getting an array element from a object node returns NullExceptionPreventer
            Assert.IsInstanceOfType(n, typeof(NullExceptionPreventer));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void InsertArrayNodeToObjectNode()
        {
            //ServiceName is a object, setting index to it is disallowed.
            ConfigDaemon.Root.ServiceName[0] = "this won't work";
        }

        [TestMethod]
        public void ReadObjectNodeInArrayNode()
        {
            object n = ConfigDaemon.Root.ICServers.BadProperty;
            Assert.IsInstanceOfType(n, typeof(NullExceptionPreventer));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void InsertObjectNodeToArrayNode()
        {
            //ICServers is an array, setting <key,value> is disallowed
            ConfigDaemon.Root.ICServers.BadProperty = "this won't work";
        }
    }
}
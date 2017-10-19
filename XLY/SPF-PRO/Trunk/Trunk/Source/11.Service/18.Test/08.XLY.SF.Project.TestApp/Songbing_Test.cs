// ***********************************************************************
// Assembly:XLY.SF.Project.TestApp
// Author:Songbing
// Created:2017-06-27 16:37:45
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Devices;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Services;
using XLY.SF.Project.Plugin.Android;
using XLY.SF.Project.Plugin.IOS;
using XLY.SF.Project.Persistable.Primitive;

namespace XLY.SF.Project.TestApp
{
    public class Songbing_Test
    {

        #region IDataItems测试
        //#region  IDataItems测试相关类

        //[Serializable]
        //public class Person : AbstractDataItem
        //{
        //    public Person(string name, int age)
        //    {
        //        Name = name;
        //        Age = age;
        //    }

        //    [Display(DataType = DisplayDataType.TEXT)]
        //    public string Name { get; set; }

        //    [Display(DataType = DisplayDataType.INTEGER)]
        //    public int Age { get; set; }

        //    [Display(DataType = DisplayDataType.TEXT)]
        //    public string Location { get; set; }

        //    [Display(DataType = DisplayDataType.INTEGER)]
        //    public int Age2 { get; set; }

        //    [Display(DataType = DisplayDataType.INTEGER)]
        //    public int Age3 { get; set; }

        //    [Display(DataType = DisplayDataType.INTEGER)]
        //    public int Age4 { get; set; }

        //    [Display(DataType = DisplayDataType.INTEGER)]
        //    public int Age5 { get; set; }

        //    [Display(DataType = DisplayDataType.INTEGER)]
        //    public int Age6 { get; set; }
        //}

        //[Serializable]
        //public class Dog : AbstractDataItem
        //{
        //    public Dog(string name, int age)
        //    {
        //        Name = name;
        //        Age = age;
        //    }

        //    [Display(Key = "xxxx", DataType = DisplayDataType.TEXT)]
        //    public string Name { get; set; }

        //    [Display(DataType = DisplayDataType.INTEGER)]
        //    public int Age { get; set; }
        //}

        //#endregion
        //public static void Test(string[] args)
        //{
        //    //TestDataItems();

        //    var dsA = Serializer.DeSerializeFromBinary<IDataSource>(@"D:\test\1.bin") as SimpleDataSource;
        //    var diA = dsA.Items as DataItems<Person>;

        //    //var dsB = DeSerializeFromBinary<IDataSource>(@"D:\test\2.bin") as TreeDataSource;
        //    //var diB = dsB.TreeNodes[0].Items as DataItems<Person>;

        //    foreach (Person s in diA)
        //    {

        //    }

        //    //foreach (Person s in diB)
        //    //{

        //    //}

        //    //var res = diA.FirstOrDefault(s => s.Name == "新华公告111");

        //    //var ls = diA.Skip(100).Take(10).ToList();

        //    //var count = diA.Count();
        //}

        //public static void TestDataItems()
        //{
        //    foreach (var ff in Directory.GetFiles(@"D:\test\"))
        //    {
        //        File.Delete(ff);
        //    }

        //    Stopwatch st = new Stopwatch();
        //    st.Start();

        //    int total = 200000;

        //    var dbfilePath = @"D:\test\test1.db";

        //    var t1 = Task.Run(() =>
        //    {
        //        SimpleDataSource ds = new SimpleDataSource();
        //        ds.Items = new DataItems<Person>(dbfilePath);

        //        int count = total;
        //        while (count > 0)
        //        {
        //            ds.Items.Add(new Person("新华公告", 11) { Location = "四川省成都市高新区天府大道9999号" });

        //            count--;
        //        }

        //        ds.Items.Add(new Person("新华公告111", 11) { Location = "四川省成都市高新区天府大道9999号" });

        //        ds.BuildParent();

        //        Serializer.SerializeToBinary(ds, @"D:\test\1.bin");

        //        Console.WriteLine("1 OK!");
        //    });

        //    var t2 = Task.Run(() =>
        //    {
        //        TreeDataSource ds = new TreeDataSource();
        //        TreeNode node = new TreeNode()
        //        {
        //            Items = new DataItems<Person>(dbfilePath)
        //        };
        //        ds.TreeNodes.Add(node);

        //        int count = total;
        //        while (count > 0)
        //        {
        //            node.Items.Add(new Person("花样区域", 11) { Location = "北京市海淀区人民广场888号" });

        //            count--;
        //        }

        //        ds.BuildParent();

        //        Serializer.SerializeToBinary(ds, @"D:\test\2.bin");

        //        Console.WriteLine("2 OK!");
        //    });

        //    var t3 = Task.Run(() =>
        //    {
        //        TreeDataSource ds = new TreeDataSource();
        //        TreeNode node = new TreeNode()
        //        {
        //            Items = new DataItems<Dog>(dbfilePath)
        //        };
        //        ds.TreeNodes.Add(node);

        //        int count = total;
        //        while (count > 0)
        //        {
        //            node.Items.Add(new Dog("cccccc", 33));

        //            count--;
        //        }

        //        ds.BuildParent();

        //        Serializer.SerializeToBinary(ds, @"D:\test\3.bin");

        //        Console.WriteLine("3 OK!");
        //    });

        //    Task.WaitAll(t1, t2, t3);

        //    st.Stop();

        //    Console.WriteLine("ALL OK!");
        //    Console.WriteLine(st.ElapsedMilliseconds / 1000.0);
        //}

        #endregion

        #region 安卓微信数据库解密测试

        public void WXDecryptedDllTest()
        {
            string sourceFile = @"I:\本地数据\微信数据没平航多1\com.tencent.mm\MicroMsg\b7d12bb40bcdfb5c682fb6cff1e69b34\EnMicroMsg.db";

            IntPtr wxHandle = IntPtr.Zero;

            // 打开数据库文件，获得文件句柄
            wxHandle = WXDeCryptedCoreDll.WXOpen(sourceFile);

            //WXDeCryptedCoreDll.WXDeCryptedDBToFile(wxHandle, @"I:\本地数据\微信数据没平航多1\com.tencent.mm\MicroMsg\b7d12bb40bcdfb5c682fb6cff1e69b34\test.db");
            WXDeCryptedCoreDll.WXDeCryptedDBToFileBakdata(wxHandle, @"I:\本地数据\微信数据没平航多1\com.tencent.mm\MicroMsg\b7d12bb40bcdfb5c682fb6cff1e69b34\test.db", pHandel =>
            {
                WxbakMsgdata data = (WxbakMsgdata)Marshal.PtrToStructure(pHandel, typeof(WxbakMsgdata));

                var ss1 = GetUnMomString(data.wxid, data.wxidByteLen);
                var ss2 = GetUnMomString(data.pmsg, data.msgbytelen);

                return 0;
            });

            WXDeCryptedCoreDll.CloseHanle(ref wxHandle);
        }

        private static string GetUnMomString(IntPtr pData, int length, Encoding encoding = null)
        {
            if (pData == IntPtr.Zero || length == 0)
            {
                return string.Empty;
            }

            if (null == encoding)
            {
                encoding = Encoding.UTF8;
            }

            byte[] arr = new byte[length];

            Marshal.Copy(pData, arr, 0, length);

            return encoding.GetString(arr);
        }

        #endregion

        #region IOS设备相关接口测试

        private IOSDeviceMonitor Imd = new IOSDeviceMonitor();

        public void IOSDeviceDllTest()
        {
            Imd.DeviceConnected = OnIOSDeviceConn;
            Imd.DeviceDisconnected = OnIOSDeviceDisConn;

            Imd.Start();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public void AnalyzeItunesBackupDATATest()
        {
            var rr1 = IOSDeviceCoreDll.AnalyzeItunesBackupDATA(@"H:\IOS加密备份\029fa1285bb67570265fc68f148d90084b81b48a", @"D:\111", (fn, no, all) =>
            {
                Console.WriteLine("{0} {1} {2}", fn, no, all);
                return 0;
            });

            var rr2 = IOSDeviceCoreDll.AnalyzeItunesBackupDATAPWD(@"H:\IOS加密备份\029fa1285bb67570265fc68f148d90084b81b48a", @"D:\111", (fn, no, all) =>
            {
                Console.WriteLine("{0} {1} {2}", fn, no, all);
                return 0;
            }, (b) =>
            {
                var password = "774933";

                var pS = Marshal.StringToHGlobalAnsi(password);

                Marshal.WriteIntPtr(b, pS);

                return 0;
            });
        }

        private void OnIOSDeviceConn(IDevice idevice)
        {
            var device = idevice as Device;

            var idm = device.DeviceManager as IOSDeviceManager;

            //var res1 = device.IsValid();
            //var res2 = device.GetProperties();
            //var res3 = device.FindInstalledApp();

            //idm.CopyUserData(device, @"D:\111", new DefaultAsyncProgress());
            idm.CopyUserData(device, @"D:\111", new DefaultAsyncProgress(), () => "12345678");
        }

        private void OnIOSDeviceDisConn(IDevice idevice)
        {

        }

        #endregion

        #region Sqlite相关接口测试

        public void SqliteRecoveryHelperTest1()
        {
            SqliteRecoveryHelper.DataRecovery(@"D:\111\test.db", "", "rcontact,message,userinfo2", "", "");
        }

        public void SqliteRecoveryHelperTest2()
        {
            SqliteRecoveryHelper.DataRecovery(@"D:\111\test1.db", "", "rcontact,message,userinfo2", "", "");
        }

        public void SqliteRecoveryHelperTest3()
        {
            int a = 0, b = 0;

            ThreadPool.QueueUserWorkItem((o) =>
            {
                SqliteRecoveryHelper.DataRecovery(@"D:\111\test.db", "", "rcontact,message,userinfo2", "", "");
                a = 1;
            });

            ThreadPool.QueueUserWorkItem((o) =>
            {
                SqliteRecoveryHelper.DataRecovery(@"D:\111\test1.db", "", "rcontact,message,userinfo2", "", "");
                b = 1;
            });

            while (a != 1 || b != 1)
            {
                Thread.Sleep(1000);
            }
        }

        #endregion

        #region 安卓镜像文件扫描接口测试

        public void FileServiceTest()
        {
            FileService fs = new FileService();

            IFileSystemDevice device = new MirrorDevice();
            device.Source = @"G:\镜像\杨燕\kyleplusctc_84893fe9_all storage_20170606145240.bin";
            device.ScanModel = 0x87;

            fs.GetFileSystem(device, new DefaultAsyncProgress());

            string path = "/data/data/com.android.providers.contacts/databases/#F".TrimEnd("#F");
            path = path.Replace("/", @"\");
            if (path.StartsWith(@"\data"))
            {
                path = path.TrimStart(@"\data");
            }
            else if (path.StartsWith(@"\system"))
            {
                path = path.TrimStart(@"\system");
            }

            fs.ExportAppFile(path, @"D:\111");

            KeyValueItem ki = new KeyValueItem("Image", "jpg;jpeg;bmp;png;exif;dxf;pcx;fpx;ufo;tiff;svg;eps;gif;psd;ai;cdr;tga;pcd;hdri;map");

            fs.ExportMediaFile(ki, @"D:\111", ';');

        }

        #endregion

        #region 安卓手机监控 SPF自带命令测试 ADB文件备份命令测试

        public void TestAndriod()
        {
            AndroidDeviceMonitor adm = new AndroidDeviceMonitor();
            adm.DeviceConnected = OnConn;
            adm.Start();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private void OnConn(IDevice idevice)
        {
            var device = idevice as Device;

            AndroidHelper.Instance.BackupAndResolve(device, @"D:\test\111.rar");

            var res = AndroidHelper.Instance.InstallPackage(@"C:\songbingcode\SFProject-new\Branches\3.60.0.Sprint\Source\21-Build\toolkit\app\SPFSocket.apk", device);

            var basicInfo = AndroidHelper.Instance.ExecuteSPFAppCommand(device, "basic_info");

            var appInfo = AndroidHelper.Instance.ExecuteSPFAppCommand(device, "app_info");

            var smsInfo = AndroidHelper.Instance.ExecuteSPFAppCommand(device, "sms_info");

            var contactInfo = AndroidHelper.Instance.ExecuteSPFAppCommand(device, "contact_info");

            var calllogInfo = AndroidHelper.Instance.ExecuteSPFAppCommand(device, "calllog_info");

        }

        #endregion

        #region SD卡监控测试

        public void SDCardDeviceMonitorTest()
        {
            SDCardDeviceMonitor sddm = new SDCardDeviceMonitor();
            sddm.Start();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        #endregion

        #region 安卓QQ数据解析插件测试

        public void TestAndroidQQDataParse()
        {
            AndroidQQDataParseCoreV1_0 parser = new AndroidQQDataParseCoreV1_0(@"D:\111\222\data.db",
                                                          "安卓QQ",
                                                          @"H:\客服数据\张茂吉 QQ解密失败\com.tencent.mobileqq",
                                                          @"C:\XLYSFTasks\任务-2017-09-21-14-37-59\source\media\0\Tencent\MobileQQ");

            parser.BiuldTree();

        }

        #endregion

        #region IOS微信数据解析测试

        public void TestIOSWechatDataParse()
        {
            IOSWeChatDataParseCoreV1_0 parser = new IOSWeChatDataParseCoreV1_0(@"D:\111\222\data.db",
                                                          "IOS微信",
                                                          @"H:\MyIphone\20170720145042\com.tencent.xin");

            parser.BiuldTree();

        }

        #endregion

    }
}

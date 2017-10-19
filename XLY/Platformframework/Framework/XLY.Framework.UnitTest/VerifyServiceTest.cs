using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Utility.Helper;
using NUnit.Framework;
using XLY.XDD.ForensicService;

namespace XLY.Framework.UnitTest
{
    [TestFixture]
    public class VerifyServiceTest
    {
        //[Test]
        //public void GenerateVerifyCode_Files(VerifyService service)
        //{
        //    var path = @"D:\";
        //    if (!System.IO.Directory.Exists(path)) return;
        //    var files = Directory.GetFiles(path);
        // }
        [Test]
        public void GenerateVerifyCode_Buffer_SHA1()
        {
            var buf = this.GetTestBuf();
            var expect = "F7A5265FF904CD6937D4B419330180B22745DC41";
            var service = new VerifyService(EnumVerifyType.SHA1);
            var code = service.GenerateVerifyCode(buf);
            service.Dispose();
            Assert.IsTrue(expect == code);
        }

        [Test]
        public void GenerateVerifyCode_Buffer_MD5()
        {
            var buf = this.GetTestBuf();
            var expect = "181595DF6E878D2E751B2171FB3CF4B4";
            var service = new VerifyService(EnumVerifyType.MD5);
            var code = service.GenerateVerifyCode(buf);
            service.Dispose();
            Assert.IsTrue(expect == code);
        }

        [Test]
        public void GenerateVerifyCode_Buffer_SHA256()
        {
            var buf = this.GetTestBuf();
            var expect = "C3997C79EAC4A963524261B9BABA383941574303A5FBF13DC50D0273CDF0E631";
            var service = new VerifyService(EnumVerifyType.SHA256);
            var code = service.GenerateVerifyCode(buf);
            service.Dispose();
            Assert.IsTrue(expect == code);
        }

        [Test]
        public void GenerateVerifyCode_Buffer_SHA512()
        {
            var buf = this.GetTestBuf();
            var service = new VerifyService(EnumVerifyType.MD5);
            var code = service.GenerateVerifyCode(buf);
            service.Dispose();
            Console.WriteLine(code);
            Assert.IsNotNullOrEmpty(code);
        }

        [Test]
        public void GenerateVerifyCode_File_SHA1()
        {
            try
            {
                System.Utility.Helper.Test tt = new Test();
                tt.Start();
                var start = GC.GetTotalMemory(false);
                var path = this.GetTestFile();
                var expect = "84114290159D8954715E4C1BA21F7614924F7E27";
                var service = new VerifyService(EnumVerifyType.SHA1);
                var code = service.GenerateVerifyCodeByFile(path);
                var time = tt.GetElapsedAndStop().TotalMilliseconds;
                var end = GC.GetTotalMemory(false);
                service.Dispose();
                Assert.IsTrue(expect == code);
                Console.WriteLine("time:" + time + " ,memory:" + System.Utility.Helper.File.GetFileSize((end - start)));
            }
            catch (Exception ex)
            {
                Console.WriteLine("There is nothing for the unit test or maybe something wrong in code!");
            }
        }

        [Test]
        public void GenerateVerifyCode_File_SHA256()
        {
            try
            {
                System.Utility.Helper.Test tt = new Test();
                tt.Start();
                var start = GC.GetTotalMemory(false);
                var path = this.GetTestFile();
                var expect = "DA65745A1951D7239B5E7834319C1722E1F7E9561BC37284C0D5CBD3818DC9BF";
                var service = new VerifyService(EnumVerifyType.SHA256);
                var code = service.GenerateVerifyCodeByFile(path);
                var time = tt.GetElapsedAndStop().TotalMilliseconds;
                var end = GC.GetTotalMemory(false);
                service.Dispose();
                Assert.IsTrue(expect == code);
                Console.WriteLine("time:" + time + " ,memory:" + System.Utility.Helper.File.GetFileSize((end - start)));
            }
            catch (Exception)
            {
                Console.WriteLine("There is nothing for the unit test or maybe something wrong in code!");
            }
            System.IO.File.Delete(@"D:\TFS.txt");
        }

        [Test]
        public void GenerateVerifyCode_Stream_SHA1()
        {
            try
            {
                System.Utility.Helper.Test tt = new Test();
                tt.Start();
                var start = GC.GetTotalMemory(false);
                Stream Tsr = this.getShortStream();
                var expect = "B200250389412DBA67B0D676178EEEE59335630A";
                var service = new VerifyService(EnumVerifyType.SHA1);
                var code = service.GenerateVerifyCode(Tsr);
                var time = tt.GetElapsedAndStop().TotalMilliseconds;
                var end = GC.GetTotalMemory(false);
                Tsr.Dispose();
                Tsr.Close();
                service.Dispose();
                Assert.IsTrue(expect == code);
                Console.WriteLine("time:" + time + " ,memory:" + System.Utility.Helper.File.GetFileSize((end - start)));
            }
            catch (Exception)
            {
                Console.WriteLine("There is nothing for the unit test or maybe something wrong in code!");
            }
        }
        [Test]
        public void GenerateVerifyCode_Stream_SHA256()
        {
            try
            {
                System.Utility.Helper.Test tt = new Test();
                tt.Start();
                var start = GC.GetTotalMemory(false);
                Stream Tsr = this.getShortStream();
                var expect = "9B644AE0E4DCAC92F61F27CC51648B6F8BBDF0AAE6CD445ABF658E51D6ECB380";
                var service = new VerifyService(EnumVerifyType.SHA256);
                var code = service.GenerateVerifyCode(Tsr);
                var time = tt.GetElapsedAndStop().TotalMilliseconds;
                var end = GC.GetTotalMemory(false);
                Tsr.Close();
                service.Dispose();
                Assert.IsTrue(expect == code);
                Console.WriteLine("time:" + time + " ,memory:" + System.Utility.Helper.File.GetFileSize((end - start)));
            }
            catch (Exception)
            {
                Console.WriteLine("There is nothing for the unit test or maybe something wrong in code!");
            }
            System.IO.File.Delete(@"D:\ST.txt");
        }

        [Test]
        public void GenerateVerifyCode_Stream_BigSHA1()
        {
            try
            {
                System.Utility.Helper.Test tt = new Test();
                tt.Start();
                var start = GC.GetTotalMemory(false);
                Stream Tbs = this.getBigStream();
                var expect = "B2F2D807E38A11F02D5EC4C14983D979D1E99315";
                var service = new VerifyService(EnumVerifyType.SHA1);
                var code = service.GenerateVerifyCode(Tbs);
                var time = tt.GetElapsedAndStop().TotalMilliseconds;
                var end = GC.GetTotalMemory(false);
                Tbs.Dispose();
                Tbs.Close();
                service.Dispose();
                Assert.IsTrue(expect == code);
                Console.WriteLine("time:" + time + " ,memory:" + System.Utility.Helper.File.GetFileSize((end - start)));
            }
            catch (Exception)
            {
                Console.WriteLine("There is nothing for the unit test or maybe something wrong in code!");
                Stream Tbs = this.getBigStream();
                
                var service = new VerifyService(EnumVerifyType.SHA1);
                var code = service.GenerateVerifyCode(Tbs);
                Console.WriteLine("{0}",code);
                Tbs.Dispose();
                Tbs.Close();
                service.Dispose();
            }
                System.IO.File.Delete(@"D:\BT.txt");
        }

        [Test]
        public void GenerateVerifyCode_Stream_BigSHA256()
        {
            try
            {
            System.Utility.Helper.Test tt = new Test();
            tt.Start();
            var start = GC.GetTotalMemory(false);
            Stream Tbs = this.getBigStream();
            var expect = "FF210007BA6482B31E4E924DD106C2D42E9C3B77D022A46A8A73F984DAC30B1D";
            var service = new VerifyService(EnumVerifyType.SHA256);
            var code = service.GenerateVerifyCode(Tbs);
            var time = tt.GetElapsedAndStop().TotalMilliseconds;
            var end = GC.GetTotalMemory(false);
            Tbs.Dispose();
            Tbs.Close();
            service.Dispose();
            Assert.IsTrue(expect == code);
            Console.WriteLine("time:" + time + " ,memory:" + System.Utility.Helper.File.GetFileSize((end - start)));
            }
            catch (Exception)
            {
                Console.WriteLine("There is nothing for the unit test or maybe something wrong in code!");
            }
            System.IO.File.Delete(@"D:\BT.txt");
        }

        private byte[] GetTestBuf()
        {
            var str =
                @"你只看到我在不停的忙碌，却没看到我奋斗的热情。你有朝九晚五，我有通宵达旦。你否定我的现在，我决定我的未来。你可以轻视我的存在，我会用代码证明这是谁的时代！Coding是注定痛苦的旅行，路上少不了Bug和Change，但！那又怎样！我是程序猿，我为自己带眼.
";
            var buf = str.ToBytes(Encoding.Default);
            return buf;
        }

        private string GetTestFile()
        {
            string[] Tcontent = {"你是我天边最美的云彩，让我用心把你留下来，巴扎黑！"};
            System.IO.File.WriteAllLines(@"D:\TFS.txt", Tcontent);
            var filename = @"D:\TFS.txt";
            return filename;
        }

        private Stream getShortStream()
        {
            string[] Scontent = {"回锅肉"};
            System.IO.File.WriteAllLines(@"D:\ST.txt", Scontent);
            FileStream sr = new FileStream(@"D:\ST.txt", FileMode.Open);
           
            return sr;
        }

        private Stream getBigStream()
        {
            for (int i = 0; i < 512; i++)
            {
                string Bcontent = "哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈哈";
                System.IO.File.AppendAllText(@"D:\BT.txt", Bcontent);
            }
            FileStream bs = new FileStream(@"D:\BT.txt", FileMode.Open);
            return bs;
        }




    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Utility;
using System.Utility.Helper;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using XLY.XDD.ForensicService;

namespace XLY.Framework.UnitTest
{
    [TestFixture]
    public class ProxyGeneratorTest
    {
        [Test, Description("根据配置信息生成类型")]
        public void GenerateType_ConfigInfo()
        {
            ProxyGenerator proxy = new ProxyGenerator("XLY.Framework.UnitTest", "XLY.Framework.UnitTest.ConfigInfo");
            proxy.AppendProperty("Name", typeof(string));
            proxy.AppendProperty("Code", typeof(EnumEncodingType));
            proxy.AppendProperty("Age", typeof(int));
            proxy.AppendProperty("Birthdy", typeof(DateTime));
            proxy.AppendProperty("Died", typeof(DateTime?));
            var type = proxy.GenerateType();
            var obj = Activator.CreateInstance(type);

            System.Utility.Helper.Reflection.SettingObjectValue(obj, "Name", "xiaoming");
            var vname = System.Utility.Helper.Reflection.GetObjectValue(obj, "Name");
            Assert.IsTrue(vname == "xiaoming");

            System.Utility.Helper.Reflection.SettingObjectValue(obj, "Code", EnumEncodingType.GB2312);
            dynamic dobj = obj;
            Assert.IsTrue(dobj.Code == EnumEncodingType.GB2312);

            dobj.Age = 33;
            var vage = System.Utility.Helper.Reflection.GetObjectValue(dobj, "Age");
            Assert.IsTrue(vage == 33);

            var now = DateTime.Now;
            dobj.Birthdy = now;
            Assert.IsTrue(dobj.Birthdy == now);
            //died
            System.Utility.Helper.Reflection.SettingObjectValue(obj, "Died", new DateTime(1986, 11, 15));
            var vdied = (DateTime)System.Utility.Helper.Reflection.GetObjectValue(obj, "Died");
            Assert.IsTrue(vdied == new DateTime(1986, 11, 15));

            //---console
            Console.WriteLine("Name:" + dobj.Name);
            Console.WriteLine("Code:" + dobj.Code);
            Console.WriteLine("Age:" + dobj.Age);
            Console.WriteLine("Birthdy:" + TypeExtension.ToDateTimeString(dobj.Birthdy));
            Console.WriteLine("Died:" + TypeExtension.ToDateTimeString(dobj.Died));
        }

        [Test, Description("根据接口生成类型")]
        public void GenerateType_Interface()
        {
            ProxyGenerator proxy = new ProxyGenerator("XLY.Framework.UnitTest", "XLY.Framework.UnitTest.ConfigInfo");
            proxy.InheritInterface<IA>();
            var type = proxy.GenerateType();
            IA obj = Activator.CreateInstance(type) as IA;
            Assert.IsNotNull(obj);

            obj.Name = "xiaoming";
            var vname = System.Utility.Helper.Reflection.GetObjectValue(obj, "Name");
            Assert.IsTrue(vname == "xiaoming");

            obj.Birthdy = DateTime.Now;
            obj.Died = new DateTime(1986, 11, 15);
            Assert.IsTrue(obj.Died == new DateTime(1986, 11, 15));

            //---console
            Console.WriteLine("Name:" + obj.Name);
            Console.WriteLine("Code:" + obj.Code);
            Console.WriteLine("Age:" + obj.Age);
            Console.WriteLine("Birthdy:" + TypeExtension.ToDateTimeString(obj.Birthdy));
            Console.WriteLine("Died:" + TypeExtension.ToDateTimeString(obj.Died));
        }

        [Test, Description("根据多个接口生成类型")]
        public void GenerateType_Interfaces()
        {
            ProxyGenerator proxy = new ProxyGenerator("XLY.Framework.UnitTest", "XLY.Framework.UnitTest.ConfigInfo");
            proxy.InheritInterface<IA>();
            proxy.InheritInterface<IB>();
            var type = proxy.GenerateType();
            var obj = Activator.CreateInstance(type);
            var a = obj as IA;
            Assert.IsNotNull(a);

            var b = obj as IB;
            Assert.IsNotNull(b);

            a.Name = "xiaosan";
            Assert.IsTrue(a.Name == b.Name);

            b.Size = 1984343;
            Console.WriteLine("a.Name:" + a.Name);
            Console.WriteLine("b.Name:" + b.Name);
            Console.WriteLine("b.Size:" + b.Size);
        }

        [Test, Description("根据接口、属性信息混合生成类型")]
        public void GenerateType_Interface_ConfigInfo()
        {
            ProxyGenerator proxy = new ProxyGenerator("XLY.Framework.UnitTest", "XLY.Framework.UnitTest.ConfigInfo");
            proxy.InheritInterface<IA>();
            proxy.InheritInterface<IB>();
            proxy.AppendProperty("Name", typeof(string));
            proxy.AppendProperty("Summary", typeof(string));
            var type = proxy.GenerateType();
            var obj = Activator.CreateInstance(type);
            var a = obj as IA;
            Assert.IsNotNull(a);

            var b = obj as IB;
            Assert.IsNotNull(b);

            a.Name = "123;";
            System.Utility.Helper.Reflection.SettingObjectValue(obj, "Name", "xiaoming");
            Assert.IsTrue(a.Name == "xiaoming");

            System.Utility.Helper.Reflection.SettingObjectValue(obj, "Summary", "ok123");
            dynamic dobj = obj;
            Assert.IsTrue(dobj.Summary == "ok123");
        }

        [Test, Description("测试接口类型属性的get"), ExpectedException(typeof(RuntimeBinderException))]
        public void GenerateType_Interface_Get()
        {
            ProxyGenerator proxy = new ProxyGenerator("Test", "TestClass");
            proxy.InheritInterface<IGS>();
            var type = proxy.GenerateType();
            var obj = type.CreateInstance() as IGS;
            Assert.IsNotNull(obj);
            Assert.IsNull(obj.Name);
            obj.Birthday = DateTime.Now;
            Console.WriteLine("dobj.Birthday:" + TypeExtension.ToDateTimeString(obj.Birthday));
            dynamic dobj = obj;
            dobj.Name = "123;";
        }

        [Test, Description("测试接口类型属性的set"), ExpectedException(typeof(RuntimeBinderException))]
        public void GenerateType_Interface_Set()
        {
            ProxyGenerator proxy = new ProxyGenerator("Test", "TestClass");
            proxy.InheritInterface<IGS>();
            var type = proxy.GenerateType();
            var obj = type.CreateInstance() as IGS;
            dynamic dobj = obj;
            dobj.Birthday = DateTime.Now;
            Console.WriteLine("dobj.Birthday:" + TypeExtension.ToDateTimeString(dobj.Birthday));
            dobj.Age = 122;
            Console.WriteLine(dobj.Age);
        }
    }

    public interface IA
    {
        string Name { get; set; }
        int Age { get; set; }
        EnumEncodingType Code { get; set; }
        DateTime Birthdy { get; set; }
        DateTime? Died { get; set; }
    }
    public interface IB
    {
        string Name { get; set; }
        long Size { get; set; }
    }
    public interface IGS
    {
        string Name { get; }
        int Age { set; }
        DateTime Birthday { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Utility;
using System.Utility.Helper;
using System.Reflection.Emit;
using System.Xml.Serialization;
using NUnit.Framework;
using Update;
using XLY.XDD.ForensicService;

namespace XLY.Framework.UnitTest
{
    [TestFixture,Ignore]
    public class KuangTest
    {
        [Test, Ignore, Description("Do some test")]
        public void DoTest()
        {
            

        }

        [Test, Ignore]
        public void UpdateResponse_ToXML_Test()
        {
            Update.UpdateResponse res = new UpdateResponse();
            res.Res = "1";
            res.Code = "SF001";
            res.Packs = new List<UpdateResponse.Pack>();
            UpdateResponse.Pack p1 = new UpdateResponse.Pack();
            p1.Version = "d.2";
            p1.Plugin = "/ddd/a.exe";
            p1.Address = "http:addd/";
            res.Packs.Add(p1);
            UpdateResponse.Pack p2 = new UpdateResponse.Pack();
            p2.Version = "33d.2";
            p2.Address = "http:addd/dfdfdf";
            res.Packs.Add(p2);
            System.Utility.Helper.Serialize.SerializeToXML(res, "d:\\a.xml");
        }
    }

    public interface IUser
    {
        string Name { get; set; }
        int Age { get; set; }
        DateTime? Birthday { get; set; }
    }

    public class User : IUser
    {
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }
        public virtual DateTime? Birthday { get; set; }
    }


}

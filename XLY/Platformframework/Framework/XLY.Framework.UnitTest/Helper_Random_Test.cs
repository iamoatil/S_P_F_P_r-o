using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Utility.Helper;
using NUnit.Framework;

namespace XLY.Framework.UnitTest
{
    [TestFixture]
    public class Helper_Random_Test
    {
        [Test]
        public void NextBoolean_Test()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(System.Utility.Helper.Random.NextBoolean());
            }
        }
        [Test]
        public void NextEnum_EnumEncodingType_Test()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(System.Utility.Helper.Random.NextEnum<EnumEncodingType>());
            }
        }
        [Test]
        public void NextDatetime_Test()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(System.Utility.Helper.Random.NextDatetime().ToDateTimeString());
            }
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XLY.SF.Project.CaseManagement;

namespace XLY.SF.UnitTest
{
    [TestClass]
    public class CaseManagementTest
    {
        [TestMethod]
        public void TestXmlSchema()
        {
            XDocument doc = XDocument.Load(@"CaseProjectTemplate.cp");
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("", "CaseProjectTemplate.xsd");
            doc.Validate(schemaSet, ValidationEventHandler);
            
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
        }

        [TestMethod]
        public void TestCreateCase()
        {
            CaseInfo ci = new CaseInfo();
            ci.Name = "hj";
            ci.Number = "123";
            ci.Type = "1";
            ci.Author = "hj";
            ci.Path = @"F:\Source\Workspaces\SPF-PRO\Trunk\Trunk\Source\11.Service\18.Test\XLY.SF.UnitTest\bin\Debug";
            Case @case = Case.New(ci);
            Console.ReadKey();
            Assert.IsNotNull(@case);
        }

        [TestMethod]
        public void TestOpenCase()
        {
            String cpFile = @"F:\Source\Workspaces\SPF-PRO\Trunk\Trunk\Source\11.Service\18.Test\XLY.SF.UnitTest\bin\Debug\hj_20171016[112748]\CaseProject.cp";
            Case @case = Case.Open(cpFile);
            Console.ReadKey();
            Assert.IsNotNull(@case);
        }

        [TestMethod]
        public void TestChangeCaseName()
        {
            CaseInfo ci = new CaseInfo();
            ci.Name = "hj";
            ci.Number = "123";
            ci.Type = "1";
            ci.Author = "hj";
            ci.Path = @"F:\Source\Workspaces\SPF-PRO\Trunk\Trunk\Source\11.Service\18.Test\XLY.SF.UnitTest\bin\Debug";
            Case @case = Case.New(ci);
            String oldName = @case.Name;
            @case.Name = "newName";
            Assert.AreNotEqual(oldName, @case.Name);
        }

        [TestMethod]
        public void TestDeleteCase()
        {
            CaseInfo ci = new CaseInfo();
            ci.Name = "hj";
            ci.Number = "123";
            ci.Type = "1";
            ci.Author = "hj";
            ci.Path = @"F:\Source\Workspaces\SPF-PRO\Trunk\Trunk\Source\11.Service\18.Test\XLY.SF.UnitTest\bin\Debug";
            Case @case = Case.New(ci);
            @case.Delete();
            Assert.IsFalse(@case.Existed);
        }

        [TestMethod]
        public void TestCPConfiguration()
        {
            try
            {
                Case @case = Case.Open(@"CaseProjectTemplate.cp");
                @case.Name = "333333";
                //CPConfiguration cp = CPConfiguration.Create(ci);
            }
            catch (FormatException ex)
            {
            }
        }

    }
}

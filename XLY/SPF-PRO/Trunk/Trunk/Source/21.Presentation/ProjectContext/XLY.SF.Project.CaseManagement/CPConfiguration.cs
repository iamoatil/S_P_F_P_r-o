using System;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 案例项目配置。
    /// </summary>
    public sealed class CPConfiguration
    {
        #region Fields

        private static readonly WeakReference<XmlSchemaSet> SchemaSet = new WeakReference<XmlSchemaSet>(null);

        private const String DefaultProjectFile = "CaseProject.cp";

        private readonly XDocument _doc;

        private readonly String _directory;

        #endregion

        #region Constructors

        private CPConfiguration(XDocument doc, String directory, Boolean validation, String projectFile = null)
        {
            if (validation) Validate(doc);
            _doc = doc;
            _directory = directory;
            CaseProjectFile = projectFile ?? DefaultProjectFile;
            FullPath = Path.Combine(directory, CaseProjectFile);
        }

        #endregion

        #region Properties

        /// <summary>
        /// XSD架构信息。
        /// </summary>
        private static XmlSchemaSet XmlSchemaSet
        {
            get
            {
                if (!SchemaSet.TryGetTarget(out XmlSchemaSet ss))
                {
                    ss = GetSchema();
                    SchemaSet.SetTarget(ss);
                }
                return ss;
            }
        }

        /// <summary>
        /// 案例项目文件所在目录。
        /// </summary>
        public String Directory => _directory;

        /// <summary>
        /// 案例项目文件名称。
        /// </summary>
        public String CaseProjectFile { get; }

        /// <summary>
        /// 项目文件的完整路径。
        /// </summary>
        public String FullPath { get; }

        #endregion

        #region Methods

        #region Internal

        /// <summary>
        /// 打开已有的案例项目文件。
        /// </summary>
        /// <param name="file">案例项目文件路径。</param>
        /// <returns>CPConfiguration 类型实例。</returns>
        /// <exception cref="FormatException">案例项目文件格式不正确。</exception>
        internal static CPConfiguration Open(String file)
        {
            if (String.IsNullOrWhiteSpace(file)) return null;
            FileStream stream = null;
            try
            {
                if (!File.Exists(file)) return null;
                stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                XDocument doc = XDocument.Load(stream);
                return new CPConfiguration(doc, Path.GetDirectoryName(file), true, Path.GetFileName(file));
            }
            catch (FormatException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                stream?.Close();
            }
        }

        /// <summary>
        /// 创建新的案例项目文件。
        /// </summary>
        /// <param name="caseInfo">案例信息。</param>
        /// <returns>CPConfiguration 类型实例。</returns>
        internal static CPConfiguration Create(CaseInfo caseInfo)
        {
            XDocument doc = GetTemplate();
            XElement propertyGroup = doc.Root.Element("PropertyGroup");
            propertyGroup.Element("Id").Value = caseInfo.Id;
            propertyGroup.Element("Name").Value = caseInfo.Name ?? String.Empty;
            propertyGroup.Element("Number").Value = caseInfo.Number ?? String.Empty;
            propertyGroup.Element("Type").Value = caseInfo.Type ?? String.Empty;
            propertyGroup.Element("Author").Value = caseInfo.Author ?? String.Empty;
            caseInfo.Timestamp = DateTime.Now;
            propertyGroup.Element("Timestamp").Value = caseInfo.Timestamp.ToString("s");

            try
            {
                return new CPConfiguration(doc, Path.Combine(caseInfo.Path, caseInfo.GetDirectoryName()), false);
            }
            catch(FormatException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取案例信息。
        /// </summary>
        /// <returns>案例信息。</returns>
        internal RestrictedCaseInfo GetCaseInfo()
        {
            XElement propertyGroup = _doc.Root.Element("PropertyGroup");
            CaseInfo caseInfo = new CaseInfo();
            caseInfo.Id = propertyGroup.Element("Id").Value;
            caseInfo.Name = propertyGroup.Element("Name").Value;
            caseInfo.Number = propertyGroup.Element("Number").Value;
            caseInfo.Type = propertyGroup.Element("Type").Value;
            caseInfo.Author = propertyGroup.Element("Author").Value;
            caseInfo.Timestamp = DateTime.Parse(propertyGroup.Element("Timestamp").Value);
            caseInfo.Path = Path.GetDirectoryName(_directory);
            return new RestrictedCaseInfo(caseInfo) { Directory = _directory };
        }

        /// <summary>
        /// 更新案例名称。
        /// </summary>
        /// <param name="name">案例名称。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        internal Boolean UpdateName(String name)
        {
            XElement propertyGroup = _doc.Root.Element("PropertyGroup");
            String oldName = propertyGroup.Element("Name").Value;
            propertyGroup.Element("Name").Value = name;
            Boolean result = false;
            try
            {
                result = Save();
                return result;
            }
            catch (FormatException)
            {
                return result;
            }
            finally
            {
                if (!result)
                {
                    propertyGroup.Element("Name").Value = oldName;
                }
            }
        }

        /// <summary>
        /// 保存案例项目文件。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        /// <exception cref="FormatException">案例项目文件格式不正确。</exception>
        internal Boolean Save()
        {
            try
            {
                Validate(_doc);
                if (!System.IO.Directory.Exists(_directory))
                {
                    System.IO.Directory.CreateDirectory(_directory);
                }
                String file = Path.Combine(_directory, CaseProjectFile);
                FileStream stream = File.Create(file);
                _doc.Save(stream);
                stream.Close();
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Private

        private static XmlSchemaSet GetSchema()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            String assemblyName = assembly.GetName().Name;
            XmlSchemaSet ss = new XmlSchemaSet();
            using (Stream stream = assembly.GetManifestResourceStream($"{assemblyName}.CaseProjectTemplate.xsd"))
            {
                ss.Add("", XmlReader.Create(stream));
                return ss;
            }
        }

        private static XDocument GetTemplate()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            String assemblyName = assembly.GetName().Name;
            using (Stream stream = assembly.GetManifestResourceStream($"{assemblyName}.CaseProjectTemplate.cp"))
            {
                return XDocument.Load(stream);
            }
        }

        private void Validate(XDocument doc)
        {
            doc.Validate(XmlSchemaSet, (o, m) =>
            {
                throw new FormatException("Invalid value in case project file.", m.Exception);
            });
        }

        #endregion

        #endregion
    }
}

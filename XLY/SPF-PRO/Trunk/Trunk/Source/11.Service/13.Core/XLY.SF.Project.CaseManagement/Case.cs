using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 案例。
    /// </summary>
    public class Case
    {
        #region Constructors

        private Case(CaseInfo caseInfo)
        {
        }

        #endregion

        #region Properties

        public CaseInfo CaseInfo { get; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 创建新的案例。
        /// </summary>
        /// <returns>新的案例。</returns>
        public static CaseInfo New()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 保存案例。
        /// </summary>
        /// <param name="caseInfo">案例信息。</param>
        public static Case Save(CaseInfo caseInfo)
        {
            Case cm = new Case(caseInfo);
            return cm;
        }

        /// <summary>
        /// 删除指定目录路径下的案例，同时删除所在目录。
        /// </summary>
        /// <param name="path">案例所在目录路径。</param>
        public static void Delete(String path)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private

        private void Save()
        {
            CaseInfo ci = CaseInfo;
            if (String.IsNullOrWhiteSpace(ci.Name)
                || String.IsNullOrWhiteSpace(ci.Number)
                || String.IsNullOrWhiteSpace(ci.Type)
                || String.IsNullOrWhiteSpace(ci.Author)
                || String.IsNullOrWhiteSpace(ci.Path))
            {
                throw new ArgumentException("Exist invalid value.");
            }
            Directory.CreateDirectory(ci.Path);
            String configFile = Path.Combine(ci.Path, $"{ci.Name}.cp");
        }

        #endregion

        #endregion
    }
}

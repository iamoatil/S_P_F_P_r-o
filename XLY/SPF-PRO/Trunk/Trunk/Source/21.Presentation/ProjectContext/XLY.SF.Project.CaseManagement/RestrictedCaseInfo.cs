using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.CaseManagement
{
    /// <summary>
    /// 无法修改的案例信息。
    /// </summary>
    public sealed class RestrictedCaseInfo: CaseInfo
    {
        #region Fields

        internal readonly CaseInfo _caseInfo;

        #endregion

        #region Constructors

        internal RestrictedCaseInfo(CaseInfo caseInfo)
        {
            _caseInfo = caseInfo ?? throw new ArgumentNullException("caseInfo");
            Init(caseInfo);
        }

        #endregion

        #region Properties

        #region Name

        private String _name;

        /// <summary>
        /// 名称。
        /// </summary>
        public override String Name
        {
            get => _name;
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Number

        private String _number;

        /// <summary>
        /// 编号。
        /// </summary>
        public override String Number
        {
            get => _number;
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Type

        private String _type;

        /// <summary>
        /// 类型。
        /// </summary>
        public override String Type
        {
            get => _type;
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Author

        private String _author;

        /// <summary>
        /// 作者。
        /// </summary>
        public override String Author
        {
            get => _author;
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Path

        private String _path;

        /// <summary>
        /// 路径。
        /// </summary>
        public override String Path
        {
            get => _path;
            set { throw new NotSupportedException(); }
        }

        #endregion

        /// <summary>
        /// 案例所在目录。
        /// </summary>
        public String Directory { get; internal set; }

        /// <summary>
        /// 与之关联的可修改的案例信息实例。
        /// </summary>
        public CaseInfo Target => _caseInfo;

        #endregion

        #region Methods

        #region Internal

        internal void Commit()
        {
            _name = _caseInfo.Name;
        }

        #endregion

        #region Private

        private void Init(CaseInfo caseInfo)
        {
            Id = caseInfo.Id;
            Timestamp = caseInfo.Timestamp;
            _name = caseInfo.Name;
            _number = caseInfo.Number;
            _type = caseInfo.Type;
            _author = caseInfo.Author;
            _path = caseInfo.Path;
        }

        #endregion

        #endregion
    }
}

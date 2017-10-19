using System;
using System.Collections.Generic;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    [Serializable]
    public class FileX
    {
        /// <summary>
        /// 数据状态
        /// </summary>
        public EnumDataState DataState { get; set; }

        #region Properties（基本文件属性）

        public string Name { get; set; }

        /// <summary>
        /// 创建时间，可空
        /// </summary>
        public DateTime? Date
        {
            get { return this.CreationDate; }
            set { }
        }

        public string DateStr
        {
            get { return this.CreationDate.ToDateTimeString(); }
            set { }
        }

        /// <summary>
        /// 数据内容，文件地址
        /// </summary>
        public string Content
        {
            get { return this.FullPath; }
            set { }
        }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// 最后访问日期
        /// </summary>

        public DateTime LastAccessDate { get; set; }
        public string LastAccessDateStr { get { return LastAccessDate.ToDateTimeString(); } }

        /// <summary>
        /// 最后编辑日期
        /// </summary>
        public DateTime LastWriteData { get; set; }
        public string LastWriteDateStr { get { return LastWriteData.ToDateTimeString(); } }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get { return System.IO.Path.Combine(this.ParentDirectory, this.Name.Replace("\"", "")); }
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public EnumColumnType Type { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 大小描述
        /// </summary>
        public string SizeDesc
        {
            get { return Partition.GetFileSize(this.Size); }
        }

        public string SourcePhonePathStr
        {
            get
            {
                if (null == this.SourcePhonePath)
                {
                    return string.Empty;
                }
                return System.IO.Path.Combine(this.SourcePhonePath, this.Name);
            }
        }

        /// <summary>
        /// 手机内部路径
        /// </summary>
        public string SourcePhonePath { get; set; }

        /// <summary>
        /// 文件所在文件夹
        /// </summary>
        public string ParentDirectory { get; set; }

        /// <summary>
        /// 扩展名
        /// </summary>
        public string Extension
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public EnumFileAttributeFlags Attributes { get; set; }//位域

        /// <summary>
        /// 是否文件夹
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// 属性描述
        /// </summary>
        public string[] AttrubuteDesc
        {
            get { return this.Attributes.GetEnumFlagDescription(); }
        }

        private List<FileX> _Files;

        /// <summary>
        /// 子文件集合
        /// </summary>
        public List<FileX> Files
        {
            get 
            { 
                return this._Files; 
            }
            set
            {
                this._Files = value;
            }
        }

        #endregion

        public FileX()
        {            
            this.ParentDirectory = string.Empty;
            this.DataState = EnumDataState.Normal;
        }


        /// <summary>
        /// 文件图标句柄
        /// </summary>
        public IntPtr Icon { get; set; }


        #region DicProperties
        /// <summary>
        /// 属性信息字典集合
        /// </summary>
        public Dictionary<string, string> DicProperties
        {
            get
            {
                var dic = new Dictionary<string, string>();

                //dic.Add(LanguageHelperSingle.Instance.Language.OtherLanguage.FileXProperty_FileName, this.Name);
                //dic.Add(LanguageHelperSingle.Instance.Language.OtherLanguage.FileXProperty_FullPath, this.FullPath);
                //dic.Add(LanguageHelperSingle.Instance.Language.OtherLanguage.FileXProperty_ExtName, this.Extension);
                //dic.Add(LanguageHelperSingle.Instance.Language.OtherLanguage.FileXProperty_FileType, this.FileType);
                //dic.Add(LanguageHelperSingle.Instance.Language.OtherLanguage.FileXProperty_FileSize, this.SizeDesc);
                //dic.Add(LanguageHelperSingle.Instance.Language.OtherLanguage.FileXProperty_Property, this.AttrubuteDesc.ToString());
                //dic.Add(LanguageHelperSingle.Instance.Language.OtherLanguage.FileXProperty_CreateDate, this.CreationDate.ToDateTimeString());
                //dic.Add(LanguageHelperSingle.Instance.Language.OtherLanguage.FileXProperty_LastAccessDate, this.LastAccessDate.ToDateTimeString());
                //dic.Add(LanguageHelperSingle.Instance.Language.OtherLanguage.FileXProperty_LastEditDate, this.LastWriteData.ToDateTimeString());
                dic.Add(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_FileXProperty_FileName), this.Name);
                dic.Add(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_FileXProperty_FullPath), this.FullPath);
                dic.Add(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_FileXProperty_ExtName), this.Extension);
                dic.Add(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_FileXProperty_FileType), this.FileType);
                dic.Add(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_FileXProperty_FileSize), this.SizeDesc);
                dic.Add(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_FileXProperty_Property), this.AttrubuteDesc.ToString());
                dic.Add(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_FileXProperty_CreateDate), this.CreationDate.ToDateTimeString());
                dic.Add(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_FileXProperty_LastAccessDate), this.LastAccessDate.ToDateTimeString());
                dic.Add(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_FileXProperty_LastEditDate), this.LastWriteData.ToDateTimeString());
                return dic;
            }
        }
        #endregion

        #region 文件系统试用

        /// <summary>
        /// 文件ID
        /// </summary>
        public ulong FileID { get; set; }

        /// <summary>
        /// 文件父目录ID
        /// </summary>
        public ulong ParentID { get; set; }

        #endregion
    }
}


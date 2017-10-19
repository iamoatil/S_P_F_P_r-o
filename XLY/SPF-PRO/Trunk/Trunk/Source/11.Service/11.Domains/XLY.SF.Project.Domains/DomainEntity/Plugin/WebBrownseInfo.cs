using System;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// Web浏览器信息类。
    /// </summary>
    [Serializable]
    public class WebBrowseInfo
    {
        /// <summary>
        /// 浏览器类别。
        /// </summary>
        public string BrowseType { get; set; }

        /// <summary>
        /// 浏览器痕迹类别。
        /// </summary>
        public WebTracesType TracesType { get; set; }

        /// <summary>
        /// 标题。
        /// </summary>
        [Display]
        public string Title { get; set; }

        /// <summary>
        /// 主要内容。
        /// </summary>
        [Display]
        public string Content { get; set; }

        /// <summary>
        /// 关联时间。
        /// </summary>
        public DateTime? StartDate { get; set; }
        [Display]
        public string _StartDate
        {
            get { return this.StartDate.ToDateTimeString(); }
        }

        /// <summary>
        /// 备注。
        /// </summary>
        [Display]
        public string Remark { get; set; }

        /// <summary>
        /// 详细信息
        /// </summary>
        public string DetailContent
        {
            get { return this.ToString(); }
        }
    }


    /// <summary>
    /// Web缓存。
    /// </summary>
    [Serializable]
    public class WebCache : AbstractDataItem
    {

        [Display]
        public string Url { get; set; }

        [Display]
        public DateTime? ExpresTime { get; set; }

        [Display]
        public string Contentlength { get; set; }

        [Display]
        public string MimeType { get; set; }

        [Display]
        public string Encoding { get; set; }

        [Display]
        public int HttpSatus { get; set; }
    }

    /// <summary>
    /// Cookies
    /// </summary>
    [Serializable]
    public class WebCookie : AbstractDataItem
    {

        [Display]
        public string Name { get; set; }

        [Display]
        public string Value { get; set; }

        [Display]
        public string Domain { get; set; }

        public DateTime? ExpresTime { get; set; }
        [Display]
        public string _ExpresTime
        {
            get { return this.ExpresTime.ToDateTimeString(); }
        }
    }

    /// <summary>
    /// 历史记录。
    /// </summary>
    [Serializable]
    public class History : AbstractDataItem
    {

        [Display]
        public string Name { get; set; }

        [Display(ColumnType = EnumColumnType.URL)]
        public string Url { get; set; }

        /// <summary>
        /// 源网址
        /// </summary>
        [Display(ColumnType = EnumColumnType.URL)]
        public string OriginalURL { get; set; }

        public DateTime? VisitTime { get; set; }

        [Display]
        public string _VisitTime
        {
            get { return this.VisitTime.ToDateTimeString(); }
        }

        /// <summary>
        /// 来源
        /// </summary>
        [Display]
        public string Source { get; set; }

        [Display]
        public string Visits { get; set; }

        /// <summary>
        /// 图标超链接
        /// </summary>
        [Display]
        public string IconUrl { get; set; }

    }

    /// <summary>
    /// 书签。
    /// </summary>
    [Serializable]
    public class BookMark : AbstractDataItem
    {

        [Display]
        public string Path { get; set; }

        [Display]
        public string Title { get; set; }

        [Display(ColumnType = EnumColumnType.URL)]
        public string Url { get; set; }

        public DateTime? CreatedTime { get; set; }

        [Display(Key = "CreatedTime")]
        public string _CreatedTime
        {
            get { return this.CreatedTime.ToDateTimeString(); }
        }

        public string IsDeleted { get; set; }

    }

    /// <summary>
    /// Web特定站点用户名和密码。
    /// 当前只能获取QQ浏览器的密码。
    /// </summary>
    [Serializable]
    public class WebSitPassword : AbstractDataItem
    {
        [Display]
        public string Host { get; set; }

        [Display]
        public string UserName { get; set; }

        [Display]
        public string Password { get; set; }
    }

    /// <summary>
    /// 收藏
    /// </summary>
    [Serializable]
    public class Favorite : AbstractDataItem
    {

        /// <summary>
        /// 标题
        /// </summary>
        [Display]
        public string Title { get; set; }

        /// <summary>
        /// 网址
        /// </summary>
        [Display(ColumnType = EnumColumnType.URL)]
        public string URL { get; set; }

        /// <summary>
        /// 源网址
        /// </summary>
        [Display(ColumnType = EnumColumnType.URL)]
        public string OriginalURL { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Display]
        public string Source { get; set; }

        /// <summary>
        /// 图标存储路径
        /// </summary>
        [Display]
        public string IconPath { get; set; }

        /// <summary>
        /// 图标超链接
        /// </summary>
        [Display(ColumnType = EnumColumnType.URL)]
        public string IconUrl { get; set; }

        /// <summary>
        /// 收藏时间
        /// </summary>
        public DateTime? AddTime { get; set; }

        [Display]
        public string _AddTime
        {
            get { return this.AddTime.ToDateTimeString(); }
        }

        /// <summary>
        /// 文章图片
        /// </summary>
        [Display]
        public string ImageList { get; set; }
        
    }

    /// <summary>
    /// 头条
    /// </summary>
    [Serializable]
    public class HeadLine : AbstractDataItem
    {

        /// <summary>
        /// 名称
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display]
        public string Title { get; set; }

        /// <summary>
        /// 读取状态
        /// </summary>
        public string ReadStatus { get; set; }

        [Display]
        public string _ReadStatus
        {
            get
            {
                if (this.ReadStatus == "0")
                {
                    return LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_MessageStatus_Read);
                }
                else if (this.ReadStatus == "1")
                {
                    return LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_MessageStatus_UnRead);
                }
                return LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_MessageStatus_UnRead);
            }
        }

        /// <summary>
        /// 网址
        /// </summary>
        [Display(ColumnType = EnumColumnType.URL)]
        public string Url { get; set; }

        public DateTime? UpTime { get; set; }

        [Display(Alignment = EnumAlignment.Center)]
        public string _UpTime
        {
            get { return this.UpTime.ToDateTimeString(); }
        }
 
    }

    /// <summary>
    /// 订阅号
    /// </summary>
    [Serializable]
    public class SubscriptionNumber : AbstractDataItem
    {

        /// <summary>
        /// 键
        /// </summary>
        //[Display(Width = 500, Text = "")]
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        //[Display(Width = 500, Text = "值")]
        public string Value { get; set; }

        /// <summary>
        /// 实体数据
        /// </summary>
        [Display]
        public string Data { get; set; }
        public string _Data
        {
            get { return this.Data.IsValid() ? this.Data : this.Value; }
            set { Data = value; }
        }

        /// <summary>
        /// 发布时间
        /// </summary>
        [Display(Alignment = EnumAlignment.Center)]
        public string Expires { get; set; }
    }

    /// <summary>
    /// 下载/文件
    /// </summary>
    [Serializable]
    public class DownloadFile : AbstractDataItem
    {

        /// <summary>
        /// ID号
        /// </summary>
        public long? ID { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 类型（1为安装包，2为视频[mp4]，3为音乐[wav\mp3]、4为图片[jpg、gif、png]、5为文档/小说[txt、xlsx、html、docx]、6为离线网页、7为压缩文件）、8为其他文件、9-本地书籍、10-下载文件、11-下载视频
        /// </summary>
        public long Type { get; set; }

        [Display]
        public string _Type
        {
            get
            {
                // 1- 8 为 [FileMgmt.db].[file_mgmt_detail] 定义
                if (this.Type == 1)
                {
                    return "Installer";
                }
                else if (this.Type == 2)
                {
                    return "Video";
                }
                else if (this.Type == 3)
                {
                    return "Music";
                }
                else if (this.Type == 4)
                {
                    return "Picture";
                }
                else if (this.Type == 5)
                {
                    return "Document";
                }
                else if (this.Type == 6)
                {
                    return "Web Page";
                }
                else if (this.Type == 7)
                {
                    return "Zip file";
                }
                else if (this.Type == 8)
                {
                    return "Others";
                }
                else if (this.Type == 9)
                {
                    return "Local Book";
                }
                else if (this.Type == 10)
                {
                    return "Download File";
                }
                else if (this.Type == 11)
                {
                    return "Download Video";
                }
                return "Undefiend";
            }
        }

        /// <summary>
        /// 大小
        /// </summary>
        [Display]
        public long Size { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [Display]
        public string Path { get; set; }

        /// <summary>
        /// 最近修改时间
        /// </summary>
        public DateTime? LastModified { get; set; }

        [Display(Alignment = EnumAlignment.Center)]
        public string _LastModified
        {
            get { return this.LastModified.ToDateTimeString(); }
        }
    }

    /// <summary>
    /// 搜索记录
    /// </summary>
    [Serializable]
    public class SearchHistory : AbstractDataItem
    {

        /// <summary>
        /// 搜索内容
        /// </summary>
        [Display]
        public string Search_Context { get; set; }

        /// <summary>
        /// 搜索URL
        /// </summary>
        [Display( ColumnType = EnumColumnType.URL)]
        public string Search_Url { get; set; }

        /// <summary>
        /// 搜索时间
        /// </summary>
        public DateTime? Search_Datetime { get; set; }

        [Display(Alignment = EnumAlignment.Center)]
        public string _Search_Datetime
        {
            get { return this.Search_Datetime.ToDateTimeString(); }
        }
    }

    /// <summary>
    /// 小说书架
    /// </summary>
    [Serializable]
    public class Novels : AbstractDataItem
    {

        /// <summary>
        /// ID号
        /// </summary>
        public string Book_ID { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [Display]
        public string Novel_Name { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [Display]
        public string Novel_Author { get; set; }

        /// <summary>
        /// 章节数目
        /// </summary>
        [Display]
        public string Summary { get; set; }

        /// <summary>
        /// 章节对应表名（在同一数据库中）
        /// </summary>
        public string Catalog_Table_Name { get; set; }

        /// <summary>
        /// 章节明细
        /// </summary>
        [Display]
        public string SummaryDetial { get; set; }

        /// <summary>
        /// 过期时间（0为长期有效）
        /// </summary>
        public long? Expire_Time { get; set; }

        [Display]
        public string _Expire_Time
        {
            get { return this.Expire_Time == 0 ? LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Status_Yes) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Status_No); }
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? Update_Time { get; set; }

        [Display(Alignment = EnumAlignment.Center)]
        public string _Update_Time
        {
            get { return this.Update_Time.ToDateTimeString(); }
        }
    }
}

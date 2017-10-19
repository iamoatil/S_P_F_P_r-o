using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

/* ==============================================================================
* Description：FilterArgs  
* Author     ：Fhjun
* Create Date：2017/3/23 14:52:07
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract
{

    /// <summary>
    /// 过滤的参数
    /// </summary>
    public abstract class FilterArgs
    {
        /// <summary>
        /// 过滤类型
        /// </summary>
        public virtual FilterEnum FilterType { get;}
        /// <summary>
        /// 是否已经处理完成，为true则表示已经处理完成，停止操作
        /// </summary>
        public bool Handled { get; set; } = false;
    }

    /// <summary>
    /// 字符串查询，string.Contains()
    /// </summary>
    public class FilterByStringContainsArgs : FilterArgs
    {
        public override FilterEnum FilterType => FilterEnum.StringContains;
        /// <summary>
        /// 查询的文字
        /// </summary>
        public string PatternText { get; set; }
        
        public override string ToString()
        {
            return $"关键词:{PatternText}";
        }
    }

    /// <summary>
    /// 正则匹配查询，比如查询url、身份证号等
    /// </summary>
    public class FilterByRegexArgs : FilterArgs
    {
        public static readonly FilterByRegexArgs RegexUrl = new FilterByRegexArgs() { Regex = new Regex(@"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&$%\$#\=~])*$") };
        public static readonly FilterByRegexArgs RegexIDCard = new FilterByRegexArgs() { Regex = new Regex(@"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$") };
        public static readonly FilterByRegexArgs RegexPhone = new FilterByRegexArgs() { Regex = new Regex(@"^(\d{3,4}-)?\d{6,8}$") };

        public override FilterEnum FilterType => FilterEnum.Regex;
        /// <summary>
        /// 查询的正则表达式
        /// </summary>
        public Regex Regex { get; set; }

        public override string ToString()
        {
            return $"正则:{Regex}";
        }
    }

    /// <summary>
    /// 在任务路径下查询文件，需要输入文件名
    /// </summary>
    public class FilterByFilePathArgs : FilterArgs
    {
        public override FilterEnum FilterType => FilterEnum.FilePath;
        /// <summary>
        /// 查询的文件名
        /// </summary>
        public string FileName { get; set; }
        public override string ToString()
        {
            return $"文件查询:{FileName}";
        }
    }

    /// <summary>
    /// 文件二进制查询，输入查询的byte数组
    /// </summary>
    public class FilterByFileBinaryArgs : FilterArgs
    {
        public override FilterEnum FilterType => FilterEnum.FileBinary;
        /// <summary>
        /// 查询的16进制
        /// </summary>
        public byte[] Content { get; set; }
        public override string ToString()
        {
            return $"二进制查询:{Content}";
        }
    }

    /// <summary>
    /// 文件内容查询，输入待查询的字符串
    /// </summary>
    public class FilterByFileContentArgs : FilterArgs
    {
        public override FilterEnum FilterType => FilterEnum.FileContent;
        /// <summary>
        /// 查询的文件内容
        /// </summary>
        public string PatternText { get; set; }
        /// <summary>
        /// 文件编码
        /// </summary>
        public Encoding Encoding { get; set; }
        public override string ToString()
        {
            return $"文件内容:{PatternText}";
        }
    }

    /// <summary>
    /// 时间范围查询，输入起始/结束时间
    /// </summary>
    public class FilterByDateRangeArgs : FilterArgs
    {
        public override FilterEnum FilterType => FilterEnum.DateTimeRange;
        /// <summary>
        /// 查询的起始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 查询的结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        public override string ToString()
        {
            return $"时间范围:{StartTime}-{EndTime}";
        }
    }

    /// <summary>
    /// 数据状态查询，输入正常/删除状态
    /// </summary>
    public class FilterByEnumStateArgs : FilterArgs
    {
        public override FilterEnum FilterType => FilterEnum.EnumState;
        /// <summary>
        /// 查询的数据状态
        /// </summary>
        public EnumDataState State { get; set; }
        public override string ToString()
        {
            return $"数据状态:{State}";
        }
    }

    /// <summary>
    /// 书签状态查询，输入书签编号
    /// </summary>
    public class FilterByBookmarkArgs : FilterArgs
    {
        public override FilterEnum FilterType => FilterEnum.BookmarkState;
        /// <summary>
        /// 查询的书签状态
        /// </summary>
        public int BookmarkId { get; set; }
        public override string ToString()
        {
            return $"标记状态:{BookmarkId}";
        }
    }

    /// <summary>
    /// 账号查询，输入查询的账号(即只查询账号名称或ID)
    /// </summary>
    public class FilterByAccountArgs : FilterArgs
    {
        public override FilterEnum FilterType => FilterEnum.Account;
        /// <summary>
        /// 查询的账号名称或ID号
        /// </summary>
        public string AccountPattern { get; set; }
        public override string ToString()
        {
            return $"账号查询:{AccountPattern}";
        }
    }

    /// <summary>
    /// 位置信息查询，输入地点描述或经纬度范围
    /// </summary>
    public class FilterByLocationArgs : FilterArgs
    {
        public override FilterEnum FilterType => FilterEnum.Location;
        /// <summary>
        /// 查询的地点描述
        /// </summary>
        public string LocationPattern { get; set; }
        public override string ToString()
        {
            return $"位置查询:{LocationPattern}";
        }
    }
}

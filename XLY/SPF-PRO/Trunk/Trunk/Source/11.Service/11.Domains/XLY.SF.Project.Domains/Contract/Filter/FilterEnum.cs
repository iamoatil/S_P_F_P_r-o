using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：FilterEnum  
* Author     ：Fhjun
* Create Date：2017/3/23 10:46:28
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract
{
    /// <summary>
    /// 查询类型
    /// </summary>
    [Flags]
    public enum FilterEnum:ulong
    {
        /// <summary>
        /// 字符串查询，string.Contains()
        /// </summary>
        StringContains = 0x01,
        /// <summary>
        /// 正则匹配查询，比如查询url、身份证号等
        /// </summary>
        Regex = 0x02,
        /// <summary>
        /// 在任务路径下查询文件，需要输入文件名
        /// </summary>
        FilePath = 0x04,
        /// <summary>
        /// 文件二进制查询，输入查询的byte数组
        /// </summary>
        FileBinary = 0x08,
        /// <summary>
        /// 文件内容查询，输入待查询的字符串
        /// </summary>
        FileContent = 0x10,
        /// <summary>
        /// 时间范围查询，输入起始/结束时间
        /// </summary>
        DateTimeRange = 0x20,
        /// <summary>
        /// 数据状态查询，输入正常/删除状态
        /// </summary>
        EnumState = 0x40,
        /// <summary>
        /// 书签状态查询，输入bool值
        /// </summary>
        BookmarkState = 0x80,
        /// <summary>
        /// 账号查询，输入查询的账号(即只查询账号名称或ID)
        /// </summary>
        Account = 0x100,
        /// <summary>
        /// 位置信息查询，输入地点描述或经纬度范围
        /// </summary>
        Location = 0x200,
    }
}

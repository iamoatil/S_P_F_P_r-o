using System;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// QQ帐号
    /// </summary>
    [Serializable]
    public sealed class QQAccountShow : AbstractDataItem
    {
        /// <summary>
        /// 全称  昵称(QQ号码)
        /// </summary>
        public string FullName
        {
            get
            {
                if (Nick.IsValid())
                {
                    return string.Format("{0}({1})", Nick, QQNumber);
                }
                return QQNumber;
            }
        }

        /// <summary>
        /// QQ号码
        /// </summary>
        [Display]
        public string QQNumber { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Display]
        public string Nick { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [Display]
        public int Age { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display]
        public EnumSex Sex { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [Display]
        public string Signature { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [Display]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Display]
        public string Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Display]
        public string Address { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        [Display]
        public string Company { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        [Display]
        public string School { get; set; }

        /// <summary>
        /// 个人说明
        /// </summary>
        [Display]
        public string PersonNote { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        [Display]
        public DateTime? RecentLoginDate { get; set; }

    }

    /// <summary>
    /// QQ好友
    /// </summary>
    [Serializable]
    public sealed class QQFriendShow : AbstractDataItem
    {
        /// <summary>
        /// 全称  备注OR昵称(QQ号码)
        /// </summary>
        public string FullName
        {
            get
            {
                if (Remark.IsValid())
                {
                    return string.Format("{0}({1})", Remark, QQNumber);
                }
                else if (Nick.IsValid())
                {
                    return string.Format("{0}({1})", Nick, QQNumber);
                }
                return QQNumber;
            }
        }

        /// <summary>
        /// QQ号码
        /// </summary>
        [Display]
        public string QQNumber { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Display]
        public string Nick { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display]
        public string Remark { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        [Display]
        public string Alias { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display]
        public EnumSex Sex { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [Display]
        public int Age { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [Display]
        public string Signature { get; set; }

        /// <summary>
        /// 最新动态
        /// </summary>
        [Display]
        public string Feed { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Display]
        public DateTime? LatestUpdate { get; set; }

    }

    /// <summary>
    /// QQ好友分组
    /// </summary>
    [Serializable]
    public sealed class QQFriendSetShow : AbstractDataItem
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 好友数量
        /// </summary>
        [Display]
        public int MemberCount { get; set; }
    }

    /// <summary>
    /// QQ群
    /// </summary>
    [Serializable]
    public class QQGroupShow : AbstractDataItem
    {
        /// <summary>
        /// 全称  群名称(QQ号码)
        /// </summary>
        public string FullName
        {
            get
            {
                if (Name.IsValid())
                {
                    return string.Format("{0}({1})", Name, QQNumber);
                }
                return QQNumber;
            }
        }

        /// <summary>
        /// 群名称
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 群帐号
        /// </summary>
        [Display]
        public string QQNumber { get; set; }

        /// <summary>
        /// 创建者帐号
        /// </summary>
        [Display]
        public string Creator { get; set; }

        /// <summary>
        /// 群说明
        /// </summary>
        [Display]
        public string Desc { get; set; }

        /// <summary>
        /// 群公告
        /// </summary>
        [Display]
        public string Notice { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 成员数量
        /// </summary>
        [Display]
        public int MemberCount { get; set; }

    }

    /// <summary>
    /// QQ讨论组
    /// </summary>
    [Serializable]
    public class QQDiscussShow : AbstractDataItem
    {
        /// <summary>
        /// 全称  讨论组名称(QQ号码)
        /// </summary>
        public string FullName
        {
            get
            {
                if (Name.IsValid())
                {
                    return string.Format("{0}({1})", Name, QQNumber);
                }
                return QQNumber;
            }
        }

        /// <summary>
        /// 讨论组名称
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 讨论组帐号
        /// </summary>
        [Display]
        public string QQNumber { get; set; }

        /// <summary>
        /// 创建者帐号
        /// </summary>
        [Display]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 成员数量
        /// </summary>
        [Display]
        public int MemberCount { get; set; }
    }

    /// <summary>
    /// QQ最近联系人
    /// </summary>
    [Serializable]
    public sealed class QQRecentShow : AbstractDataItem
    {
        /// <summary>
        /// QQ号码
        /// </summary>
        [Display]
        public string QQNumber { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display]
        public string Name { get; set; }

        /// <summary>
        /// 最后联系时间
        /// </summary>
        [Display]
        public DateTime? RecentDatetime { get; set; }

    }

}

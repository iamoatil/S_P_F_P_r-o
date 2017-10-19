using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 联系人
    /// </summary>
    [Serializable]
    public class Contact : AbstractDataItem
    {
        #region 显示属性

        /// <summary>
        /// 联系人全称
        /// </summary>
        [Display]
        public string ContactName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(this.Name))
                {
                    return this.Number;
                }

                if (String.IsNullOrWhiteSpace(this.Number))
                {
                    return this.Name;
                }
                if (string.IsNullOrEmpty(this.BBNumber) || this.BBNumber.Length <= 11)
                    return string.Format("{0}({1})", this.Name, this.Number);
                else
                    return string.Format("{0}\r\n({1})", this.Name, this.BBNumber);
            }
        }

        /// <summary>
        /// 号码归属地
        /// </summary>
        [Display]
        public string LocationInfo
        {
            get { return string.Format("{0}-{1} {2}", this.Province, this.City, this.Operator); }
        }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Display(Key = "邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 组织(公司+职位）
        /// </summary>
        [Display(Key = "组织")]
        public string Organization { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Display(Key = "地址")]
        public string PostalAddress { get; set; }

        /// <summary>
        /// 最后联系时间
        /// </summary>

        public DateTime? LastContactDate { get; set; }

        [Display(Alignment = EnumAlignment.Left, Key = "LastDate")]
        public string _LastContactDate
        {
            get { return this.LastContactDate.ToString(); }
        }

        /// <summary>
        /// 其他备注
        /// </summary>
        [Display(Key = "RemarkInfo")]
        public string Remark { get; set; }

        /// <summary>
        /// 联系人创建时间。
        /// </summary>
        public DateTime? CreateDate { get; set; }

        [Display(Key = "创建时间")]
        public string _CreateDate
        {
            get { return this.CreateDate.ToString(); }
        }
        #endregion

        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 主号码
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 黑莓：主号码
        /// </summary>
        public string BBNumber { get; set; }

        /// <summary>
        /// 即时通信号
        /// </summary>
        public string ImNumber { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 网络电话
        /// </summary>
        public string SipAddress { get; set; }

        /// <summary>
        /// 网站
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 运营商
        /// </summary>
        public string Operator { get; set; }

        public bool Equals(Contact other)
        {
            if (other == null)
                return false;
            if (this.Name.ToString().Equals(other.Name) && this.Number.ToString().Equals(other.Number) && this.DataState == other.DataState)
                return true;
            return false;
        }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        public double Latitude { get; set; }
    }
}

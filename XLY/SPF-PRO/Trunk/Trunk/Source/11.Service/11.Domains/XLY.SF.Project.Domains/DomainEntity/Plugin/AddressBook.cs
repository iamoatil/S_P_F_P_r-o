using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Language;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 黑莓-联系人
    /// </summary>
    public class AddressBook
    {
        public string FirstNumber
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Working_phone01))
                    return this.Working_phone01;
                else if (!string.IsNullOrEmpty(this.Working_phone02))
                    return this.Working_phone02;
                else if (!string.IsNullOrEmpty(this.Residential_telephone01))
                    return this.Residential_telephone01;
                else if (!string.IsNullOrEmpty(this.Residential_telephone02))
                    return this.Residential_telephone02;
                else if (!string.IsNullOrEmpty(this.Mobile_phone01))
                    return this.Mobile_phone01;
                else if (!string.IsNullOrEmpty(this.Mobile_phone02))
                    return this.Mobile_phone02;
                else
                    return PhoneInfo;
            }
        }
        /// <summary>
        /// 所有电话号码
        /// </summary>
        public string PhoneInfo
        {
            get
            {
                StringBuilder sb1 = new StringBuilder();
                if (!string.IsNullOrEmpty(this.Working_phone01) && !string.IsNullOrEmpty(this.Working_phone02))
                {
                    sb1.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_Work) + " [{0},{1}]", this.Working_phone01, this.Working_phone02);
                }
                else if (!string.IsNullOrEmpty(this.Working_phone01))
                {
                    sb1.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_Work)+ " [{0}]", this.Working_phone01);
                }
                else if (!string.IsNullOrEmpty(this.Working_phone02))
                {
                    sb1.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_Work) + " [{0}]", this.Working_phone02);
                }

                StringBuilder sb2 = new StringBuilder();
                if (!string.IsNullOrEmpty(this.Residential_telephone01) && !string.IsNullOrEmpty(this.Residential_telephone02))
                {
                    sb2.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_House) + " [{0},{1}]", this.Residential_telephone01, this.Residential_telephone02);
                }
                else if (!string.IsNullOrEmpty(this.Residential_telephone01))
                {
                    sb2.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_House) + " [{0}]", this.Residential_telephone01);
                }
                else if (!string.IsNullOrEmpty(this.Residential_telephone02))
                {
                    sb2.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_House) + " [{0}]", this.Residential_telephone02);
                }

                StringBuilder sb3 = new StringBuilder();
                if (!string.IsNullOrEmpty(this.Mobile_phone01) && !string.IsNullOrEmpty(this.Mobile_phone02))
                {
                    sb3.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_Mobile) + " [{0},{1}]", this.Mobile_phone01, this.Mobile_phone02);
                }
                else if (!string.IsNullOrEmpty(this.Mobile_phone01))
                {
                    sb3.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_Mobile) + " [{0}]", this.Mobile_phone01);
                }
                else if (!string.IsNullOrEmpty(this.Mobile_phone02))
                {
                    sb3.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_Mobile) + " [{0}]", this.Mobile_phone02);
                }

                StringBuilder sb4 = new StringBuilder();
                if (!string.IsNullOrEmpty(sb1.ToString()))
                {
                    sb4.AppendFormat("{0}", sb1.ToString());
                }
                if (!string.IsNullOrEmpty(sb2.ToString()))
                {
                    sb4.AppendFormat("{0}", string.IsNullOrEmpty(sb4.ToString()) ? sb2.ToString() : "\r\n" + sb2.ToString());
                }
                if (!string.IsNullOrEmpty(sb3.ToString()))
                {
                    sb4.AppendFormat("{0}", string.IsNullOrEmpty(sb4.ToString()) ? sb3.ToString() : "\r\n" + sb3.ToString());
                }
                return sb4.ToString();
            }
        }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string EmailInfo
        {
            /*
             add.Email01,//电子邮件01
            add.Email01,//电子邮件02
             */
            get
            {
                StringBuilder sb1 = new StringBuilder();
                if (!string.IsNullOrEmpty(this.Email01) && !string.IsNullOrEmpty(this.Email02))
                {
                    sb1.AppendFormat("{0}\r\n{1}", this.Email01, this.Email02);
                }
                else if (!string.IsNullOrEmpty(this.Email01))
                {
                    sb1.AppendFormat("{0}", this.Email01);
                }
                else if (!string.IsNullOrEmpty(this.Email02))
                {
                    sb1.AppendFormat("{0}", this.Email02);
                }
                return sb1.ToString();
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string PostalAddressInfo
        {
            ////-----------------------------工作地址
            //    add.Work_residence01 = str_Data;//工作住宅01--地址1
            //    add.Work_residence02 = str_Data;//工作住宅02--地址2
            ////-----------------------------住宅地址
            //    add.Residential_address01 = str_Data;//住宅地址01--地址1
            //    add.Residential_address02 = str_Data;//住宅地址02--地址2
            get
            {
                StringBuilder sb1 = new StringBuilder();
                if (!string.IsNullOrEmpty(this.Work_residence01) && !string.IsNullOrEmpty(this.Work_residence02))
                {
                    sb1.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_Work) + " [{0},{1}]", this.Work_residence01, this.Work_residence02);
                }
                else if (!string.IsNullOrEmpty(this.Work_residence01))
                {
                    sb1.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_Work) + " [{0}]", this.Work_residence01);
                }
                else if (!string.IsNullOrEmpty(this.Work_residence02))
                {
                    sb1.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_Work) + " [{0}]", this.Work_residence02);
                }

                StringBuilder sb2 = new StringBuilder();
                if (!string.IsNullOrEmpty(this.Residential_address01) && !string.IsNullOrEmpty(this.Residential_address02))
                {
                    sb2.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_House) + " [{0},{1}]", this.Residential_address01, this.Residential_address02);
                }
                else if (!string.IsNullOrEmpty(this.Residential_address01))
                {
                    sb2.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_House) + " [{0}]", this.Residential_address01);
                }
                else if (!string.IsNullOrEmpty(this.Residential_address02))
                {
                    sb2.AppendFormat(LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_AddressBook_House) + " [{0}]", this.Residential_address02);
                }

                StringBuilder sb4 = new StringBuilder();
                if (!string.IsNullOrEmpty(sb1.ToString()))
                {
                    sb4.AppendFormat("{0}", sb1.ToString());
                }
                if (!string.IsNullOrEmpty(sb2.ToString()))
                {
                    sb4.AppendFormat("{0}", string.IsNullOrEmpty(sb4.ToString()) ? sb2.ToString() : "\r\n" + sb2.ToString());
                }
                return sb4.ToString();
            }
        }

        /// <summary>
        /// 名
        /// 20
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 姓
        /// 20
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 称谓
        /// 37
        /// </summary>
        public string Appellation { get; set; }
        /// <summary>
        /// 公司
        /// 21
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 电话呼叫
        /// </summary>
        public string TelephoneCall { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string News { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// 电子邮件01
        /// </summary>
        public string Email01 { get; set; }
        /// <summary>
        /// 电子邮件02
        /// </summary>
        public string Email02 { get; set; }
        /// <summary>
        /// 工作电话01
        /// </summary>
        public string Working_phone01 { get; set; }
        /// <summary>
        /// 工作电话02
        /// </summary>
        public string Working_phone02 { get; set; }
        /// <summary>
        /// 住宅电话01
        /// </summary>
        public string Residential_telephone01 { get; set; }
        /// <summary>
        /// 住宅电话02
        /// </summary>
        public string Residential_telephone02 { get; set; }
        /// <summary>
        /// 手机01
        /// </summary>
        public string Mobile_phone01 { get; set; }
        /// <summary>
        /// 手机02
        /// </summary>
        public string Mobile_phone02 { get; set; }
        /// <summary>
        /// 寻呼机
        /// </summary>
        public string Beeper { get; set; }
        /// <summary>
        /// 工作传真
        /// </summary>
        public string Work_fax { get; set; }
        /// <summary>
        /// 住宅传真
        /// </summary>
        public string Residential_fax { get; set; }
        /// <summary>
        /// 其他
        /// </summary>
        public string Other { get; set; }
        /// <summary>
        /// pin
        /// </summary>
        public string Pin { get; set; }
        /// <summary>
        /// 工作住宅01
        /// </summary>
        public string Work_residence01 { get; set; }
        /// <summary>
        /// 工作邮编
        /// </summary>
        public string Work_zip_code { get; set; }
        /// <summary>
        /// 工作住宅02
        /// </summary>
        public string Work_residence02 { get; set; }
        /// <summary>
        /// 工作国
        /// </summary>
        public string Work_country { get; set; }
        /// <summary>
        /// 工作地市
        /// </summary>
        public string Work_City { get; set; }
        /// <summary>
        /// 工作省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 住宅地址01
        /// </summary>
        public string Residential_address01 { get; set; }
        /// <summary>
        /// 住宅地址02
        /// </summary>
        public string Residential_address02 { get; set; }
        /// <summary>
        /// 住宅地市
        /// </summary>
        public string Residential_city { get; set; }
        /// <summary>
        /// 住宅省
        /// </summary>
        public string Residential_province { get; set; }
        /// <summary>
        /// 住宅邮编
        /// </summary>
        public string Residential_zip_code { get; set; }
        /// <summary>
        /// 住宅国家
        /// </summary>
        public string Residential_country { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 纪念日
        /// </summary>
        public string Anniversaries_of_important_events { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 网站
        /// </summary>
        public string Website { get; set; }
    }
}

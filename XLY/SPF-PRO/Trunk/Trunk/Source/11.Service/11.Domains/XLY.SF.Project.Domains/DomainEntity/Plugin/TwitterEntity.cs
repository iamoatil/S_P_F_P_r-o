using System;
using System.Text.RegularExpressions;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// Twitter相关抽象类 
    /// </summary>
    [Serializable]
    public abstract class AbstractTwitterEntity : AbstractDataItem
    {
    }

    /// <summary>
    /// Twitter帐号信息
    /// </summary>
    [Serializable]
    public class TwitterAccount : AbstractTwitterEntity
    {
        [Display]
        public string UserId { get; set; }

        [Display]
        public string UserName { get; set; }

        [Display]
        public string NickName { get; set; }

        [Display]
        public string Description { get; set; }

        [Display]
        public string WebUrl { get; set; }

        [Display]
        public string ImageUrl { get; set; }

        [Display]
        public string HeadUrl { get; set; }

        [Display]
        public string Location { get; set; }

        [Display]
        public int Followers { get; set; }

        [Display]
        public int Friends { get; set; }

        [Display]
        public int Statuses { get; set; }

        [Display]
        public int Favorites { get; set; }

        [Display]
        public int MediaCount { get; set; }

        public string FriendShipTime { get; set; }

        public static TwitterAccount DyConvert(dynamic data)
        {
            TwitterAccount account = new TwitterAccount();
            if (data != null)
            {
                account.UserId = DynamicConvert.ToSafeString(data.user_id);
                account.UserName = DynamicConvert.ToSafeString(data.username);
                account.NickName = DynamicConvert.ToSafeString(data.name);
                account.Description = DynamicConvert.ToSafeString(data.description);
                account.WebUrl = DynamicConvert.ToSafeString(data.web_url);
                account.ImageUrl = DynamicConvert.ToSafeString(data.image_url);
                account.HeadUrl = DynamicConvert.ToSafeString(data.header_url);
                account.Location = DynamicConvert.ToSafeString(data.location);
                account.Followers = DynamicConvert.ToSafeInt(data.followers);
                account.Friends = DynamicConvert.ToSafeInt(data.friends);
                account.Statuses = DynamicConvert.ToSafeInt(data.statuses);
                account.Favorites = DynamicConvert.ToSafeInt(data.favorites);
                account.MediaCount = DynamicConvert.ToSafeInt(data.media_count);
                account.FriendShipTime = DynamicConvert.ToSafeString(data.friendship_time);
                account.DataState = EnumDataState.Normal;
            }
            return account;
        }

    }

    /// <summary>
    /// 推文类型
    /// </summary>
    public enum TwitterStatusesType
    {
        /// <summary>
        /// 未知
        /// </summary>
        None,
        /// <summary>
        /// 主推文
        /// </summary>
        Master,
        /// <summary>
        /// 回复推文
        /// </summary>
        Reply,
    }

    /// <summary>
    /// 推文信息
    /// </summary>
    [Serializable]
    public class TwitterStatuses : AbstractTwitterEntity
    {
        [Display]
        public string StatusesId { get; set; }

        [Display]
        public string AuthorDesc
        {
            get { return string.Format("{0}({1})", AuthorName, AuthorId); }
        }

        public string AuthorId { get; set; }

        public string AuthorName { get; set; }

        [Display]
        public string Content { get; set; }

        [Display]
        public string SourceDesc { get; set; }

        [Display]
        public DateTime? CreateTime { get; set; }

        [Display(ColumnType = EnumColumnType.Enum)]
        public TwitterStatusesType TwitterStatusesType { get; set; }

        [Display]
        public string InRUserDesc
        {
            get { return InRUserId.IsValid() && InRStatusId != "0" && InRStatusId != "-1" ? string.Format("{0}({1})", InRUserName, InRUserId) : ""; }
        }

        public string InRUserId { get; set; }

        public string InRUserName { get; set; }

        [Display]
        public string InRStatusDesc
        {
            get { return InRStatusId.IsValid() && InRStatusId != "0" && InRStatusId != "-1" ? InRStatusId : ""; }
        }

        public string InRStatusId { get; set; }

        [Display]
        public string FavoritesDesc
        {
            get { return Favorites == 1 ? LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Status_Yes) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Status_No); }
        }

        public int Favorites { get; set; }

        [Display]
        public string RetweetedDesc
        {
            get { return Retweeted == 1 ? LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Status_Yes) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Status_No); }
        }

        public int Retweeted { get; set; }

        [Display]
        public int FavoriteCount { get; set; }

        [Display]
        public int RetweetCount { get; set; }

        [Display]
        public string SourceUrl { get; set; }

        private static readonly Regex _UrlRegex = new Regex("https://t.co/\\S+$");

        public static TwitterStatuses DyConvert(dynamic data)
        {
            TwitterStatuses statuse = new TwitterStatuses();
            statuse.StatusesId = DynamicConvert.ToSafeString(data.status_id);
            statuse.AuthorId = DynamicConvert.ToSafeString(data.username);
            statuse.AuthorName = DynamicConvert.ToSafeString(data.author_name);
            statuse.Content = DynamicConvert.ToSafeString(data.content);
            statuse.SourceDesc = DynamicConvert.ToSafeString(data.source);
            statuse.CreateTime = DynamicConvert.ToSafeFromUnixTime(data.created, 1000);
            statuse.InRUserId = DynamicConvert.ToSafeString(data.in_r_user_id);
            statuse.InRUserName = DynamicConvert.ToSafeString(data.in_r_user_name);
            statuse.InRStatusId = DynamicConvert.ToSafeString(data.in_r_status_id);
            statuse.Favorites = DynamicConvert.ToSafeInt(data.favorited);
            statuse.Retweeted = DynamicConvert.ToSafeInt(data.retweeted);
            statuse.FavoriteCount = DynamicConvert.ToSafeInt(data.favorite_count);
            statuse.RetweetCount = DynamicConvert.ToSafeInt(data.retweet_count);

            switch (statuse.InRStatusId)
            {
                case "0":
                    statuse.TwitterStatusesType = TwitterStatusesType.Master;
                    break;
                case "-1":
                    statuse.TwitterStatusesType = TwitterStatusesType.None;
                    break;
                default:
                    statuse.TwitterStatusesType = TwitterStatusesType.Reply;
                    break;
            }

            var res = _UrlRegex.Match(statuse.Content);
            if (res.Success)
            {
                statuse.SourceUrl = res.Value;
                statuse.Content = statuse.Content.TrimEnd(statuse.SourceUrl.ToCharArray());
            }

            statuse.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(data.XLY_DataType, EnumDataState.Normal);

            return statuse;
        }
    }

    /// <summary>
    /// 群聊
    /// </summary>
    [Serializable]
    public class TwitterGroupConverstation : AbstractTwitterEntity
    {
        [Display]
        public string ConversationId { get; set; }

        [Display]
        public string GroupName { get; set; }

        [Display]
        public int GroupMembersCount { get; set; }

        [Display]
        public string GroupMembers { get; set; }

    }

    /// <summary>
    /// 聊天内容
    /// </summary>
    [Serializable]
    public class TwitterConverstationEntry : AbstractTwitterEntity
    {
        [Display]
        public string UserID { get; set; }

        [Display]
        public string UserName { get; set; }

        [Display]
        public string NickName { get; set; }

        [Display]
        public DateTime? CreateTime { get; set; }

        [Display]
        public string Context { get; set; }

        //[Display(Text = "CreateTimeDesc")]
        //public string CreateTimeDesc { get; set; }

        [Display]
        public string ContextType { get; set; }

        [Display]
        public string EntryTypeDesc
        {
            get { return "0" == EntryType ? (IsSend ? LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Message_Send) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Message_Receive)) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_Message_SendFail); }
        }

        public bool IsSend { get; set; }

        public string EntryType { get; set; }


        /// <summary>
        /// 搜索记录
        /// </summary>
        [Serializable]
        public class TwitterSearchEntry : AbstractTwitterEntity
        {

            [Display]
            public DateTime? SearchTime { get; set; }

            [Display]
            public string Context { get; set; }

            [Display]
            public string ResultDesc
            {
                get { return ResultType == "0" ? LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_ResultType_YouXiaoShuJu) : LanguageHelperSingle.Instance.GetLanguageByKey(Languagekeys.OtherLanguage_ResultType_WuXiaoShuJu); }
            }

            public string ResultType { get; set; }

        }

        /// <summary>
        /// 查询列表
        /// </summary>
        [Serializable]
        public class TwitterViewEntry : AbstractTwitterEntity
        {
            public string ID { get; set; }

            [Display]
            public string OwnerID { get; set; }

            [Display]
            public string OwnerName { get; set; }

            [Display]
            public string Title { get; set; }

            [Display]
            public string Subtitle { get; set; }

        }

        /// <summary>
        /// 瞬间
        /// </summary>
        [Serializable]
        public class TwitterMomentEntry : AbstractTwitterEntity
        {
            public string ID { get; set; }

            [Display]
            public string Title { get; set; }

            [Display]
            public string Subcategory { get; set; }

            [Display]
            public string SubcategoryUrl { get; set; }

            [Display]
            public string Description { get; set; }

            [Display]
            public string MomentUrl { get; set; }

            [Display]
            public DateTime? Time { get; set; }

        }
    }
}

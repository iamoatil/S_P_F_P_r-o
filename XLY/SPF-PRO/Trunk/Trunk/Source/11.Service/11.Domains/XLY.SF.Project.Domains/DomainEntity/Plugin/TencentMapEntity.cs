using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    [Serializable]
    public class TencentMapEntity
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// 经度
        /// </summary>
        public double Lon
        {
            get;
            set;
        }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Lat
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 腾讯地图相关抽象类 
    /// </summary>
    [Serializable]
    public abstract class AbstractTencentMapEntity : AbstractDataItem
    {
    }

    /// <summary>
    /// 腾讯地图帐号信息
    /// </summary>
    [Serializable]
    public class TencentMapAccount : AbstractTencentMapEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Display]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [Display]
        public string NickName
        {
            get;
            set;
        }

        [Display]
        public string LastLoginTimeDesc
        {
            get { return null != LastLoginTime ? LastLoginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""; }
        }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime
        {
            get;
            set;
        }

        [Display]
        public string LastLoginCityDesc
        {
            get { return LastLoginCity.ToString(); }
        }

        /// <summary>
        /// 最后登录城市
        /// </summary>
        public TencentMapEntity LastLoginCity
        {
            get;
            set;
        }

        public TencentMapAccount()
        {
            LastLoginCity = new TencentMapEntity();
        }
    }

    /// <summary>
    /// 腾讯地图搜索地点
    /// </summary>
    [Serializable]
    public class TencentMapSearchAddress : AbstractTencentMapEntity
    {
        [Display]
        public string Key
        {
            get;
            set;
        }

        [Display]
        public string ResultDesc
        {
            get { return ResultAddress.ToString(); }
        }

        /// <summary>
        /// 搜索结果
        /// </summary>
        public TencentMapEntity ResultAddress
        {
            get;
            set;
        }

        [Display]
        public string LastTimeDesc
        {
            get { return null != LastTime ? LastTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""; }
        }

        /// <summary>
        /// 最后搜索时间
        /// </summary>
        public DateTime? LastTime
        {
            get;
            set;
        }

        [Display]
        public int SearchCount
        {
            get;
            set;
        }

        public TencentMapSearchAddress()
        {
            ResultAddress = new TencentMapEntity();
        }
    }

    /// <summary>
    /// 腾讯地图搜索路线
    /// </summary>
    [Serializable]
    public class TencentMapSearchRoute : AbstractTencentMapEntity
    {
        [Display]
        public string Key
        {
            get;
            set;
        }

        [Display]
        public string RouteFromDesc
        {
            get { return RouteFrom.ToString(); }
        }

        /// <summary>
        /// 搜索起点
        /// </summary>
        public TencentMapEntity RouteFrom
        {
            get;
            set;
        }

        [Display]
        public string RouteToDesc
        {
            get { return RouteTo.ToString(); }
        }

        /// <summary>
        /// 搜索终点
        /// </summary>
        public TencentMapEntity RouteTo
        {
            get;
            set;
        }

        [Display]
        public string LastTimeDesc
        {
            get { return null != LastTime ? LastTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""; }
        }

        /// <summary>
        /// 最后搜索时间
        /// </summary>
        public DateTime? LastTime
        {
            get;
            set;
        }

        [Display]
        public int SearchCount
        {
            get;
            set;
        }

        public TencentMapSearchRoute()
        {
            RouteFrom = new TencentMapEntity();
            RouteTo = new TencentMapEntity();
        }

    }

    /// <summary>
    /// 腾讯地图收藏地点
    /// </summary>
    [Serializable]
    public class TencentMapFavoriteAddr : AbstractTencentMapEntity
    {
        [Display]
        public string Name
        {
            get;
            set;
        }

        [Display]
        public string TimeDesc
        {
            get { return null != Time ? Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; }
        }

        public DateTime? Time
        {
            get;
            set;
        }

        /// <summary>
        /// 经度
        /// </summary>
        [Display]
        public double Lon
        {
            get;
            set;
        }

        /// <summary>
        /// 纬度
        /// </summary>
        [Display]
        public double Lat
        {
            get;
            set;
        }

        [Display]
        public string Tag
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 腾讯地图收藏路线
    /// </summary>
    [Serializable]
    public class TencentMapFavoriteRoute : AbstractTencentMapEntity
    {
        [Display]
        public string Name
        {
            get;
            set;
        }

        [Display]
        public string From
        {
            get;
            set;
        }

        [Display]
        public string To
        {
            get;
            set;
        }

        [Display]
        public string RouteType
        {
            get;
            set;
        }

        [Display]
        public string TakeTime
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 腾讯地图收藏街景
    /// </summary>
    [Serializable]
    public class TencentMapFavoriteStreet : AbstractTencentMapEntity
    {
        [Display]
        public string Name
        {
            get;
            set;
        }

        [Display]
        public string TimeDesc
        {
            get { return null != Time ? Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; }
        }

        public DateTime? Time
        {
            get;
            set;
        }


        [Display]
        public string FileInfo
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 腾讯地图同行聊天消息
    /// </summary>
    [Serializable]
    public class TencentMapTongxingMsg : AbstractTencentMapEntity
    {
        [Display]
        public string GroupId
        {
            get;
            set;
        }


        [Display]
        public string Sender
        {
            get;
            set;
        }

        [Display]
        public string TimeDesc
        {
            get { return null != Time ? Time.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; }
        }

        public DateTime? Time
        {
            get;
            set;
        }

        [Display]
        public string Msg
        {
            get;
            set;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：IMap  
* Author     ：Fhjun
* Create Date：2017/3/24 14:36:53
* ==============================================================================*/

namespace XLY.SF.Project.Domains.Contract.DataItemContract
{
    /// <summary>
    /// 地图接口
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// 经度
        /// </summary>
        double Longitude { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        double Latitude { get; set; }

        /// <summary>
        /// 时间，可空
        /// </summary>
        DateTime? Date { get; set; }

        /// <summary>
        /// 地点描述
        /// </summary>
        string Desc { get; set; }
    }
}

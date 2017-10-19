using System;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 经纬度坐标点
    /// </summary>
    [Serializable]
    public class MPoint
    {
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

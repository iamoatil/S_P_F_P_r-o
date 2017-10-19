using System;

namespace XLY.XDD.ForensicService
{
    /// <summary>
    /// 基本常用公共变量（静态）数据定义
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 常用图片文件扩展名。
        /// 用于缩略图、图片提取等。
        /// </summary>
        public readonly static string[] ImageExtensions = new string[] { "jpg", "jpeg", "jpe", "bmp", "tif", "tiff", "png", "gif", "emf", "ico", "wmf", "psd","pdd","dwg" };

        /// <summary>
        /// 常用视频文件扩展名。
        /// 用户缩略图、视频提取等
        /// </summary>
        public readonly static string[] VedioExtensions = new string[] { "avi", "mkv", "mp4", "rmvb", "mpeg", "mpg", "m4v", "mov", "3gp", "rm", "flv", "wmv", "asf", "navi",  "f4v", "webm" };

        /// <summary>
        /// 常用音频文件扩展名。
        /// 用户缩略图、音频提取等
        /// </summary>
        public readonly static string[] AudioExtensions = new string[] { "m4a", "mpeg-4", "mp3", "wma", "wav", "ape", "acc", "ogg", "amr", "3ga" };
    }
}
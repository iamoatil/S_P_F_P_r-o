using System;
using System.Text;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 抽象的分区结构信息
    /// </summary>
    public class Partition
    {
        /// <summary>
        ///Name对应的界面描述
        /// </summary>
        /// <value>The name.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets the mount point name
        /// 名称
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the mount point block
        /// 块区
        /// </summary>
        /// <value>The block.</value>
        public string Block { get; set; }

        /// <summary>
        /// 大小，字节数
        /// </summary>
        public long Size { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Text).Append("[").Append(this.Name).Append("]");
            if (this.Size > 0)
            {
                sb.Append("(").Append(GetFileSize(this.Size)).Append(")");
            }
            return sb.ToString();
        }
        #region 获取文件大小描述（参数：文件大小，字节数）

        /// <summary>
        /// 获取文件大小描述（参数：文件大小，字节数）
        /// </summary>
        public static string GetFileSize(long len, string format = "F2")
        {
            if (len <= 0)
            {
                return "0 KB";
            }

            string unit = " B";
            double res = len, rule = 1024D;
            //KB
            if (len >= rule)
            {
                res = len / rule;
                unit = " KB";
            }
            //M
            if (res > rule)
            {
                res = res / rule;
                unit = " MB";
            }
            //G
            if (res > rule)
            {
                res = res / rule;
                unit = " GB";
            }
            //去掉多余的0
            if (res - Math.Truncate(res) == 0)
            {
                return string.Concat(res.ToString("F2"), unit);
            }
            return string.Concat(res.ToString("F2"), unit);
        }

        /// <summary>
        /// 获取文件大小（参数：文件大小，字节数）
        /// </summary>
        public static string GetFileSize(int len)
        {
            return GetFileSize((long)len);
        }
        #endregion
    }
}
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 数据泵/提取对象
    /// </summary>
    public class Pump
    {
        /// <summary>
        /// 操作系统类型。
        /// </summary>
        public EnumOSType OSType { get; set; }

        /// <summary>
        /// 提取数据的源设备,根据提取方式的不同而不同
        /// 可能是手机(USB提取)、镜像文件（镜像提取）、SD卡（SD卡提取）、本地文件夹路径（文件夹提取）等等
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// 提取方式
        /// </summary>
        public EnumPump Type { get; set; }

        /// <summary>
        /// 扫描模式(镜像文件、SDCard)
        /// </summary>
        public ScanFileModel ScanModel { get; set; }
    }
}
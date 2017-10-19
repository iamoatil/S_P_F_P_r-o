namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// 镜像数据模型
    /// </summary>
    public class Mirror
    {
        /// <summary>
        /// 镜像类型
        /// </summary>
        public EnumMirror Type { get; set; }

        /// <summary>
        /// 目标路径
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 目标镜像文件名
        /// </summary>
        public string TargetFile { get; set; }

        /// <summary>
        /// 本地镜像文件路径
        /// </summary>
        public string Local { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 镜像源
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// 镜像制定的块区
        /// </summary>
        public Partition Block { get; set; }

        /// <summary>
        /// 镜像标记
        /// </summary>
        public MirrorFlag MirrorFlag { get; set; }
    }
}
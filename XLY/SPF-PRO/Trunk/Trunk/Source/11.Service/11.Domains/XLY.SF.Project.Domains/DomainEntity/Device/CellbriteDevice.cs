using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Domains
{
    /// <summary>
    /// UFED镜像设备
    /// </summary>
    public class CellbriteDevice : IFileSystemDevice
    {
        #region Constructors

        public CellbriteDevice(string source)
        {
            this.Source = source;
            this.Name = System.IO.Path.GetFileName(source);
            IniConfig config = new IniConfig(source);
            var fileName = config.IniReadValue("Dumps", "Image0");
            var files = config.ReadSectionKeys(fileName);
            var dir = FileHelper.GetFilePath(source);
            this.Mirrors = files.Where(o => o.Key != "PartsCount").Select(file => System.IO.Path.Combine(dir, file.Value)).ToList();
            this.Mirrors.ForEach(f => this.Size += (ulong)new System.IO.FileInfo(f).Length);
        }

        #endregion

        #region Properties
        /// <summary>
        /// 文件列表（镜像）
        /// </summary>
        public List<string> Mirrors { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 芯片数据源
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// 当前设备句柄
        /// </summary>
        public IntPtr Handle { get; set; }

        /// <summary>
        /// 当前装载设备句柄
        /// </summary>
        public IntPtr Handle_Flsh { get; set; }

        private ArrayList _parts;

        /// <summary>
        /// 分区列表
        /// </summary>
        public ArrayList Parts
        {
            get { return _parts ?? (_parts = new ArrayList()); }
        }

        /// <summary>
        /// 扫描模式
        /// </summary>
        public byte ScanModel { get; set; }

        /// <summary>
        /// 磁盘序列号
        /// </summary>
        public byte DiskNumber { get; set; }

        /// <summary>
        /// 磁盘大小
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// 总扇区
        /// </summary>
        public ulong TotalSectors { get { return this.Size > 0 ? this.Size / 512 : 0; } }

        /// <summary>
        /// 是否存在分区
        /// </summary>
        public bool HasPartition
        {
            get { return this.Parts.Count != 0; }
        }

        /// <summary>
        /// 芯片类型：
        /// 0xA000 联发科手机芯片(mtk)
        /// 0xA001 展讯手机芯片
        /// 0xA002 诺机亚手机芯片
        /// 0xA003 WindowsPhone手机芯片
        /// 0xA004 晨星芯片(MStar)
        /// </summary>
        public FlshType FlshType { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DevType DevType { get; set; }

        #endregion
    }
}

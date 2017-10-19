using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    /// <summary>
    /// 分区信息接收器
    /// </summary>
    public class MountPointReceiver : AbstractOutputReceiver
    {
        /// <summary>
        /// The mount parsing pattern.
        /// </summary>
        private const String RE_MOUNTPOINT_PATTERN = @"^([\S]+)\s+([\S]+)\s+([\S]+)\s+(r[wo]).*$";

        public List<MountPoint> Mounts { get; private set; }

        public override void DoResolver()
        {
            Mounts = new List<MountPoint>();
            foreach (var line in Lines)
            {
                Match m = Regex.Match(line, RE_MOUNTPOINT_PATTERN, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                if (m.Success)
                {
                    String block = m.Groups[1].Value.Trim().Replace("//", "/");
                    String name = m.Groups[2].Value.Trim();
                    String fs = m.Groups[3].Value.Trim();
                    bool ro = String.Compare("ro", m.Groups[4].Value.Trim(), false) == 0;
                    MountPoint mnt = new MountPoint(block, name, fs, ro);
                    Mounts.Add(mnt);
                }
            }
        }

        /// <summary>
        /// 解析分区（连接文件）路径。
        /// </summary>
        /// <param name="d"></param>
        public void ResolveLinkFiles(Device d)
        {
            if (Mounts.IsInvalid()) return;
            foreach (var m in Mounts)
            {
                if (m.Name == ConstCodeHelper.PARTITION_CACHE || m.Name == ConstCodeHelper.PARTITION_DATA ||
                    m.Name.EndsWith(ConstCodeHelper.PARTITION_SDCARD) || m.Name == ConstCodeHelper.PARTITION_SYSTEM)
                {
                    var fs = AndroidHelper.Instance.FindFiles(d, m.Block);
                    if (fs.IsInvalid()) continue;
                    var f = fs[0];
                    if (f.LinkPath.IsValid())
                    {
                        m.Block = f.LinkPath;
                    }
                }
            }
        }
    }
}
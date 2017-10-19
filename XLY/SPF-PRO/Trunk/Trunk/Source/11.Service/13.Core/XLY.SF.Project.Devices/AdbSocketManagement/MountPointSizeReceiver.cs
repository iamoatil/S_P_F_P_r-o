using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    /// <summary>
    /// 分区大小处理接收器
    /// </summary>
    public class MountPointSizeReceiver : AbstractOutputReceiver
    {
        public List<MountPoint> Mounts { get; set; }

        private List<Parti> _Partis;

        #region MountPointSizeReceiver-构造函数（初始化）

        /// <summary>
        ///  MountPointSizeReceiver-构造函数（初始化）
        /// </summary>
        public MountPointSizeReceiver(List<MountPoint> mounts)
        {
            Mounts = mounts;
        }

        #endregion

        public override void DoResolver()
        {
            _Partis = new List<Parti>();
            foreach (var line in Lines)
            {
                var values = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.IsInvalid() || values.Length < 4)
                {
                    continue;
                }
                var p = new Parti();
                p.Major = values[0].ToSafeInt();
                p.Minor = values[1].ToSafeInt();
                p.Size = values[2].ToSafeInt64() * 1024;
                p.Name = values[3];
                _Partis.Add(p);

                //setting size by name.  ex: 179        2     778240 mmcblk0p2  :  /dev/block/mmcblk0p3 /data ext4 
                var ms = Mounts.Where(s => s.BlockName == p.Name);
                if (ms.IsInvalid())
                {
                    //by major and minor.  ex: 179        1   12341248 mmcblk0p1 :   /dev/block/vold/179:1 
                    var mmname = string.Concat(p.Major, ":", p.Minor);
                    ms = Mounts.Where(s => s.BlockName == mmname);
                }

                if (ms.IsValid())
                {
                    var m = ms.First();
                    m.Size = p.Size;
                    m.Block = FileHelper.ConnectLinuxPath(FileHelper.GetLinuxFilePath(m.Block), p.Name);
                }
            }
        }

        /// <summary>
        /// 解析可用分区
        /// </summary>
        /// <returns></returns>
        public List<MountPoint> ResolverUsableMounts()
        {
            List<MountPoint> mps = new List<MountPoint>();
            if (Mounts.IsInvalid() || _Partis.IsInvalid()) return mps;
            //all存储区，最大的分区设为all存储区
            //var p = this._Partis.OrderBy(s => s.Size).Last();
            //var mall = new MountPoint();
            //mall.Name = p.Name;
            //mall.Size = p.Size;

            //有效分区
            var ms = Mounts.Where(m => m.Size > 0).Where(m => m.Name == ConstCodeHelper.PARTITION_CACHE || m.Name == ConstCodeHelper.PARTITION_DATA ||
                        m.Name.Contains(ConstCodeHelper.PARTITION_SDCARD) || m.Name == ConstCodeHelper.PARTITION_SYSTEM);
            if (ms.IsValid())
            {
                //mall.Block = System.Utility.Helper.File.ConnectLinuxPath(System.Utility.Helper.File.GetLinuxFilePath(ms.First().Block), mall.Name);
                //mall.Name = ConstCodeHelper.PARTITION_All;
                mps.AddRange(ms);
            }
            //处理sdcard分区路径，使用data分区的路径
            var dm = Mounts.FirstOrDefault(m => m.Name == ConstCodeHelper.PARTITION_DATA);
            var dsd = Mounts.FirstOrDefault(m => m.Name.Contains(ConstCodeHelper.PARTITION_SDCARD));
            if (dm != null && dsd != null)
            {
                var pm = FileHelper.GetLinuxFilePath(dm.Block);
                dsd.Block = FileHelper.ConnectLinuxPath(pm, dsd.BlockName);
            }

            ////如果all存储区不是最大的，则不添加all存储区
            //if (mps.Sum(s => s.Size) <= mall.Size)
            //{
            //    mps.Add(mall);
            //}

            //原来判断all存储区方法（最大的分区设为all存储区）不正确，改为由system和data两个区的相同路径来判断
            try
            {
                var partData = Mounts.Where(m => m.Size > 0).FirstOrDefault(m => m.Name == ConstCodeHelper.PARTITION_DATA);
                var partSystem = Mounts.Where(m => m.Size > 0).FirstOrDefault(m => m.Name == ConstCodeHelper.PARTITION_SYSTEM);
                if (partSystem != null && partData != null)
                {
                    string allblockname = "";
                    if (partSystem.BlockName.StartsWith("sd"))
                    {
                        allblockname = partSystem.BlockName.Substring(0, 3);
                    }
                    else
                    {
                        int pindex = partSystem.BlockName.LastIndexOf('p');  //比如/system为mmcblk0p12，/data为mmcblk0p23，需要找出mmcblk0
                        allblockname = partSystem.BlockName.Substring(0, pindex);
                    }
                    if (allblockname.IsValid() &&
                       partData.BlockName.StartsWith(allblockname))
                    {
                        var p = _Partis.FirstOrDefault(x => x.Name == allblockname);
                        var mall = new MountPoint();
                        mall.Block = FileHelper.ConnectLinuxPath(FileHelper.GetLinuxFilePath(ms.First().Block), allblockname);
                        mall.Name = ConstCodeHelper.PARTITION_All;
                        mall.Size = p.Size;
                        mps.Add(mall);
                    }
                }
            }
            catch
            {
            }

            return mps;
        }


        internal struct Parti
        {
            public int Major;
            public int Minor;
            public long Size;
            public string Name;
        }
    }
}

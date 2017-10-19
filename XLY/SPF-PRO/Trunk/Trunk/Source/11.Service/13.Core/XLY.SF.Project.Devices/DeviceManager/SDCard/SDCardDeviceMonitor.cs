/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/7 15:17:59 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using X64Service;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Devices.AdbSocketManagement;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// SD卡设备监控
    /// </summary>
    public sealed class SDCardDeviceMonitor : AbstractDeviceMonitor
    {
        /// <summary>
        /// 监控定时器
        /// </summary>
        private Timer MonitorTimer { get; set; }

        /// <summary>
        /// 执行监控处理的标示，true正在执行监控处理。
        /// </summary>
        private bool IsMonitoring { get; set; } = false;

        /// <summary>
        /// 当前连接的设备
        /// </summary>
        private List<SDCardDevice> CurConnectDevs { get; set; }

        public SDCardDeviceMonitor()
        {
            CurConnectDevs = new List<SDCardDevice>();
        }

        public override bool Start()
        {
            if (null != MonitorTimer)
            {
                MonitorTimer.Stop();
            }
            MonitorTimer = new Timer();
            MonitorTimer.Interval = ConstCodeHelper.MONITOR_INTERVAL;
            MonitorTimer.Elapsed += MonitorTimer_Elapsed;
            MonitorTimer.Start();

            return true;
        }

        private void MonitorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsMonitoring)
            {
                return;
            }
            IsMonitoring = true;
            DoOneDeviceMonitor();
            IsMonitoring = false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void DoOneDeviceMonitor()
        {
            //1.获取当前连接列表
            var devices = new List<SDCardDevice>();

            var bottomdiskinfo = new DISK_INFO();
            for (byte index = 0; index < 16; index++)
            {
                var result = FileServiceCoreDll.GetDiskInfo(ref bottomdiskinfo, index);
                if (result != 0 || !bottomdiskinfo.controller.Contains("USB driver"))
                {
                    continue;
                }
                var device = new SDCardDevice();
                device.DiskNumber = bottomdiskinfo.disk_num;
                device.Name = string.Format("Disk:{0}-{1}", index, bottomdiskinfo.sModelNumber);
                device.Size = (ulong)bottomdiskinfo.sectors.ToSafeInt64() * 512;
                devices.Add(device);
            }

            //2.获取新增设备和断开连接设备
            var newConnecnted = devices.Except(CurConnectDevs).ToList();
            var disConnecteds = CurConnectDevs.Except(devices).ToList();

            foreach (var add in newConnecnted)
            {
                OnConnected(add);
                CurConnectDevs.Add(add);
            }

            foreach (var dis in disConnecteds)
            {
                OnDisconnected(dis);
                CurConnectDevs.Remove(dis);
            }
        }

        public override void Close()
        {
            if (null != MonitorTimer)
            {
                MonitorTimer.Stop();
                MonitorTimer = null;
            }
        }

    }
}

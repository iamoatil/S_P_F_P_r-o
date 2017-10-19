/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/9/11 10:57:45 
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
using XLY.SF.Project.Devices.AdbSocketManagement;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// 黑莓设备监控
    /// </summary>
    public sealed class BlackBerryDeviceMonitor : AbstractDeviceMonitor
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
        private List<Device> CurConnectDevs { get; set; }

        public BlackBerryDeviceMonitor()
        {
            CurConnectDevs = new List<Device>();
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
            var service = DllClient.X86DLLClientSingle.Instance.BlackBerryDeviceAPIChannel;

            //1.获取当前连接列表
            var list = service.BlackBerry_FindDevices();

            //2.获取新增设备和断开连接设备
            var newConnecnted = list.Where(s => !CurConnectDevs.Any(d => d.ID == s.pinStr));
            var disConnecteds = CurConnectDevs.Where(d => !list.Any(s => s.pinStr == d.ID));

            foreach (var phone in newConnecnted)
            {
                var device = new Device(phone.pinStr);
                device.Name = phone.modelStr;
                device.Manufacture = "BlackBerry";
                device.Model = phone.modelStr;
                device.OSType = EnumOSType.BlackBerry;
                string[] split_data = phone.softVersion.Split('.');
                if (split_data.Length == 8)
                {
                    StringBuilder sbVersion = new StringBuilder();
                    for (int j = 4; j < 8; j++)
                    {
                        sbVersion.Append(string.IsNullOrEmpty(sbVersion.ToString()) ? split_data[j] : "." + split_data[j]);
                    }
                    device.OSVersion = sbVersion.ToString();
                }
                device.Status = EnumDeviceStatus.Online;
                device.IsRoot = false;
                device.IMEI = phone.imei;// "357256.04.163074.7";
                device.ID = phone.pinStr;

                OnConnected(device);
                CurConnectDevs.Add(device);
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

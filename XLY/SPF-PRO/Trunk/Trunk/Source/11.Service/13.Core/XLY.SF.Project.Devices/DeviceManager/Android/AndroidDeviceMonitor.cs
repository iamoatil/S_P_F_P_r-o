using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Devices.AdbSocketManagement;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Devices
{
    /// <summary>
    /// 安卓手机设备监控
    /// </summary>
    public sealed class AndroidDeviceMonitor : AbstractDeviceMonitor
    {
        #region *********************************************只读字段（仅内部使用）

        /// <summary>
        /// 设备列表正则
        /// </summary>
        private readonly string Re_DeviceList_Info = @"^([a-z0-9-_&#@\?]+(?:\s[a-z0-9\?]+)?)\s+(device|offline|unknown|bootloader|recovery|download)$";

        #endregion

        /// <summary>
        /// Android服务（ADB命令传输）
        /// </summary>
        private AndroidHelper AndroidDeviceHelper { get; set; }

        /// <summary>
        /// 当前连接的Android设备
        /// </summary>
        private List<Device> CurConnectDevs { get; set; }

        /// <summary>
        /// 监控定时器
        /// </summary>
        private Timer MonitorTimer { get; set; }

        /// <summary>
        /// 执行监控处理的标示，true正在执行监控处理。
        /// </summary>
        private bool IsMonitoring { get; set; } = false;

        //由于USB监控无法检测手机授权的情况，故放弃使用
        //private UsbMonitorService UsbMonitor { get; set; }

        public AndroidDeviceMonitor()
        {
            AndroidDeviceHelper = AndroidHelper.Instance;
            CurConnectDevs = new List<Device>();
        }

        public override bool Start()
        {
            DoSelfPretection();

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

        public override void Close()
        {
            if (null != MonitorTimer)
            {
                MonitorTimer.Stop();
                MonitorTimer = null;
            }
        }

        public override void DoSelfPretection()
        {
            AndroidDeviceHelper.StopADB();
            AndroidDeviceHelper.StartADB();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void DoOneDeviceMonitor()
        {
            //1.获取当前连接列表
            List<Device> devices = null;

            int iRestartAttemptCount = 0;
            do
            {
                devices = FindCurConnectDevices();

                iRestartAttemptCount++;
                if (null == devices && iRestartAttemptCount > ConstCodeHelper.MONITOR_ATTEMPTCOUNT)
                {//如果多次查找都返回null 说明adb连接失败 考虑重启ADB服务
                    AndroidDeviceHelper.StopADB();
                    AndroidDeviceHelper.StartADB();
                    return;
                }
            } while (null == devices);

            //2.获取新增设备和断开连接设备
            var sameds = CurConnectDevs.Where(s => devices.Any(sd => sd.Equals(s))).ToList();
            var newConnecnted = devices.Except(CurConnectDevs).ToList();
            var disConnecteds = CurConnectDevs.Except(devices).ToList();

            foreach (var same in sameds)
            {
                //如果基本属性获取不成功，则重试一次
                if (same.Model == null || same.Manufacture == null)
                {
                    UpdateNewDevice(same);
                }
            }

            foreach (var add in newConnecnted)
            {
                UpdateNewDevice(add);

                OnConnected(add);
                CurConnectDevs.Add(add);
            }

            foreach (var dis in disConnecteds)
            {
                OnDisconnected(dis);
                CurConnectDevs.Remove(dis);
            }
        }

        /// <summary>
        /// 查找当前设备列表 如果返回null 说明adb连接失败
        /// </summary>
        /// <returns>如果返回null 说明adb连接失败</returns>
        private List<Device> FindCurConnectDevices()
        {
            List<Device> result = new List<Device>();

            using (AdbSocketOperator _androidManagementSocket = new AdbSocketOperator())
            {
                if (!_androidManagementSocket.IsOpened)
                {
                    return null;
                }
                try
                {
                    var request = AdbSocketHelper.CmdToBytes("host:devices");
                    _androidManagementSocket.Write(request);
                    var res = _androidManagementSocket.ReadResponse();
                    if (res.IsOkay)
                    {
                        var len = _androidManagementSocket.ReadDataLength(4);
                        var data = new byte[len];
                        _androidManagementSocket.Read(data);
                        string devString = Encoding.UTF8.GetString(data);

                        if (!string.IsNullOrWhiteSpace(devString))
                        {
                            string[] devices = devString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            Regex re = new Regex(Re_DeviceList_Info, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            foreach (var ds in devices)
                            {
                                Match m = re.Match(ds);
                                if (!m.Success)
                                {
                                    LoggerManagerSingle.Instance.Error(string.Format("无法识别的设备信息：{0}", ds));
                                    continue;
                                }
                                Device d = new Device(m.Groups[1].Value);
                                var st = m.Groups[2].Value;
                                //找到的设备都为在线状态
                                if (st == "device")
                                {
                                    d.Status = EnumDeviceStatus.Online;
                                    result.Add(d);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LoggerManagerSingle.Instance.Error(e);
                }
            }

            return result;
        }

        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="device"></param>
        private void UpdateNewDevice(Device device)
        {
            var pros = AndroidDeviceHelper.GetProperteis(device);

            device.OSType = EnumOSType.Android;
            device.DeviceManager = AndroidDeviceManager.Instance;

            if (pros.IsValid())
            {
                device.Properties = pros;
                device.SerialNumber = device.GetProperty("ro.serialno").Trim();
                device.Manufacture = device.GetProperty("ro.product.manufacturer") ?? "unknow";
                device.Brand = device.GetProperty("ro.product.brand") ?? "unknow";
                device.Model = device.GetProperty("ro.product.model") ?? "unknow";
                device.Name = device.GetProperty("ro.product.name") ?? "unknow";
                device.OSVersion = device.GetProperty("ro.build.version.release") ?? "unknow";
            }

            //获取手机的IMEI号，不同手机Key不一样，且当它为空时，取MEID号代替。
            if (device.IMEI.IsInvalid())
            {
                AndroidDeviceHelper.GetIMEINumber(device);
            }
            if (device.IMSI.IsInvalid())
            {
                AndroidDeviceHelper.GetIMSINumber(device);
            }

            //获取设备su命令和ls命令格式 以及是否root的信息
            device.SU = "su -c \"{0}\" ";
            device.LS = "ls -l \"{0}\"";

            device.IsRoot = AndroidDeviceHelper.CanSU(device);

            if (!device.IsRoot)
            {
                device.SU = "su -c {0} ";
                device.IsRoot = AndroidDeviceHelper.CanSU(device);

                if (!device.IsRoot)
                {
                    device.SU = "{0}";
                    device.IsRoot = AndroidDeviceHelper.CanSU(device);
                    if (!device.IsRoot)
                    {
                        device.SU = "su -c \"{0}\" ";
                    }
                }
            }

            //获取设备的SD卡路径
            device.SDCardPath = AndroidDeviceHelper.GetSDCardPath(device);

        }

    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Devices.AdbSocketManagement;
using XLY.SF.Project.Domains;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.BaseUtility.Helper;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/4/7 11:43:19
 * 类功能说明：
 * 1. 提供ADB命令服务
 *
 *************************************************/

namespace XLY.SF.Project.Devices
{
    public sealed class AndroidHelper
    {
        #region 只读字段（仅内部使用）

        /// <summary>
        /// 属性匹配正则
        /// </summary>
        public const string GETPROP_PATTERN = "^\\[([^]]+)\\]\\:\\s*\\[(.*)\\]$";

        /// <summary>
        /// Gets or Sets the adb location on the OS.
        /// </summary>
        /// <value>The adb location on the OS.</value>
        public static readonly string AdbOsLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"adb\adb.exe");

        #endregion

        public static AndroidHelper Instance
        {
            get { return SingleWrapperHelper<AndroidHelper>.Instance; }
        }

        [Obsolete("请使用AndroidHelper.Instance获取实例！")]
        public AndroidHelper()
        {
            HostAddress = IPAddress.Loopback;
            SocketAddress = new IPEndPoint(HostAddress, ConstCodeHelper.ADB_PORT);
        }

        #region 属性定义


        #endregion

        #region Adb调用

        #region StartADB（启动adb）

        /// <summary>
        /// 启动adb
        /// </summary>
        public void StartADB()
        {
            int status = -1;
            try
            {
                String command = "start-server";
                ProcessStartInfo psi = new ProcessStartInfo(AdbOsLocation, command);
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;

                using (Process proc = Process.Start(psi))
                {
                    //DoAdbHelper(proc.Id);  //修改adb启动端口号
                    proc.WaitForExit();
                    status = proc.ExitCode;
                }
            }
            catch (IOException ioe)
            {
                LoggerManagerSingle.Instance.Error(ioe, "adb启动失败");
            }
            catch (ThreadInterruptedException ie)
            {
                LoggerManagerSingle.Instance.Error(ie, "adb启动失败");
            }
            catch (Exception e)
            {
                LoggerManagerSingle.Instance.Error(e, "adb启动失败");
            }

            if (status != 0)
            {
                LoggerManagerSingle.Instance.Error("adb启动失败, ExitCode:" + status);
            }
        }

        #endregion

        #region StopADB（关闭adb）

        /// <summary>
        /// 关闭adb
        /// </summary>
        public void StopADB()
        {
            int status = -1;
            try
            {
                String command = "kill-server";
                ProcessStartInfo psi = new ProcessStartInfo(AdbOsLocation, command);
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                using (Process proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                    status = proc.ExitCode;
                }
                //再补一刀
                //System.Utility.Helper.Sys.KillProcess(ConstCodeHelper.ADB);
            }
            catch (IOException ie)
            {
                LoggerManagerSingle.Instance.Error(ie, "终止adb失败");
            }
            catch (Exception e)
            {
                LoggerManagerSingle.Instance.Error(e, "终止adb失败");
            }

            if (status != 0)
            {
                LoggerManagerSingle.Instance.Error("终止adb失败, ExitCode:" + status);
            }

            //此处，要歇会儿
            Thread.Sleep(1000);
        }

        #endregion

        #region 连接手机

        /// <summary>
        /// 连接指定手机设备
        /// </summary>
        /// <param name="curSocket">连接使用的Socket</param>
        /// <param name="curDev">要连接的设备</param>
        /// <returns></returns>
        private bool ConnectDevice(AdbSocketOperator curSocket, Device curDev)
        {
            string msg = "host:transport:" + curDev.ID;
            byte[] device_query = AdbSocketHelper.CmdToBytes(msg);
            curSocket.Write(device_query);
            var res = curSocket.ReadResponse();
            return res.IsOkay;
        }

        #endregion

        #region 获取设备属性

        /// <summary>
        /// 获取设备属性信息
        /// </summary>
        public Dictionary<string, string> GetProperteis(Device device)
        {
            AdbSocketOperator adbOperator = new AdbSocketOperator();
            //连接手机
            if (ConnectDevice(adbOperator, device) &&
                ExecuteAdbCommand("getprop", device, adbOperator))
            {
                if (adbOperator.ReadResponse().IsOkay)
                {
                    var devInfo = Encoding.Default.GetString(adbOperator.ReadToEnd());
                    return ConvertDevProperty(devInfo);
                }
            }
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// 执行数据解析
        /// </summary>
        public Dictionary<string, string> ConvertDevProperty(string devInfo)
        {
            var Properties = new Dictionary<string, string>();
            var infoLines = devInfo.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in infoLines)
            {
                if (line.StartsWith("#") || line.StartsWith("$"))
                {
                    continue;
                }
                var m = Regex.Match(line, GETPROP_PATTERN, RegexOptions.Compiled);
                if (m.Success)
                {
                    String label = m.Groups[1].Value.Trim();
                    String value = m.Groups[2].Value.Trim();
                    if (label.IsValid() && !Properties.ContainsKey(label))
                    {
                        Properties.Add(label, value);
                    }
                }
            }
            return Properties;
        }

        #endregion

        #endregion

        #region 执行Adb命令

        /// <summary>
        /// 执行ADB命令
        /// </summary>
        /// <param name="cmd">ADB指令</param>
        /// <param name="dev">执行的设备</param>
        /// <param name="socketOperator">AdbSocket</param>
        private bool ExecuteAdbCommand(string cmd, Device dev, AdbSocketOperator socketOperator)
        {
            if (!string.IsNullOrWhiteSpace(cmd) && dev != null && socketOperator != null)
            {
                //连接成功
                var request = AdbSocketHelper.CmdToBytesByShell(cmd);
                return socketOperator.Write(request);
            }
            return false;
        }

        #endregion

        #region InstallPackage/UnInstallPackage

        /// <summary>
        /// 安装本地制定apk文件到手机，强制安装。
        /// 默认安装到data/local/tmp/
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="device">目标设备</param>
        /// <param name="remoutePath">设备文件路径</param>
        /// <returns></returns>
        public bool InstallPackage(string localFile, Device device, string remotePath = "/data/local/tmp/")
        {
            string remouteFile = string.Empty;
            var flag = false;
            try
            {
                remouteFile = PushSingleFile(device, localFile, remotePath);
                if (String.IsNullOrWhiteSpace(remouteFile))
                    return false;
                //1.1 要提升文件权限
                UpgradeFilePermission(device, remouteFile);
                //2. 安装APK
                var cmd = string.Format("pm install -r {0}", remouteFile);
                DefaultReceiver receiver = new DefaultReceiver();
                ExecuteRemoteCommand("pm setInstallLocation 0", device, receiver);
                ExecuteRemoteCommand(cmd, device, receiver, 16 * 1024, 20000);
                if (Contains(receiver.Data, "success", StringComparison.OrdinalIgnoreCase))
                {
                    flag = true;
                }
                else
                {
                    throw new ApplicationException(receiver.Data);
                }
            }
            catch 
            {
                flag = false;
                //如果默认路径安装失败，则尝试拷贝到SDCard安装
                if (remotePath.Equals("/data/local/tmp/"))
                {
                    flag = InstallPackage(localFile, device, remotePath = TryParseSDCardByDict(device));
                }
            }
            finally
            {
                try
                {
                    var receiver = new DefaultReceiver();
                    ExecuteRemoteCommand(string.Format("rm {0}", remouteFile), device, receiver);
                }
                catch { }
            }
            return flag;
        }

        /// <summary>
        /// 卸载指定包名的App，如com.mwh.spfsocket
        /// </summary>
        /// <param name="packageName">APK安装包名称</param>
        /// <param name="device">目标设备</param>
        public void UninstallPackage(string packageName, Device device)
        {
            try
            {
                var cmd = string.Format("pm uninstall {0}", packageName);
                DefaultReceiver receiver = new DefaultReceiver();
                ExecuteRemoteCommand(cmd, device, receiver);
            }
            catch
            {

            }
        }
        #endregion

        #region 其他

        public IPAddress HostAddress { get; private set; }

        public IPEndPoint SocketAddress { get; private set; }

        public bool Contains(string value, string match, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return value.IndexOf(match, comparison) > -1;
        }
        #endregion

        #region 提升文件操作权限
        /// <summary>
        /// 提升文件操作权限
        /// recursion=true 支持递归所有子文件、文件夹，但有些手机（三星）不支持该参数
        /// recursion=false 只操作当前指定的文件/文件夹路径
        /// </summary>
        /// <param name="path">文件路径</param>
        public bool UpgradeFilePermission(Device device, string path, bool recursion = false)
        {
            try
            {
                //若文件名中包含空格
                path = path.Replace(" ", "\\ ");
                DefaultReceiver receiver = new DefaultReceiver();
                string cmd = recursion ? "chmod -R 777 {0}" : "chmod 777 {0}";
                var result = ExecuteRemoteAutoCommandNoException(string.Format(cmd, path), device, receiver);
                if (!result.Success && null != result.Ex)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 拷贝本地文件到设备临时目录

        /// <summary>
        /// 拷贝本地文件到设备临时目录(/data/local/tmp/)
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="localFile">本地文件路径</param>
        /// <param name="remotePath">设备文件路径</param>
        /// <param name="bufferSize">缓冲区大小</param>
        /// <param name="maxtimeout">超时时间</param>
        /// <returns></returns>
        public string PushSingleFile(Device device, string localFile, string remotePath,
            int bufferSize = 64 * 1024,
            int maxtimeout = 9000)
        {
            if (!System.IO.File.Exists(localFile))
            {
                LoggerManagerSingle.Instance.Error("File not exist:" + localFile);
                return string.Empty;
            }
            //验证目标目录
            if (!RemotFileIsExist(device, remotePath))
            {
                DefaultReceiver reciver = new DefaultReceiver();
                ExecuteRemoteAutoCommand(string.Format("mkdir -p {0}", remotePath), device, reciver);
            }
            UpgradeFilePermission(device, remotePath);

            var remoutFile = FileHelper.ConnectLinuxPath(remotePath, Path.GetFileName(localFile));
            try
            {
                if (DoPushFile(device, localFile, remoutFile, bufferSize, maxtimeout))
                    return remoutFile;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "push file Error:" + localFile);
            }
            return string.Empty;
        }
        #endregion

        #region 拷贝文件到设备
        /// <summary>
        /// 拷贝文件到设备
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="localFile">本地文件路径</param>
        /// <param name="remoutFile">设备文件路径</param>
        /// <param name="bufferSize">缓冲区大小</param>
        /// <param name="maxtimeout">超时时间</param>
        /// <returns></returns>
        private bool DoPushFile(Device device, string localFile, string remoutFile, int bufferSize = 64 * 1024, int maxtimeout = 9000)
        {
            FileStream fs = null;
            byte[] remoutFileBytes = Encoding.UTF8.GetBytes(remoutFile);
            fs = new FileStream(localFile, FileMode.Open, FileAccess.Read);
            //socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(SocketAddress);
                socket.ReceiveTimeout = maxtimeout;
                socket.SendTimeout = maxtimeout;
                socket.Blocking = true;
                socket.ReceiveBufferSize = bufferSize;
                SetDevice(socket, device);
                SendAsyncReqest(socket);
                var request = AdbSocketHelper.CreateSendFileRequest(Encoding.UTF8.GetBytes("SEND"), remoutFileBytes);
                AdbSocketHelper.Write(socket, request);
                //send data
                var buf = new byte[bufferSize + 8];
                var cdata = Encoding.UTF8.GetBytes("DATA");
                Array.Copy(cdata, 0, buf, 0, cdata.Length);
                int readindex = 1;
                while (readindex > 0)
                {
                    readindex = fs.Read(buf, 8, bufferSize);
                    readindex.Swap32bitsToArray(buf, 4);
                    AdbSocketHelper.Write(socket, buf, readindex + 8);
                }
                // create the DONE message
                long time = 1447655524;
                var msg = AdbSocketHelper.CreateRequest(Encoding.UTF8.GetBytes("DONE"),
                    (int)time);
                AdbSocketHelper.Write(socket, msg);
                var result = new byte[8];
                AdbSocketHelper.Read(socket, result);
                if (!AdbSocketHelper.CheckResult(result, Encoding.UTF8.GetBytes("OKAY")))
                {
                    throw new Exception("Push file:RESULT_UNKNOWN_ERROR");
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "push file error:" + localFile);
            }
            finally
            {
                fs.Dispose();
                DisposeSocket(socket);
            }
            return false;
        }


        /// <summary>
        /// 设置当前使用的设备
        /// Sets the device.
        /// </summary>
        private void SetDevice(Socket adbChan, Device device)
        {
            String msg = "host:transport:" + device.ID;
            byte[] device_query = AdbSocketHelper.CmdToBytes(msg);
            AdbSocketHelper.Write(adbChan, device_query);
            var res = AdbSocketHelper.ReadResponse(adbChan);
            if (!res.IsOkay)
            {
                if (String.Compare("device not found", res.Data, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    throw new ApplicationException(string.Format("I'm so sorry,device[{0}] not found", device.ID));
                }
                else
                {
                    throw new ApplicationException("device (" + device + ") request rejected: " + res.Data);
                }
            }
        }

        /// <summary>
        /// 执行文件拷贝钱，发生异步通知，测试
        /// </summary>
        /// <param name="socket">端口</param>
        private void SendAsyncReqest(Socket socket)
        {
            byte[] request = AdbSocketHelper.CmdToBytes("sync:");
            AdbSocketHelper.Write(socket, request);
            var res = AdbSocketHelper.ReadResponse(socket);
            if (!res.IsOkay)
            {
                throw new ApplicationException("socket response error:" + res.Data);
            }
        }

        #endregion

        #region 验证一个目录或文件是已存在
        /// <summary>
        /// 验证一个目录或文件是已存在
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="remouteFile">设备文件路径</param>
        internal bool RemotFileIsExist(Device device, string remouteFile)
        {
            try
            {
                DefaultReceiver receiver = new DefaultReceiver();
                //若文件名中包含空格
                var path = remouteFile.Replace(" ", "\\ ");
                ExecuteRemoteAutoCommand(string.Format("ls -l {0}", path), device, receiver);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        #endregion

        #region 释放接口
        /// <summary>
        /// 释放接口
        /// </summary>
        /// <param name="socket">端口</param>
        private void DisposeSocket(Socket socket)
        {
            if (socket != null)
            {
                socket.Close();
                socket.Dispose();
            }
        }
        #endregion

        #region CanSU

        /// <summary>
        /// 检测设备是否root
        /// </summary>
        public bool CanSU(Device device)
        {
            try
            {
                DefaultReceiver receiver = new DefaultReceiver();

                ExecuteRemoteRootCommand(ConstInstructionSet.CAN_SU, device, receiver, maxtimeout: ConstCodeHelper.CAN_SU_DEFAULT_TIMEOUT);

                if (!receiver.Lines.IsInvalid() &&
                    (receiver.Data.IsMatch(@"app_parts")
                    || receiver.Data.IsMatch(@"databases")
                    || receiver.Data.IsMatch(@"lib")
                    || receiver.Data.IsMatch(@"shared_prefs")
                    || receiver.Data.IsMatch(@"Unknown id:data/data/com.android.providers.telephony/")))
                {
                    return true;
                }
                else
                {
                    ExecuteRemoteRootCommand(ConstInstructionSet.CAN_SU2, device, receiver, maxtimeout: ConstCodeHelper.CAN_SU_DEFAULT_TIMEOUT);
                    if (!receiver.Lines.IsInvalid() &&
                      (receiver.Data.IsMatch(@"app_parts")
                      || receiver.Data.IsMatch(@"databases")
                      || receiver.Data.IsMatch(@"lib")
                      || receiver.Data.IsMatch(@"shared_prefs")
                      || receiver.Data.IsMatch(@"Unknown id:data/data/com.android.providers.contacts/")))
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (PermissionDeniedException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region GetApps

        /// <summary>
        /// 获取设备安装的应用列表
        /// </summary>
        public List<AppEntity> GetApps(Device device)
        {
            try
            {
                APPReceiver receiver = new APPReceiver();
                ExecuteRemoteAutoCommand(ConstInstructionSet.PACKAGE, device, receiver);
                return receiver.APPs;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
            return null;
        }

        #endregion

        #region GetMountPoints

        /// <summary>
        /// 获取设备分区信息
        /// </summary>
        public List<MountPoint> GetMountPoints(Device device)
        {
            try
            {
                if (device.Status == EnumDeviceStatus.Recovery)
                {//Recovery模式手机必须先执行mount -a命令才可以获取到分区
                    device.RealStatus = EnumDeviceStatus.Recovery;
                    try
                    {
                        MountPointReceiver mpr = new MountPointReceiver();
                        ExecuteRemoteAutoCommand("mount -a", device, mpr);
                    }
                    catch
                    {
                    }
                }

                //1.分区信息
                MountPointReceiver receiver = new MountPointReceiver();
                ExecuteRemoteAutoCommand(ConstInstructionSet.MOUNT, device, receiver);
                receiver.ResolveLinkFiles(device);
                var ms = receiver.Mounts;
                //分区大小
                MountPointSizeReceiver sizeReceiver = new MountPointSizeReceiver(ms);
                ExecuteRemoteAutoCommand(ConstInstructionSet.PARTITON, device, sizeReceiver);
                return sizeReceiver.ResolverUsableMounts();
            }
            catch
            {
            }
            return null;
        }

        #endregion

        #region GetSDCardPath

        /// <summary>
        /// 获取指定设备的SDCard路径，会消耗几秒钟
        /// </summary>
        public string GetSDCardPath(Device device)
        {
            var path = TryParseSDCardByDict(device);
            if (path.IsValid())
            {
                return path;
            }
            else
            {
                return TryParseSDCardByDB(device);
            }
        }

        public static readonly string[] SDCardPathDictionary = new[] { "/mnt/sdcard/", "/storage/sdcard0/", "/mnt/shell/emulated/0/" };

        /// <summary>
        /// 通过路径字典尝试获取SDCard路径
        /// </summary>
        private string TryParseSDCardByDict(Device device)
        {
            foreach (var p in SDCardPathDictionary)
            {
                DefaultReceiver receiver = new DefaultReceiver();
                try
                {
                    ExecuteRemoteCommand(string.Format("ls -l {0}", p), device, receiver);
                }
                catch { continue; }
                if (receiver.Lines != null) return p;
            }
            return string.Empty;
        }

        /// <summary>
        /// 通过媒体文件DB获取SDCard路径
        /// </summary>
        private string TryParseSDCardByDB(Device device)
        {
            var source = "/data/data/com.android.providers.media/databases/#F";
            string local = string.Empty;
            try
            {
                string temp = Environment.GetEnvironmentVariable("TEMP");
                local = CopyFile(device, source, temp, null);
                //2.查找DCIM文件目录-获取SDCard路径
                var db = Path.Combine(local, "external.db");
                if (!File.Exists(db))
                    return string.Empty;
                SqliteContext con = new SqliteContext(db);
                var res = con.Find(new SQLiteString("select * from files where _data like '%DCIM'"));
                if (res.IsInvalid()) return string.Empty;
                string data = res.First().xly_data.ToString();
                var sdcard = data.TrimEnd("DCIM");
                //3.LS指令获取真实路径（是否链接文件）
                var fs = FindFiles(device, sdcard);
                //3.1如果第一次没找到，去掉末尾的0再来一次
                if (fs.IsInvalid())
                {
                    sdcard = sdcard.TrimEnd("0/");
                    fs = FindFiles(device, sdcard);
                }
                if (fs.IsInvalid()) return sdcard;
                var file = fs.First();
                if (file.LinkPath.IsValid()) sdcard = file.LinkPath;
                return string.Concat(sdcard.TrimEnd("/"), "/");
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        #region GetIMEINumber

        /// <summary>
        /// 获取设备唯一ID（IMEI）
        /// </summary>
        /// <param name="device"></param>
        public void GetIMEINumber(Device device)
        {
            try
            {
                var receiver = new DefaultReceiver();
                ExecuteRemoteAutoCommand("dumpsys iphonesubinfo", device, receiver);
                var res = receiver.Data.Replace("\r\n", ";").Split(';');

                if (res.Length >= 3)
                {
                    var phoneType = res[1].Split('=')[1].Trim();
                    var deviceId = res[2].Split('=')[1].Trim();

                    if (phoneType == "CDMA")
                    {
                        device.IMEI = deviceId.Substring(0, 14);
                    }
                    else
                    {
                        device.IMEI = deviceId;
                    }
                }
            }
            catch (Exception exception)
            {
#if DEBUG
                LoggerManagerSingle.Instance.Error(exception, "");
#endif
            }

            if (device.IMEI.IsValid() || device.Properties.IsInvalid()) return;
            string imei = "IMEI";
            string meid = "MEID";

            string meidKey = device.Properties.Keys.FirstOrDefault(key => key.Contains(meid, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(meidKey))
            {
                var v = device.Properties[meidKey];
                var len = v.Length > 14 ? 14 : 0;
                device.IMEI = v.Substring(0, len);
                return;
            }

            foreach (var imeiKey in device.Properties.Keys.Where(key => key.Contains(imei, StringComparison.OrdinalIgnoreCase)))
            {
                var v = device.Properties[imeiKey];
                if (v.Length > 14)
                {
                    device.IMEI = v.Substring(0, 14);
                    return;
                }
            }
        }

        #endregion

        #region GetIMSINumber

        public void GetIMSINumber(Device device)
        {
            if (device.Properties.IsInvalid())
            {
                return;
            }

            foreach (var imsiKey in device.Properties.Keys.Where(key => key.Contains("imsi", StringComparison.OrdinalIgnoreCase)))
            {
                var v = device.Properties[imsiKey];
                if (v.Length > 14)
                {
                    device.IMSI = v.Substring(0, 14);
                    return;
                }
            }
        }

        #endregion

        #region ExecuteRemoteCommandResult
        public class ExecuteRemoteCommandResult
        {
            public bool Success;
            public Exception Ex;

            public ExecuteRemoteCommandResult(Exception ex = null)
            {
                Success = false;
                Ex = ex;
            }

            public ExecuteRemoteCommandResult(bool success, Exception ex = null)
            {
                Success = success;
                Ex = ex;
            }

        }
        #endregion

        #region ExecuteRemoteRootCommand

        /// <summary>
        /// 执行shell的Root指令
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="device">目标设备</param>
        /// <param name="maxtimeout">最大超时时间</param>
        public void ExecuteRemoteRootCommand(string command, Device device, AbstractOutputReceiver receiver,
            int buffersize = ConstCodeHelper.DEFAULT_COMMAND_BUFFER_SIZE, int maxtimeout = ConstCodeHelper.DEFAULT_TIMEOUT)
        {
            ExecuteRemoteCommand(string.Format(device.SU, command), device, receiver, buffersize, maxtimeout);
        }

        #endregion

        #region ExecuteRemoteCommand

        /// <summary>
        /// 执行shell命令，返回string
        /// </summary>
        /// <param name="command">ADB 命令</param>
        /// <param name="device">目标设备</param>
        /// <param name="receiver">接收器</param>
        /// <param name="buffersize">缓冲区大小</param>
        /// <param name="maxtimeout">超时时间</param>
        /// <returns>返回执行后的结果</returns>
        /// 
        private string ExecuteRemoteCommandString(string command, Device device, AbstractOutputReceiver receiver,
             int buffersize = ConstCodeHelper.DEFAULT_COMMAND_BUFFER_SIZE, int maxtimeout = ConstCodeHelper.DEFAULT_TIMEOUT)
        {
            if (String.IsNullOrWhiteSpace(command) || device == null)
            {
                throw new ArgumentException("The command or device is invalid");
            }

            using (AdbSocketOperator adbOperator = new AdbSocketOperator())
            {
                if (ConnectDevice(adbOperator, device) && ExecuteAdbCommand(command, device, adbOperator))
                {
                    adbOperator.Read(receiver, buffersize);

                    return receiver.Data.ToString().Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 执行shell指令
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="device">目标设备</param>
        /// <param name="maxtimeout">最大超时时间</param>
        public void ExecuteRemoteCommand(string command, Device device,
            AbstractOutputReceiver receiver,
            int buffersize = 16 * 1024,
            int maxtimeout = 9000)
        {
            if (String.IsNullOrWhiteSpace(command) || device == null)
            {
                throw new ArgumentException("The command or device is invalid");
            }

            string[] cmd = command.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var tdata = ExecuteRemoteCommandString(command, device, receiver, buffersize, maxtimeout);

            if (tdata.Contains(String.Format("{0}: not found", cmd[0])))
            {
                throw new FileNotFoundException(string.Format("The remote execution returned: '{0}: not found'", cmd[0]));
            }
            if (tdata.Contains("No such file or directory"))
            {
                throw new FileNotFoundException(String.Format("The remote execution returned: {0}", tdata));
            }
            if (tdata.Contains("Operation not permitted"))
            {
                throw new ApplicationException(tdata);
            }
            if (tdata.Contains("Unknown option"))
            {
                throw new ApplicationException(tdata);
            }

            if (tdata.Contains("Starting service: ")
                && tdata.Contains("java.lang.SecurityException:")
                && tdata.Contains("is not privileged to communicate"))
            {
                throw new ApplicationException(tdata);
            }
            if (BaseTypeExtension.IsMatch(tdata, "Aborting.$"))
            {
                throw new ApplicationException(tdata);
            }
            if (BaseTypeExtension.IsMatch(tdata, "applet not found$") && cmd.Length > 1)
            {
                throw new FileNotFoundException(string.Format("The remote execution returned: '{0}'", tdata));
            }
            if (BaseTypeExtension.IsMatch(tdata, "(permission|access) denied$"))
            {
                throw new PermissionDeniedException();
            }
        }

        /// <summary>
        /// 执行shell的指令，自动判断指令方式。若root，则使用root指令。不产生异常
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="device">目标设备</param>
        /// <param name="maxtimeout">最大超时时间</param>
        internal ExecuteRemoteCommandResult ExecuteRemoteAutoCommandNoException(string command,
            Device device, AbstractOutputReceiver receiver,
            int buffersize = 16 * 1024,
            int maxtimeout = 3000)
        {
            if (device.IsRoot && device.RealStatus != EnumDeviceStatus.Recovery)
            {
                return ExecuteRemoteCommandNoException(string.Format("su -c \"{0}\" ", command), device, receiver, buffersize, maxtimeout);
            }
            else
            {
                return ExecuteRemoteCommandNoException(command, device, receiver, buffersize, maxtimeout);
            }
        }

        /// <summary>
        /// 执行shell指令,不产生异常
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="device">目标设备</param>
        /// <param name="maxtimeout">最大超时时间</param>
        public ExecuteRemoteCommandResult ExecuteRemoteCommandNoException(string command, Device device,
            AbstractOutputReceiver receiver,
            int buffersize = 16 * 1024,
            int maxtimeout = 3000)
        {
            if (String.IsNullOrWhiteSpace(command) || device == null)
            {
                return new ExecuteRemoteCommandResult(new ArgumentException("The command or device is invalid"));
            }

            string[] cmd = command.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var tdata = ExecuteRemoteCommandString(command, device, receiver, buffersize, maxtimeout);

            if (tdata.Contains(String.Format("{0}: not found", cmd[0])))
            {
                return new ExecuteRemoteCommandResult(new FileNotFoundException(string.Format("The remote execution returned: '{0}: not found'", cmd[0])));
            }
            if (tdata.Contains("No such file or directory"))
            {
                return new ExecuteRemoteCommandResult(new FileNotFoundException(String.Format("The remote execution returned: {0}", tdata)));
            }
            if (tdata.Contains("Operation not permitted"))
            {
                return new ExecuteRemoteCommandResult(new ApplicationException(tdata));
            }
            if (tdata.Contains("Unknown option"))
            {
                return new ExecuteRemoteCommandResult(new ApplicationException(tdata));
            }
            if (tdata.Contains("Starting service: ")
                && tdata.Contains("java.lang.SecurityException:")
                && tdata.Contains("is not privileged to communicate"))
            {
                return new ExecuteRemoteCommandResult(new ApplicationException(tdata));
            }
            if (BaseTypeExtension.IsMatch(tdata, "Aborting.$"))
            {
                return new ExecuteRemoteCommandResult(new ApplicationException(tdata));
            }
            if (BaseTypeExtension.IsMatch(tdata, "applet not found$") && cmd.Length > 1)
            {
                return new ExecuteRemoteCommandResult(new FileNotFoundException(string.Format("The remote execution returned: '{0}'", tdata)));
            }
            if (BaseTypeExtension.IsMatch(tdata, "(permission|access) denied$"))
            {
                return new ExecuteRemoteCommandResult(new PermissionDeniedException());
            }

            return new ExecuteRemoteCommandResult(true);
        }

        /// <summary>
        /// 执行shell的指令，自动判断指令方式。若root，则使用root指令。
        /// </summary>
        /// <param name="command">指令</param>
        /// <param name="device">目标设备</param>
        /// <param name="maxtimeout">最大超时时间</param>
        internal void ExecuteRemoteAutoCommand(string command, Device device, AbstractOutputReceiver receiver,
            int buffersize = 16 * 1024,
            int maxtimeout = 9000)
        {
            if (device.IsRoot && device.RealStatus != EnumDeviceStatus.Recovery)
            {
                ExecuteRemoteCommand(string.Format("su -c \"{0}\" ", command), device, receiver, buffersize, maxtimeout);
            }
            else
            {
                ExecuteRemoteCommand(command, device, receiver, buffersize, maxtimeout);
            }
        }
        #endregion

        #region ExecuteSPFAppCommand

        private int _ForwardProt = 12580;

        /// <summary>
        /// 执行SPF app指令，并返回结果。
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="command">SPF socket命令</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>返回命令执行结果</returns>
        public string ExecuteSPFAppCommand(Device device, string command = "basic_info", int timeout = ConstCodeHelper.DEFAULT_TIMEOUT)
        {
            // 服务启动提取失败：是否进行过重试
            int doretry = 0;
            DoRetry:

            var port = _ForwardProt++;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                /**
                 * 启动APP:
                 * 目前APP支持两种启动方式，一种为服务命令，一种为直接启动组件
                 * 服务启动方式：优点是用户不需要做任何操作，缺点是启动服务不稳定，针对部分手机启动受权限限制
                 * 组件启动方式：优点是启动服务稳定，缺点是启动组件时屏幕会闪动一下，用户体验比较差
                 * 综合上述两种启动方式，在APP中提供两种启动方式，有限已服务启动，启动失败后再通过组件方式进行启动
                 */
                // 服务启动
                if (!AppServiceStart(device, port, 12345) || doretry == 1 || doretry == 2)
                {
                    // 组件启动
                    AppAmStart(device, port, 12345);
                }

                socket.Connect(IPAddress.Loopback, port);
                socket.Blocking = true;
                socket.ReceiveBufferSize = 16 * 1024;
                socket.SendBufferSize = 16 * 1024;
                socket.ReceiveTimeout = timeout;
                socket.SendTimeout = timeout;
                byte[] request = command.ToBytes(Encoding.UTF8);
                AdbSocketHelper.Write(socket, request);
                var res = AdbSocketHelper.ReadResponse(socket);
                if (!res.IsOkay)
                {
                    // 服务启动提取数据失败后则转变为组件方式启动提取数据
                    if (doretry == 0)
                    {
                        doretry = 1;
                        goto DoRetry;
                    }
                    else
                    {
                        throw new Exception(string.Format("Device rejected command:{0}. Error:{1} ", command, res.Data));
                    }
                }
                var sizebuf = new byte[16];
                socket.Receive(sizebuf);
                int length = Convert.ToInt32(sizebuf.GetString(), 16);
                ;
                var databuf = new byte[length];
                socket.ReceiveBufferSize = length;
                AdbSocketHelper.Read(socket, databuf, length);
                return databuf.GetString(Encoding.UTF8);
            }
            catch (Exception ex)
            {
                if (doretry <= 1)
                {
                    doretry = 2;
                    goto DoRetry;
                }
                else
                {
                    throw new Exception("Receive SPF App Data Error,", ex);
                }
            }
            finally
            {
                DisposeSocket(socket);
                //remove forward
                try
                {
                    var sc2 = String.Format("host-serial:{0}:killforward-all", device.ID);
                    ExecuteAdbSocketCommand(null, sc2);
                }
                catch { }
            }
        }

        /// <summary>
        /// APP服务启动
        /// 目前APP支持两种启动方式，一种为服务命令，一种为直接启动组件
        /// 服务启动方式：优点是用户不需要做任何操作，缺点是启动服务不稳定，针对部分手机启动受权限限制</summary>
        /// 组件启动方式：优点是启动服务稳定，缺点是启动组件时屏幕会闪动一下，用户体验比较差<param name="device">设备对象</param>
        /// 综合上述两种启动方式，在APP中提供两种启动方式，有限已服务启动，启动失败后再通过组件方式进行启动<param name="port">映射端口（第一个：“12580”是由你们自己定义的pc端口，第二个你们不用管，它是手机端的服务端口）</param>
        /// <param name="tcp">默认密码</param>
        /// <returns>是否成功，true成功，反之亦然</returns>
        private bool AppServiceStart(Device device, int port, int tcp = 12345)
        {
            try
            {
                DefaultReceiver dreceiver = new DefaultReceiver();
                string sc1 = "am startservice -n mwh.com.spfsocket/mwh.com.spfsocket.MainService";
                ExecuteRemoteCommand(sc1, device, dreceiver);
                if (device.IsRoot)
                {
                    sc1 = "su -c \"am startservice -n mwh.com.spfsocket/mwh.com.spfsocket.MainService\"";
                    ExecuteRemoteCommand(sc1, device, dreceiver);
                }
                string sc2 = String.Format("host-serial:{0}:forward:tcp:{1};tcp:{2}", device.ID, port, tcp);
                ExecuteAdbSocketCommand(null, sc2);

                // 服务启动失败则使用组件启动
                if (dreceiver.Lines.Contains("Error: Not found; no service started."))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "SPF App服务启动失败");

                // 组件启动
                if (AppAmStart(device, port, 12345))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// APP组件启动
        /// </summary>
        /// <param name="device">设备对象</param>
        /// <param name="port">映射端口（第一个：“12580”是由你们自己定义的pc端口，第二个你们不用管，它是手机端的服务端口）</param>
        /// <param name="tcp">默认密码</param>
        /// <returns>是否成功，true成功，反之亦然</returns>
        private bool AppAmStart(Device device, int port, int tcp = 12345)
        {
            try
            {
                DefaultReceiver dreceiver = new DefaultReceiver();
                string sc1 = "am start mwh.com.spfsocket/.MainActivity";
                string sc2 = "am broadcast -a NotifyServiceStop";
                var sc3 = String.Format("host-serial:{0}:forward:tcp:{1};tcp:{2}", device.ID, port, tcp);
                var sc4 = "am broadcast -a NotifyServiceStart";
                ExecuteRemoteCommand(sc1, device, dreceiver);
                ExecuteRemoteCommand(sc2, device, dreceiver);
                ExecuteAdbSocketCommand(null, sc3);
                ExecuteRemoteCommand(sc4, device, dreceiver);
                return true;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "SPF App组件启动失败");
                return false;
            }
        }

        #endregion

        #region ExecuteAdbSocketCommand

        /// <summary>
        /// 执行ADB指令，可以指定端口。
        /// </summary>
        /// <param name="device">目标设备，device可以为null，若为null则不指定设备</param>
        /// <param name="command">ADB命令</param>
        /// <param name="port">端口</param>
        /// <param name="receiver">结果接收器，可以为空，为null则不处理结果数据</param>
        /// <param name="bufSize">缓冲区大小</param>
        /// <param name="timeOut">超时时间</param>
        public void ExecuteAdbSocketCommand(Device device, string command, int port = 5037, AbstractOutputReceiver receiver = null,
            int bufSize = ConstCodeHelper.DEFAULT_COMMAND_BUFFER_SIZE, int timeOut = ConstCodeHelper.DEFAULT_TIMEOUT)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(IPAddress.Loopback, port);
                socket.Blocking = true;
                socket.ReceiveBufferSize = bufSize;
                socket.SendBufferSize = bufSize;
                socket.ReceiveTimeout = timeOut;
                socket.SendTimeout = timeOut;
                if (device != null) SetDevice(socket, device);
                byte[] request = AdbSocketHelper.CmdToBytes(command);
                AdbSocketHelper.Write(socket, request);
                var res = AdbSocketHelper.ReadResponse(socket);
                if (!res.IsOkay)
                {
                    throw new Exception(string.Format("Device rejected command:{0}. Error:{1} ", command, res.Data));
                }

                if (receiver != null)
                {
                    AdbSocketHelper.Read(socket, receiver, bufSize);
                }
            }
            finally
            {
                DisposeSocket(socket);
            }
        }

        #endregion

        #region BackupAndResolve

        /// <summary>
        /// 备份同时解析文件为zip格式压缩文件（解析后的压缩文件）。
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="localFile">本地文件</param>
        /// <param name="packages">要备份的APP列表</param>
        /// <returns>备份成功返回本地文件，否则返回string.Empty</returns>
        public string BackupAndResolve(Device device, string localFile, IEnumerable<string> packages = null)
        {
            try
            {
                BackupAndResolveReceiver receiver = new BackupAndResolveReceiver(localFile);
                int bufSize = 8096;
                int timeOut = 50000;
                ExecuteAdbSocketCommand(device, packages.IsValid() && packages.Count() <= 20 ? string.Format("backup:{0}", string.Join(" ", packages)) : "backup:-all",
                                        5037, receiver, bufSize, timeOut);
                return localFile;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "BackupAndResolve Error:" + localFile);
            }
            return string.Empty;
        }

        #endregion

        #region CopyFile（拷贝文件（文件夹））

        /// <summary>
        /// 拷贝文件（文件夹）,返回对应的本地文件或文件夹路径
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="sourcePath">设备文件路径</param>
        /// <param name="targetPath">本地全文件路径</param>
        /// /// <param name="asyn">进度信息</param>
        public string CopyFile(Device device, string sourcePath, string targetPath, IAsyncProgress asyn)
        {
            if (!sourcePath.IsValid() || !targetPath.IsValid())
            {
                return string.Empty;
            }

            if (!IsExistsFileOrPath(device, sourcePath))
            {//如果文件或路径不存在，直接返回
                return string.Empty;
            }

            //upgrade permission
            UpgradeFilePermission(device, sourcePath.Replace("#F", ""), true);

            if (!sourcePath.EndsWith("#F"))
            {
                //直接copy单文件的
                return CopySingleFile(device, sourcePath, targetPath, asyn);
            }
            //folder
            sourcePath = sourcePath.Replace("#F", "");
            if (asyn != null)
            {
                asyn.Advance(0, "FindFile" + sourcePath);
            }
            var files = FindFiles(device, sourcePath);
            if (!files.IsValid())
            {
                return string.Empty;
            }
            //copy folder
            var path = FileHelper.ConnectPath(targetPath, FilterLinuxFileName(sourcePath).Replace("/", "\\"));
            FileHelper.CreateDirectory(path);
            CopyFolder(device, files, targetPath, asyn);
            return path;
        }

        #region CopyFolder
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="files">被拷贝的文件集合</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <param name="asyn">进度信息</param>
        private void CopyFolder(Device device, List<LSFile> files, string targetPath, IAsyncProgress asyn)
        {
            if (!files.IsValid())
            {
                return;
            }

            foreach (var file in files)
            {
                if (file.IsFolder)
                {
                    if (!file.HasPermission)
                    {
                        UpgradeFilePermission(device, file.FullPath);
                    }
                    if (asyn != null)
                    {
                        asyn.Advance(0, "FindFile" + file.FullPath);
                    }
                    var fs = FindFiles(device, file.FullPath);
                    CopyFolder(device, fs, targetPath, asyn);
                }
                else if (file.Size > 0)
                {
                    CopySingleFile(device, file, targetPath, asyn);
                }
            }
        }
        #endregion

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="file">被拷贝的文件</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="asyn">进度信息</param>
        /// <returns></returns>
        public string CopySingleFile(Device device, LSFile file, string targetPath, IAsyncProgress asyn)
        {
            if (!file.HasPermission)
            {
                if (!UpgradeFilePermission(device, file.FullPath))
                {
                    return string.Empty;
                }
            }
            var res = CopySingleFile(device, file.FullPath, targetPath, asyn);
            return res;
        }

        /// <summary>
        /// 对文件或文件夹执行ls指令，获取起文件列表信息
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="sourceFile">目标源文件路径</param>
        /// <returns>获取到的文件列表</returns>
        public List<LSFile> FindFiles(Device device, string sourceFile)
        {
            try
            {
                LSFileReceiver receiver = new LSFileReceiver();
                receiver.Source = sourceFile;
                //若文件名中包含空格
                sourceFile = sourceFile.Replace(" ", "\\ ");
                ExecuteRemoteAutoCommand(string.Format(device.LS, sourceFile), device, receiver);
                return receiver.Files;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "扫描文件发生异常" + sourceFile);
                return null;
            }
        }

        /// <summary>
        /// 拷贝单个设备文件到本地路径，返回新的本地路径。
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="sourceFile">目标源文件路径</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="asyn">进度信息</param>
        /// <returns></returns>
        public string CopySingleFile(Device device, string sourceFile, string targetPath, IAsyncProgress asyn)
        {
            string target = string.Empty;
            try
            {
                target = FileHelper.ConnectPath(targetPath, FilterLinuxFileName(sourceFile).Replace(
                                                                    "/", "\\"));
                FileHelper.CreateDirectory(FileHelper.GetFilePath(target));
                DoPullFile(device, sourceFile, target);
                if (asyn != null)
                {
                    asyn.Advance(0, "PullFile" + sourceFile);
                }
                return target;
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(target);
                LoggerManagerSingle.Instance.Error(ex, "重设被拷贝文件失败" + sourceFile);
            }
            return string.Empty;
        }

        #region DoPullFile（执行文件拷贝到本地操作）

        /// <summary>
        /// 执行文件拷贝到本地操作
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="sourceFile">目标源文件路径</param>
        /// <param name="local">本地路径</param>
        /// <param name="receiver">接收器</param>
        /// <param name="bufferSize">缓冲区大小</param>
        /// <param name="maxtimeout">超时时间</param>
        /// <param name="trycat"></param>
        private void DoPullFile(Device device, string sourceFile, string local, PullFileReceiver receiver = null,
                                int bufferSize = 64 * 1024, int maxtimeout = 9000, bool trycat = true)
        {
            if (device == null || !sourceFile.IsValid() || !local.IsValid())
            {
                return;
            }

            #region /data/data/文件处理

            //只处理/data/data/文件夹下文件
            bool IsNeedMoveFile = sourceFile.Replace('\\', '/').StartsWith("/data/data/") && sourceFile.EndsWith(".db");
            var dr = new DefaultReceiver();
            string rmCmd = "";

            if (IsNeedMoveFile)
            {
                string tempSrc = string.Format("/data/local/tmp/{0}", FileHelper.GetFileName(local));
                string cpCmd = string.Format("cp {0} /data/local/tmp", sourceFile);

                ExecuteRemoteAutoCommandNoException(cpCmd, device, dr);//拷贝/data/data/ 到/data/local/tmp

                if (IsExistsFileOrPath(device, tempSrc))//判断是否拷贝成功
                {
                    UpgradeFilePermission(device, tempSrc);//提权
                    sourceFile = tempSrc;
                    rmCmd = string.Format("rm {0}", tempSrc);
                }
            }

            #endregion

            receiver = receiver ?? new PullFileReceiver(local);
            //socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(SocketAddress);
                socket.ReceiveTimeout = maxtimeout;
                socket.SendTimeout = maxtimeout;
                socket.Blocking = true;
                socket.ReceiveBufferSize = bufferSize;
                SetDevice(socket, device);
                SendAsyncReqest(socket);
                //request
                var request = AdbSocketHelper.FormPullFileRequest(sourceFile);
                AdbSocketHelper.Write(socket, request);
                receiver.DoReceive(socket);
            }
            catch (ADBConnectionException cex)
            {
                var mes = string.Format("pull file[{0}] failed:{1}", sourceFile, cex.AllMessage());
                LoggerManagerSingle.Instance.Error(cex, mes);
                if (trycat)
                {
                    //mes = string.Format("try to cat file:{0} ", sourceFile);
                    //LogHelper.Warn(mes);
                    var rec = new CatToFileReceiver(local);
                    rec.OnReceiveData = receiver.OnReceiveData;
                    var rt = CatToFile(device, sourceFile, local, rec);
                    if (rt != string.Empty)
                    {
                        throw new Exception(rt);
                    }
                }
                else
                {
                    throw new ApplicationException(mes, cex);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("pull file[{0}] failed", sourceFile), ex);
            }
            finally
            {
                if (IsNeedMoveFile && rmCmd.IsValid())
                {//删除temp文件
                    try
                    {
                        ExecuteRemoteAutoCommandNoException(rmCmd, device, dr);
                    }
                    catch { }
                }
                DisposeSocket(socket);
            }
        }

        #endregion

        #region CatToFile
        /// <summary>
        /// 尝试读取源文件并写入本地文件中
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="sourceFile">目标源文件路径</param>
        /// <param name="local">本地路径</param>
        /// <param name="receiver">接收器</param>
        /// <returns></returns>
        private string CatToFile(Device device, string sourceFile, string local, CatToFileReceiver receiver = null)
        {
            try
            {
                UpgradeFilePermission(device, sourceFile);
                receiver = receiver ?? new CatToFileReceiver(local);
                ExecuteRemoteAutoCommand(string.Format("cat {0}", sourceFile), device, receiver, 64 * 1024);
                return string.Empty;
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(local);
                string e = string.Format("try cat to file[{0}] failed", sourceFile);
                LoggerManagerSingle.Instance.Error(ex, e);
                return e;
            }
            finally
            {
                if (receiver != null) receiver.Dispose();
            }
        }
        #endregion

        private static readonly char[] InvalidPathChars = new char[] {
                        '"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v',
                        '\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b',
                        '\x001c', '\x001d', '\x001e', '\x001f','*', '?', ':','\\',
                 };
        /// <summary>
        /// 过滤linux文件路径中的非法字符。
        /// </summary>
        public static string FilterLinuxFileName(string filename)
        {
            var index = filename.IndexOfAny(InvalidPathChars);
            if (index >= 0)
            {
                return FilterLinuxFileName(filename.Replace(filename[index], '_'));
            }
            return filename;
        }
        #endregion

        #region ReadFile（直接读取文件内容）

        /// <summary>
        /// 直接读取文件内容
        /// </summary>
        public string ReadFile(Device device, string sourceFile)
        {
            try
            {
                UpgradeFilePermission(device, sourceFile);
                DefaultReceiver receiver = new DefaultReceiver();
                ExecuteRemoteAutoCommand(string.Format(ConstInstructionSet.CAT, sourceFile), device, receiver);
                return receiver.Data;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
            return string.Empty;
        }

        #endregion

        #region IsExistsFileOrPath

        /// <summary>
        /// 判断文件或者路径是否存在
        /// </summary>
        /// <param name="device">目标设备</param>
        /// <param name="path">路径</param>
        /// <returns></returns>
        private bool IsExistsFileOrPath(Device device, string path)
        {
            int buffersize = 16 * 1024;
            int maxtimeout = 9000;
            DefaultReceiver receiver = new DefaultReceiver();

            string command = string.Format(device.LS, path.TrimEnd("#F".ToArray()));
            if (device.IsRoot)
            {
                command = string.Format(device.SU, command);
            }

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(SocketAddress);
                socket.ReceiveTimeout = maxtimeout;
                socket.SendTimeout = maxtimeout;
                socket.ReceiveBufferSize = buffersize;

                //设置当前使用设备
                String msg = "host:transport:" + device.ID;
                byte[] device_query = AdbSocketHelper.CmdToBytes(msg);
                AdbSocketHelper.Write(socket, device_query);
                var res = AdbSocketHelper.ReadResponse(socket);
                if (!res.IsOkay)
                {
                    return false;
                }

                //reqest
                var request = AdbSocketHelper.CmdToBytes("shell:" + command);
                AdbSocketHelper.Write(socket, request);
                //response
                res = AdbSocketHelper.ReadResponse(socket);
                if (!res.IsOkay)
                {
                    return false;
                }
                //read
                AdbSocketHelper.Read(socket, receiver, buffersize);
                //验证输出是否合法
                //determines weahter the output is valid
                string[] cmd = command.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var tdata = receiver.Data.ToSafeString().Trim();
                if (tdata.Contains(String.Format("{0}: not found", cmd[0])))
                {
                    return false;
                }
                if (tdata.Contains("No such file or directory"))
                {
                    return false;
                }
                if (tdata.Contains("no closing quote"))
                {
                    return false;
                }
            }
            finally
            {
                DisposeSocket(socket);
            }

            return true;
        }

        #endregion

        #region ClearScreenLock（清除屏幕锁）

        /// <summary>
        /// 清除屏幕锁
        /// </summary>
        public void ClearScreenLock(Device device)
        {
            DefaultReceiver receiver = new DefaultReceiver();
            foreach (var com in ConstInstructionSet.SCREEN_LOCK_FILES)
            {
                try
                {
                    UpgradeFilePermission(device, com);
                    var nf = string.Format("{0}{1}", com, ConstInstructionSet.SCREEN_LOCK_EXT);
                    ExecuteRemoteAutoCommand(string.Format(ConstInstructionSet.RENAM_EFILE, com, nf), device, receiver);
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex, "execute command error:" + com);
                }
            }
        }

        #endregion

        #region RecoveryScreenLock（恢复屏幕锁）

        /// <summary>
        /// 恢复屏幕锁
        /// </summary>
        public void RecoveryScreenLock(Device device)
        {
            DefaultReceiver receiver = new DefaultReceiver();
            foreach (var com in ConstInstructionSet.SCREEN_LOCK_FILES)
            {
                try
                {
                    var of = string.Format("{0}{1}", com, ConstInstructionSet.SCREEN_LOCK_EXT);
                    ExecuteRemoteAutoCommand(string.Format(ConstInstructionSet.RENAM_EFILE, of, com), device, receiver);
                }
                catch (Exception ex)
                {
                    LoggerManagerSingle.Instance.Error(ex, "execute command error:" + com);
                }
            }
        }

        #endregion

    }
}

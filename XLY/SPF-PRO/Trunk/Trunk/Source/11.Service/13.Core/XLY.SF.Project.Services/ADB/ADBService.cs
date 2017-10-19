using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Domain;
using XLY.SF.Project.Domains.DomainEnum;
using XLY.SF.Project.Services.ADB;
using XLY.SF.Framework.Core.Base;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.Services
{
    public class ADBService
    {
        #region UpgradeFilePermission（提升文件操作权限）

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
                var result = this.ExecuteRemoteAutoCommandNoException(string.Format(cmd, path), device, receiver);
                if (!result.Success && null != result.Ex)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
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
                return this.ExecuteRemoteCommandNoException(string.Format("su -c \"{0}\" ", command), device, receiver, buffersize, maxtimeout);
            }
            else
            {
                return this.ExecuteRemoteCommandNoException(command, device, receiver, buffersize, maxtimeout);
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
            var tdata = this.ExecuteRemoteCommandString(command, device, receiver, buffersize, maxtimeout);

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
            if (IsMatch(tdata,"Aborting.$"))
            {
                return new ExecuteRemoteCommandResult(new ApplicationException(tdata));
            }
            if (IsMatch(tdata,"applet not found$") && cmd.Length > 1)
            {
                return new ExecuteRemoteCommandResult(new FileNotFoundException(string.Format("The remote execution returned: '{0}'", tdata)));
            }
            if (IsMatch(tdata,"(permission|access) denied$"))
            {
                return new ExecuteRemoteCommandResult(new PermissionDeniedException());
            }

            return new ExecuteRemoteCommandResult(true);
        }

        /// <summary>
        ///  正则验证字符是否符合规则（默认单行模式，并忽略大小写），返回true，符合规则，否则不符合。
        /// </summary>
        public bool IsMatch(string source, String pattern)
        {
            if (String.IsNullOrWhiteSpace(source))
                return false;
            return IsMatch(source, pattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public bool IsMatch(string source, String pattern, RegexOptions options)
        {
            if (String.IsNullOrWhiteSpace(source))
                return false;
            return Regex.IsMatch(source, pattern, options);
        }

        public IPEndPoint SocketAddress { get; private set; }

        /// <summary>
        /// 执行shell命令，返回string
        /// </summary>
        /// <param name="command"></param>
        /// <param name="device"></param>
        /// <param name="receiver"></param>
        /// <param name="buffersize"></param>
        /// <param name="maxtimeout"></param>
        /// <returns></returns>
        /// 
        private string ExecuteRemoteCommandString(string command, Device device, 
            AbstractOutputReceiver receiver, 
            int buffersize = 16 * 1024,
            int maxtimeout = 3000)
        {
            if (String.IsNullOrWhiteSpace(command) || device == null)
            {
                throw new ArgumentException("The command or device is invalid");
            }
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(this.SocketAddress);
                socket.ReceiveTimeout = maxtimeout;
                socket.SendTimeout = maxtimeout;
                socket.ReceiveBufferSize = buffersize;
                this.SetDevice(socket, device);
                //reqest
                var request = AdbSocketHelper.FormAdbRequest("shell:" + command);
                AdbSocketHelper.Write(socket, request);
                //response
                var res = AdbSocketHelper.ReadResponse(socket);
                if (!res.IsOkay)
                {
                    throw new ApplicationException(string.Format("device[{0}] response error:{1}", device.SerialNumber, res.Data));
                }
                //read
                AdbSocketHelper.Read(socket, receiver, buffersize);
                //验证输出是否合法
                //determines weahter the output is valid
                var tdata = receiver.Data.ToString().Trim();

                return tdata;
            }
            catch (DoneWithReadException de)
            {
                
            }
            catch (SocketException ex)
            {
                throw new ApplicationException(string.Format("The shell command\"{0} \" has become unresponsive! ,max timeout:{1}", command, maxtimeout));
            }
            finally
            {
                this.DisposeSocket(socket);
            }

            return string.Empty;
        }

        private void DisposeSocket(Socket socket)
        {
            if (socket != null)
            {
                socket.Close();
                socket.Dispose();
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

        #region InstallPackage/UnInstallPackage

        /// <summary>
        /// 安装本地制定apk文件到手机，强制安装。
        /// 默认安装到data/local/tmp/
        /// </summary>
        public bool InstallPackage(string localFile, Device device, string remoutePath = "/data/local/tmp/")
        {
            string remouteFile = string.Empty;
            var flag = false;
            try
            {
                remouteFile = this.PushSingleFile(device, localFile, remoutePath);
                if (String.IsNullOrWhiteSpace(remouteFile))
                    return false;
                //1.1 要提升文件权限，这要命，找了半天才知道是这个问题
                this.UpgradeFilePermission(device, remouteFile);
                //2.install
                var cmd = string.Format("pm install -r {0}", remouteFile);
                DefaultReceiver receiver = new DefaultReceiver();
                this.ExecuteRemoteCommand("pm setInstallLocation 0", device, receiver);
                this.ExecuteRemoteCommand(cmd, device, receiver, 16 * 1024, 20000);
                if (Contains(receiver.Data,"success", StringComparison.OrdinalIgnoreCase))
                {
                    flag = true;
                }
                else throw new ApplicationException(receiver.Data);
            }
            catch (Exception ex)
            {
                flag = false;
                //如果默认路径安装失败，则尝试拷贝到SDCard安装
                if (remoutePath.Equals("/data/local/tmp/"))
                {
                    flag = this.InstallPackage(localFile, device, remoutePath = this.TryParseSDCardByDict(device));
                }
            }
            finally
            {
                try
                {
                    var receiver = new DefaultReceiver();
                    this.ExecuteRemoteCommand(string.Format("rm {0}", remouteFile), device, receiver);
                }
                catch { }
            }
            return flag;
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
                    this.ExecuteRemoteCommand(string.Format("ls -l {0}", p), device, receiver);
                }
                catch { continue; }
                if (receiver.Lines!=null) return p;
            }
            return string.Empty;
        }

        public  bool Contains( string value, string match, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return value.IndexOf(match, comparison) > -1;
        }

        #region PushSingleFile

        /// <summary>
        /// 拷贝本地文件到设备临时目录(/data/local/tmp/)
        /// </summary>
        public string PushSingleFile(Device device, string localFile, string remotePath, 
            int bufferSize = 64 * 1024,
            int maxtimeout = 9000)
        {
            if (!System.IO.File.Exists(localFile))
            {
                return string.Empty;
            }
            //验证目标目录
            if (!this.RemoutFileIsExist(device, remotePath))
            {
                DefaultReceiver reciver = new DefaultReceiver();
                this.ExecuteRemoteAutoCommand(string.Format("mkdir -p {0}", remotePath), device, reciver);
            }
            this.UpgradeFilePermission(device, remotePath);

            var remoutFile = ConnectLinuxPath(remotePath, Path.GetFileName(localFile));
            try
            {
                if (this.DoPushFile(device, localFile, remoutFile, bufferSize, maxtimeout))
                    return remoutFile;
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }
        #endregion


        #region DoPushFile
        /// <summary>
        /// 拷贝文件到设备
        /// localFile:本地文件
        /// remoutFile：设备存储文件路径
        /// </summary>
        private bool DoPushFile(Device device, string localFile, string remoutFile,
            int bufferSize = 64 * 1024, 
            int maxtimeout = 9000)
        {
            FileStream fs = null;
            byte[] remoutFileBytes = Encoding.UTF8.GetBytes(remoutFile);
            fs = new FileStream(localFile, FileMode.Open, FileAccess.Read);
            //socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(this.SocketAddress);
                socket.ReceiveTimeout = maxtimeout;
                socket.SendTimeout = maxtimeout;
                socket.Blocking = true;
                socket.ReceiveBufferSize = bufferSize;
                this.SetDevice(socket, device);
                this.SendAsyncReqest(socket);
                var request =
                    AdbSocketHelper.CreateSendFileRequest(
                        Encoding.UTF8.GetBytes("SEND"), remoutFileBytes);
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
                if (!AdbSocketHelper.CheckResult(result,Encoding.UTF8.GetBytes("OKEY")))
                {
                    throw new Exception("Push file:RESULT_UNKNOWN_ERROR");
                }
                return true;
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                fs.Dispose();
                this.DisposeSocket(socket);
            }
            return false;
        }

        /// <summary>
        /// 设置当前使用的设备
        /// Sets the device.
        /// </summary>
        private void SetDevice(Socket adbChan, Device device)
        {
            String msg = "host:transport:" + device.SerialNumber;
            byte[] device_query = AdbSocketHelper.FormAdbRequest(msg);
            AdbSocketHelper.Write(adbChan, device_query);
            var res = AdbSocketHelper.ReadResponse(adbChan);
            if (!res.IsOkay)
            {
                if (String.Compare("device not found", res.Data, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    throw new ApplicationException(string.Format("I'm so sorry,device[{0}] not found", device.SerialNumber));
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
        /// <param name="socket"></param>
        private void SendAsyncReqest(Socket socket)
        {
            byte[] request = AdbSocketHelper.FormAdbRequest("sync:");
            AdbSocketHelper.Write(socket, request);
            var res = AdbSocketHelper.ReadResponse(socket);
            if (!res.IsOkay)
            {
                throw new ApplicationException("socket response error:" + res.Data);
            }
        }

        #endregion
        public static string ConnectLinuxPath(string path1, string path2)
        {
            return ConnectPath('/', path1, path2);
        }

        public static string ConnectPath(char separate, params string[] path)
        {
            if (path==null)
                return string.Empty;
            if (path.Length == 2)
                return string.Format("{0}{1}{2}", path[0].TrimEnd(separate), separate, path[1].TrimStart(separate));
            if (path.Length == 1)
                return path[0];
            StringBuilder sb = new StringBuilder(32);
            foreach (var p in path)
            {
                sb.Append(p.TrimEnd(separate).TrimStart(separate)).Append(separate);
            }
            return sb.ToString().TrimEnd(separate);
        }

        /// <summary>
        /// 验证一个目录或文件是已存在
        /// </summary>
        /// <param name="device"></param>
        /// <param name="remouteFile"></param>
        internal bool RemoutFileIsExist(Device device, string remouteFile)
        {
            try
            {
                DefaultReceiver receiver = new DefaultReceiver();
                //若文件名中包含空格
                var path = remouteFile.Replace(" ", "\\ ");
                this.ExecuteRemoteAutoCommand(string.Format("ls -l {0}", path), device, receiver);
                return true;
            }
            catch (FileNotFoundException ex)
            {
                return false;
            }
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

        /// <summary>
        /// 卸载指定包名的App，如com.mwh.spfsocket
        /// </summary>
        /// <param name="packageName"></param>
        public void UninstallPackage(string packageName, Device device)
        {
            try
            {
                var cmd = string.Format("pm uninstall {0}", packageName);
                DefaultReceiver receiver = new DefaultReceiver();
                this.ExecuteRemoteCommand(cmd, device, receiver);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region ExecuteRemoteCommand
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
            var tdata = this.ExecuteRemoteCommandString(command, device, receiver, buffersize, maxtimeout);

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
            if (IsMatch(tdata,"Aborting.$"))
            {
                throw new ApplicationException(tdata);
            }
            if (IsMatch(tdata,"applet not found$") && cmd.Length > 1)
            {
                throw new FileNotFoundException(string.Format("The remote execution returned: '{0}'", tdata));
            }
            if (IsMatch(tdata,"(permission|access) denied$"))
            {
                throw new PermissionDeniedException();
            }
        }
#endregion

        #region ExecuteSPFAppCommand

        private int _ForwardProt = 12580;

        /// <summary>
        /// 执行SPF app指令，并返回结果。
        /// </summary>
        /// <returns></returns>
        public string ExecuteSPFAppCommand(Device device, string command = "basic_info", int timeout = 12000)
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
                byte[] request = Encoding.UTF8.GetBytes(command);
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
                int length = Convert.ToInt32(sizebuf.GetString(Encoding.UTF8), 16);
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
                    throw new Exception("ReceiveSPFAppData Error,", ex);
                }
            }
            finally
            {
                this.DisposeSocket(socket);
                //remove forward
                try
                {
                    var sc2 = String.Format("host-serial:{0}:killforward-all", device.SerialNumber);
                    this.ExecuteAdbSocketCommand(null, sc2);
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
                this.ExecuteRemoteCommand(sc1, device, dreceiver);
                if (device.IsRoot)
                {
                    sc1 = "su -c \"am startservice -n mwh.com.spfsocket/mwh.com.spfsocket.MainService\"";
                    this.ExecuteRemoteCommand(sc1, device, dreceiver);
                }
                string sc2 = String.Format("host-serial:{0}:forward:tcp:{1};tcp:{2}", device.SerialNumber, port, tcp);
                this.ExecuteAdbSocketCommand(null, sc2);

                // 服务启动失败则使用组件启动
                if (dreceiver.Lines.Contains("Error: Not found; no service started."))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {

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
                var sc3 = String.Format("host-serial:{0}:forward:tcp:{1};tcp:{2}", device.SerialNumber, port, tcp);
                var sc4 = "am broadcast -a NotifyServiceStart";
                this.ExecuteRemoteCommand(sc1, device, dreceiver);
                this.ExecuteRemoteCommand(sc2, device, dreceiver);
                this.ExecuteAdbSocketCommand(null, sc3);
                this.ExecuteRemoteCommand(sc4, device, dreceiver);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region ExecuteAdbSocketCommand

        /// <summary>
        /// 执行ADB指令，可以指定端口，device可以为null，若为null则不指定设备。
        /// 结果接收器receiver可以为null，为null则不处理结果数据
        /// </summary>
        public void ExecuteAdbSocketCommand(Device device, string command, int port = 5037,
            AbstractOutputReceiver receiver = null,
            int bufSize = 16 * 1024, int timeOut = 9000)
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
                if (device != null) this.SetDevice(socket, device);
                byte[] request = AdbSocketHelper.FormAdbRequest(command);
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
                this.DisposeSocket(socket);
            }
        }

        #endregion

        #region CopyFile（拷贝文件（文件夹））

        /// <summary>
        /// 拷贝文件（文件夹）,返回对应的本地文件或文件夹路径
        /// </summary>
        /// <param name="sourcePath">设备文件路径</param>
        /// <param name="targetPath">本地全文件路径</param>
        public string CopyFile(Device device, string sourcePath, string targetPath, IAsyncResult asyn)
        {
            if (String.IsNullOrWhiteSpace(sourcePath) || String.IsNullOrWhiteSpace(targetPath))
            {
                return string.Empty;
            }

            if (!IsExistsFileOrPath(device, sourcePath))
            {//如果文件或路径不存在，直接返回
                return string.Empty;
            }

            //upgrade permission
            this.UpgradeFilePermission(device, sourcePath.Replace("#F", ""), true);

            if (!sourcePath.EndsWith("#F"))
            {
                //直接copy单文件的
                return this.CopySingleFile(device, sourcePath, targetPath, asyn);
            }
            //folder
            sourcePath = sourcePath.Replace("#F", "");
            var files = this.FindFiles(device, sourcePath);
            if (files==null)
            {
                return string.Empty;
            }
            //copy folder
            var path = FileHelper.ConnectPath(targetPath, FilterLinuxFileName(sourcePath).Replace("/", "\\"));
            FileHelper.CreateDirectory(path);
            CopyFolder(device, files, targetPath, asyn);
            return path;
        }
        #endregion

        #region CopyFolder
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        private void CopyFolder(Device device, List<LSFile> files, string targetPath, IAsyncResult asyn)
        {
            if (files==null)
            {
                return;
            }

            foreach (var file in files)
            {
                if (file.IsFolder)
                {
                    //
                    if (!file.HasPermission)
                    {
                        this.UpgradeFilePermission(device, file.FullPath);
                    }
                    var fs = this.FindFiles(device, file.FullPath);
                    this.CopyFolder(device, fs, targetPath, asyn);
                }
                else if (file.Size > 0)
                {
                    this.CopySingleFile(device, file, targetPath, asyn);
                }
            }
        }
        #endregion

        #region FindFiles（对文件或文件夹执行ls指令，获取起文件列表信息）
        /// <summary>
        /// 对文件或文件夹执行ls指令，获取起文件列表信息
        /// </summary>
        public List<LSFile> FindFiles(Device device, string sourceFile)
        {
            try
            {
                LSFileReceiver receiver = new LSFileReceiver();
                receiver.Source = sourceFile;
                //若文件名中包含空格
                sourceFile = sourceFile.Replace(" ", "\\ ");
                this.ExecuteRemoteAutoCommand(string.Format("ls -l {0}", sourceFile), device, receiver);
                return receiver.Files;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region CopySingleFile
        /// <summary>
        /// 拷贝单个设备文件到本地路径，返回新的本地路径。
        /// </summary>
        public string CopySingleFile(Device device, string sourceFile, string targetPath, IAsyncResult asyn)
        {
            string target = string.Empty;
            try
            {
                target = FileHelper.ConnectPath(targetPath,FilterLinuxFileName(sourceFile).Replace("/", "\\"));
                FileHelper.CreateDirectory(FileHelper.GetFilePath(target));
                DoPullFile(device, sourceFile, target);

                return target;
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(target);
            }
            return string.Empty;
        }

        /// <summary>
        /// 拷贝单个设备文件到本地路径，返回新的本地路径。
        /// </summary>
        public string CopySingleFile(Device device, LSFile file, string targetPath, IAsyncResult asyn)
        {
            if (!file.HasPermission)
            {
                if (!this.UpgradeFilePermission(device, file.FullPath))
                {
                    return string.Empty;
                }
            }
            var res = this.CopySingleFile(device, file.FullPath, targetPath, asyn);
            return res;
        }
        private static readonly char[] InvalidPathChars = new char[] {
                        '"', '<', '>', '|', '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v',
                        '\f', '\r', '\x000e', '\x000f', '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b',
                        '\x001c', '\x001d', '\x001e', '\x001f','*', '?', ':','\\',
                 };
        public string FilterLinuxFileName(string filename)
        {
            var index = filename.IndexOfAny(InvalidPathChars);
            if (index >= 0)
            {
                return FilterLinuxFileName(filename.Replace(filename[index], '_'));
            }
            return filename;
        }

#endregion

        #region IsExistsFileOrPath

        /// <summary>
        /// 判断文件或者路径是否存在
        /// </summary>
        /// <param name="device"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsExistsFileOrPath(Device device, string path)
        {
            int buffersize = 16 * 1024;
            int maxtimeout = 9000;
            DefaultReceiver receiver = new DefaultReceiver();

            string command = string.Format("ls -l {0}", path.TrimEnd(new char[] { '#','F' }));
            if (device.IsRoot)
            {
                command = string.Format("su -c \"{0}\" ", command);
            }

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(this.SocketAddress);
                socket.ReceiveTimeout = maxtimeout;
                socket.SendTimeout = maxtimeout;
                socket.ReceiveBufferSize = buffersize;

                //设置当前使用设备
                String msg = "host:transport:" + device.SerialNumber;
                byte[] device_query = AdbSocketHelper.FormAdbRequest(msg);
                AdbSocketHelper.Write(socket, device_query);
                var res = AdbSocketHelper.ReadResponse(socket);
                if (!res.IsOkay)
                {
                    return false;
                }

                //reqest
                var request = AdbSocketHelper.FormAdbRequest("shell:" + command);
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
                var tdata = receiver.Data.ToString().Trim();
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
                this.DisposeSocket(socket);
            }

            return true;
        }

        #endregion

        #region CatToFile
        /// <summary>
        /// 尝试读取源文件并写入本地文件中。
        /// </summary>
        private string CatToFile(Device device, string sourceFile, string local, CatToFileReceiver receiver = null)
        {
            try
            {
                this.UpgradeFilePermission(device, sourceFile);
                receiver = receiver ?? new CatToFileReceiver(local);
                this.ExecuteRemoteAutoCommand(string.Format("cat {0}", sourceFile), device, receiver,64 * 1024);
                return string.Empty;
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(local);
                string e = string.Format("try cat to file[{0}] failed", sourceFile);
                return e;
            }
            finally
            {
                if (receiver != null) receiver.Dispose();
            }
        }
        #endregion

        #region DoPullFile（执行文件拷贝到本地操作）

        /// <summary>
        /// 执行文件拷贝到本地操作
        /// </summary>
        private void DoPullFile(Device device, string sourceFile, string local, PullFileReceiver receiver = null,
                                int bufferSize = 64 * 1024, 
                                int maxtimeout = 9000, bool trycat = true)
        {
            if (device == null || String.IsNullOrWhiteSpace(sourceFile) || String.IsNullOrWhiteSpace(local))
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

                this.ExecuteRemoteAutoCommandNoException(cpCmd, device, dr);//拷贝/data/data/ 到/data/local/tmp

                if (this.IsExistsFileOrPath(device, tempSrc))//判断是否拷贝成功
                {
                    this.UpgradeFilePermission(device, tempSrc);//提权
                    sourceFile = tempSrc;
                    rmCmd = string.Format("rm {0}", tempSrc);
                }
            }

            #endregion

            receiver = receiver ?? new PullFileReceiver(local);
            
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(this.SocketAddress);
                socket.ReceiveTimeout = maxtimeout;
                socket.SendTimeout = maxtimeout;
                socket.Blocking = true;
                socket.ReceiveBufferSize = bufferSize;
                this.SetDevice(socket, device);
                this.SendAsyncReqest(socket);
                var request = AdbSocketHelper.FormPullFileRequest(sourceFile);
                AdbSocketHelper.Write(socket, request);
                receiver.DoReceive(socket);
            }
            catch (ADBConnectionException cex)
            {
                var mes = string.Format("pull file[{0}] failed:{1}", sourceFile, cex.Message);

                if (trycat)
                {
                    var rec = new CatToFileReceiver(local);
                    rec.OnReceiveData = receiver.OnReceiveData;
                    var rt = this.CatToFile(device, sourceFile, local, rec);
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
                if (IsNeedMoveFile && !String.IsNullOrWhiteSpace(rmCmd))
                {//删除temp文件
                    try
                    {
                        this.ExecuteRemoteAutoCommandNoException(rmCmd, device, dr);
                    }
                    catch { }
                }

                this.DisposeSocket(socket);
            }
        }

        #endregion
    }
}

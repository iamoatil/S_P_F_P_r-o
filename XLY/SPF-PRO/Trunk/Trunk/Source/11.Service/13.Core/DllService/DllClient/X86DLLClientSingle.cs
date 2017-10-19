using DllClient.Callback;
using DllClient.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DllClient
{
    /// <summary>
    /// WCF调用X86DLL服务
    /// </summary>
    public class X86DLLClientSingle
    {
        /// <summary>
        /// 服务器回调客户端
        /// </summary>
        public readonly ServerCallback ClientCallback;

        #region 通信管道

        #region 核心服务

        /// <summary>
        /// 底层核心服务
        /// </summary>
        private ServiceElement<CoreServiceClient, ICoreService> _coreEmt;

        /// <summary>
        /// 核心服务通道（预留接口）
        /// </summary>
        public ICoreService CoreChannel
        {
            get
            {
                return _coreEmt.IService;
            }
        }

        #endregion

        #region 安卓手机服务

        private ServiceElement<AndroidMirrorAPIServiceClient, IAndroidMirrorAPIService> _androidEmt;

        /// <summary>
        /// 安卓手机镜像服务通道
        /// </summary>
        public IAndroidMirrorAPIService AndroidMirrorAPIChannel
        {
            get
            {
                return _androidEmt.IService;
            }
        }

        #endregion

        #region Vivo手机服务

        private ServiceElement<VivoBackupAPIServiceClient, IVivoBackupAPIService> _vivoEmt;

        /// <summary>
        /// Vivo手机备份服务通道
        /// </summary>
        public IVivoBackupAPIService VivoBackupAPIChannel
        {
            get
            {
                return _vivoEmt.IService;
            }
        }

        #endregion

        #region 黑莓手机服务

        private ServiceElement<BlackBerryDeviceAPIServiceClient, IBlackBerryDeviceAPIService> _blackberryEmt;

        /// <summary>
        /// 黑莓手机服务
        /// </summary>
        public IBlackBerryDeviceAPIService BlackBerryDeviceAPIChannel
        {
            get
            {
                return _blackberryEmt.IService;
            }
        }

        #endregion

        #region SIM服务

        private ServiceElement<SIMcoreAPIServiceClient, ISIMcoreAPIService> _simEmt;
        /// <summary>
        /// SIM服务
        /// </summary>
        public ISIMcoreAPIService SIMcoreAPIServiceChannel
        {
            get
            {
                return _simEmt.IService;
            }
        }

        #endregion

        #endregion

        #region 单例

        private X86DLLClientSingle()
        {
            ClientCallback = new ServerCallback();

            _coreEmt = new ServiceElement<CoreServiceClient, ICoreService>(new CoreServiceClient(new InstanceContext(ClientCallback)));
            _androidEmt = new ServiceElement<AndroidMirrorAPIServiceClient, IAndroidMirrorAPIService>(new AndroidMirrorAPIServiceClient());
            _vivoEmt = new ServiceElement<VivoBackupAPIServiceClient, IVivoBackupAPIService>(new VivoBackupAPIServiceClient());
            _blackberryEmt = new ServiceElement<BlackBerryDeviceAPIServiceClient, IBlackBerryDeviceAPIService>(new BlackBerryDeviceAPIServiceClient());
            _simEmt = new ServiceElement<SIMcoreAPIServiceClient, ISIMcoreAPIService>(new SIMcoreAPIServiceClient());
        }

        private volatile static X86DLLClientSingle _instance;

        private static object _objLock = new object();

        /// <summary>
        /// 底层X86DLL服务
        /// </summary>
        public static X86DLLClientSingle Instance
        {
            get
            {
                if (_instance == null)
                    lock (_objLock)
                        if (_instance == null)
                            _instance = new X86DLLClientSingle();
                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// 关闭所有服务
        /// </summary>
        public void CloseServices()
        {
            _coreEmt.CloseService();
            _androidEmt.CloseService();
            _vivoEmt.CloseService();
            _blackberryEmt.CloseService();
            _simEmt.CloseService();
        }
    }
}

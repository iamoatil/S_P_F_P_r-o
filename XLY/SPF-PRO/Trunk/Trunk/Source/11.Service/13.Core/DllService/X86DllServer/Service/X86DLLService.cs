using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using X86DllServer.IService;

namespace X86DllServer.Service
{
    /// <summary>
    /// DLL服务具体实现
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class X86DLLService : ICoreService
    {
        #region 元素定义

        private DateTime _lastUpdate;

        /// <summary>
        /// USB回调客户端
        /// </summary>
        private IClientCallback _clientCallback;

        #endregion

        #region 核心服务实现

        public void Leave()
        {
            Console.WriteLine("服务关闭时间{0}", _lastUpdate);
        }

        public bool Login()
        {
            _clientCallback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
            return true;
        }

        public void Update()
        {
            _lastUpdate = DateTime.Now;
        }

        #endregion
    }
}

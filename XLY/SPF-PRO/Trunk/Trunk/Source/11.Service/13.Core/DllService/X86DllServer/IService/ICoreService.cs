using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace X86DllServer.IService
{
    /// <summary>
    /// DLL核心服务，主要用于心跳更新，预留其他接口以备使用
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface ICoreService
    {
        /// <summary>
        /// 更新心跳
        /// </summary>
        void Update();

        /// <summary>
        /// 登录
        /// </summary>
        [OperationContract(IsOneWay = false)]
        bool Login();

        /// <summary>
        /// 退出
        /// </summary>
        [OperationContract]
        void Leave();
    }
}

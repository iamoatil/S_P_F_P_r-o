using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DllClient
{
    public class ServiceElement<TServiceClient,TService>
        where TServiceClient : ClientBase<TService>
        where TService : class
    {
        public ServiceElement(TServiceClient client)
        {
            if (client == null)
                throw new NullReferenceException("client不能为NULL");

            _client = client;
            IService = _client.ChannelFactory.CreateChannel();
            _comObj = IService as ICommunicationObject;
            _comObj.Faulted += _comObj_Faulted;
        }

        private void _comObj_Faulted(object sender, EventArgs e)
        {
            IService = _client.ChannelFactory.CreateChannel();
            _comObj.Faulted -= _comObj_Faulted;
            _comObj = IService as ICommunicationObject;
            _comObj.Faulted += _comObj_Faulted;
        }

        /// <summary>
        /// 本地服务实例
        /// </summary>
        private TServiceClient _client;

        /// <summary>
        /// 当前使用的信道
        /// </summary>
        private ICommunicationObject _comObj;

        /// <summary>
        /// 当前使用的服务信道
        /// </summary>
        public TService IService { get; private set; }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void CloseService()
        {
            try
            {
                _client.Close();
            }
            catch
            {
                _client.Abort();
            }
        }
    }
}

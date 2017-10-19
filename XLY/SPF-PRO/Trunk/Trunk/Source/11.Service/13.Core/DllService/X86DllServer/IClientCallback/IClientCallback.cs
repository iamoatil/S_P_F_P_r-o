/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/8/29 10:25:49 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace X86DllServer
{
    public interface IClientCallback
    {
        /// <summary>
        /// 镜像回调函数
        /// </summary>
        /// <param name="data">每次返回数据</param>
        /// <param name="stop">是否停止，0-继续，1-停止</param>
        /// <returns></returns>
        [OperationContract]
        void ImageDataCallBack(byte[] data, ref int stop);

        /// <summary>
        /// Vivo手机备份回调函数
        /// </summary>
        /// <param name="imagedALLSize"></param>
        /// <param name="filename">正在备份的文件</param>
        /// <param name="stop">是否停止，0-继续，1-停止</param>
        [OperationContract]
        void VivoBackupCallBack(Int64 imagedALLSize, string filename, ref int stop);

        /// <summary>
        /// 黑莓手机镜像回调函数
        /// </summary>
        /// <param name="imagedALLSize"></param>
        /// <param name="filename">正在备份的文件</param>
        /// <param name="stop">是否停止，0-继续，1-停止</param>
        [OperationContract]
        void BlackBerryImageDataCallBack(Int64 imagedALLSize, string filename, ref int stop);

    }
}

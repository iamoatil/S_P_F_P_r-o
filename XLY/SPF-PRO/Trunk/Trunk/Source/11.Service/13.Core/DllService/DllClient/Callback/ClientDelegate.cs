using DllClient.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllClient.Callback
{
    /// <summary>
    /// 镜像回调函数
    /// </summary>
    /// <param name="data">每次返回数据</param>
    /// <param name="stop">是否停止，0-继续，1-停止</param>
    /// <returns></returns>
    public delegate void ImageDataCallBackDelegate(byte[] data, ref int stop);

    /// <summary>
    /// Vivo手机备份回调
    /// </summary>
    /// <param name="imagedALLSize"></param>
    /// <param name="filename"></param>
    /// <param name="stop">是否停止，0-继续，1-停止</param>
    public delegate void VivoBackupCallBackDelegate(long imagedALLSize, string filename, ref int stop);

    /// <summary>
    /// 黑莓手机备份回调
    /// </summary>
    /// <param name="imagedALLSize"></param>
    /// <param name="filename"></param>
    /// <param name="stop">是否停止，0-继续，1-停止</param>
    public delegate void BlackBerryBackupCallBackDelegate(long imagedALLSize, string filename, ref int stop);

}

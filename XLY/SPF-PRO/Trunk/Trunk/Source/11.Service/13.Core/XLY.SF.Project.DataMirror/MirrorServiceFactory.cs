// ***********************************************************************
// Assembly:XLY.SF.Project.DataMirror
// Author:Songbing
// Created:2017-04-05 14:31:09
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataMirror
{
    /// <summary>
    /// 镜像服务工厂
    /// </summary>
    internal class MirrorServiceFactory : ISimpleFactory<Mirror, IMirrorService>
    {
        /// <summary>
        /// 获取数据镜像服务实例
        /// </summary>
        /// <param name="key">镜像源</param>
        /// <returns>数据镜像服务</returns>
        public IMirrorService GetInstance(Mirror key)
        {
            /*
             * 
             * 注意：这儿必须每次都返回新的实例
             * 以免多任务运行的时候产生冲突
             * 
             * */

            switch (key.Type)
            {
                case EnumMirror.Device:
                    var d = key.Source as Device;
                    if (d == null)
                    {
                        break;
                    }
                    switch (d.OSType)
                    {
                        case EnumOSType.Android:
                            return new AndroidUSBAPIMirrorService();
                        case EnumOSType.IOS:
                            return new IOSDeviceMirrorService();
                    }
                    break;
            }

            throw new NotImplementedException();
        }
    }
}

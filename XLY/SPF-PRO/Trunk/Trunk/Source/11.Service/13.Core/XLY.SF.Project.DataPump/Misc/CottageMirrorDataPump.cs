using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.DataPump.Android;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.Misc
{
    /// <summary>
    /// 棒棒鸡Cottage的镜像数据泵。
    /// </summary>
    public class CottageMirrorDataPump : AndroidMirrorDataPump
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.Misc.CottageMirrorDataPump 实例。
        /// </summary>
        public CottageMirrorDataPump()
        {
        }

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 创建实现了 IFileSystemDevice 接口的类型实例。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        /// <returns>实现了 IFileSystemDevice 接口的类型实例。</returns>
        protected override IFileSystemDevice CreateFileSystemDevice(DataPumpControllableExecutionContext context)
        {
            Device device = context.PumpDescriptor.Source as Device;
            if (device == null) return null;
            IFileSystemDevice fsDevice = new CottageDevice
            {
                FlshType = device.FlshType,
                DevType = device.DevType,
                Source = context.PumpDescriptor.Source,
                ScanModel = (Byte)context.PumpDescriptor.ScanModel
            };
            return fsDevice;
        }

        #endregion

        #endregion
    }
}

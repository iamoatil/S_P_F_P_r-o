using System;
using XLY.SF.Project.DataPump.Android;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.Misc
{
    /// <summary>
    /// SD卡数据泵。
    /// </summary>
    public class SdCardDataPump : AndroidMirrorDataPump
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.DataPump.Misc.SdCardDataPump 实例。
        /// </summary>
        public SdCardDataPump()
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
            IFileSystemDevice device = new SDCardDevice
            {
                Source = context.PumpDescriptor,
                ScanModel = (Byte)context.PumpDescriptor.ScanModel
            };
            return device;
        }

        #endregion

        #endregion
    }
}

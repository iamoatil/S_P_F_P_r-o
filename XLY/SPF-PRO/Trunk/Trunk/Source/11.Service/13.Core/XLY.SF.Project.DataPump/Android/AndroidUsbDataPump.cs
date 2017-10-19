using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataPump.Android
{
    /// <summary>
    /// Android USB数据泵。
    /// </summary>
    public class AndroidUsbDataPump : ControllableDataPumpBase
    {
        #region Methods

        #region Public

        /// <summary>
        /// 使用特定的执行上下文执行服务。
        /// </summary>
        /// <param name="context">执行上下文。</param>
        protected override void ExecuteCore(DataPumpControllableExecutionContext context)
        {
            Device device = context.PumpDescriptor.Source as Device;
            if (device == null) return;
            switch (context.Source.ItemType)
            {
                case SourceFileItemType.AndroidSDCardPath:
                    HandleWithSDCardPath(device, context);
                    break;
                case SourceFileItemType.NormalPath:
                    HandleWithNormalPath(device, context);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region Private

        private void HandleWithSDCardPath(Device device, DataPumpControllableExecutionContext context)
        {
            SourceFileItem source = context.Source;
            String path = FileHelper.ConnectLinuxPath(device.SDCardPath, source.SDCardConfig);
            source.Local = device.CopyFile(path, context.TargetDirectory, context.Reporter);
        }

        private void HandleWithNormalPath(Device device, DataPumpControllableExecutionContext context)
        {
            String path = context.Source.Config;
            context.Source.Local = device.CopyFile(path, context.TargetDirectory, context.Reporter);
        }

        #endregion

        #endregion
    }
}

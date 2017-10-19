using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;
using XLY.SF.Project.DataPump;
using XLY.SF.Project.Devices;

namespace XLY.SF.Project.TestApp
{
    public class HuJjingDataDumpTest
    {
        #region Fields

        private readonly AbstractDeviceMonitor _deviceMonitor;

        #endregion

        #region Constructors

        public HuJjingDataDumpTest()
        {
            _deviceMonitor = new AndroidDeviceMonitor();
            _deviceMonitor.DeviceConnected = Connected;
        }

        #endregion

        #region Methods

        #region Public

        public void TestExecuteMethod(Pump pump, String savePath, SourceFileItem source, IAsyncProgress reporter, IEnumerable<ExtractItem> items = null)
        {
            DataPumpExecutionContext context = pump.Execute(savePath, source, reporter, items);
        }

        public void TestCancelMethod(Pump pump, String savePath, SourceFileItem source, IAsyncProgress reporter, IEnumerable<ExtractItem> items = null)
        {
            DataPumpExecutionContext context = pump.Execute(savePath, source, reporter, items);
            context.Cancel();
        }

        public void Start()
        {
            _deviceMonitor.Start();
        }

        public void Stop()
        {
            _deviceMonitor.Close();
        }

        #endregion

        #region Private

        private static Pump CreatePump(IDevice device,out SourceFileItem souce)
        {
            Pump pump = null;
            switch (device.DeviceType)
            {
                case EnumDeviceType.SDCard:
                    pump = new Pump { OSType = EnumOSType.SDCard, Type = EnumPump.SDCard};
                    break;
                case EnumDeviceType.SIM:
                    pump = new Pump { OSType = EnumOSType.SIMCard, Type = EnumPump.SIMCard };
                    break;
                case EnumDeviceType.Phone when device is Device dev:
                    pump = new Pump { OSType = dev.OSType, Type = EnumPump.Mirror };
                    break;
                default:
                    souce = null;
                    return null;
            }
            SourceFileItem item = new SourceFileItem();
            item.ItemType = SourceFileItemType.NormalPath;
            item.Config = @"/system/build.prop";
            souce = item;
            pump.Source = device;
            pump.ScanModel = ScanFileModel.Quick;
            return pump;
        }

        private void Connected(IDevice device)
        {
            Pump pump = CreatePump(device,out SourceFileItem item);
            if (pump == null) return;
            TestExecuteMethod(pump, @"F:\Temp", item, null);
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.ViewDomain.Model
{
    /// <summary>
    /// 设备上下线
    /// </summary>
    /// <param name="isOnline">是否上线</param>
    /// <param name="dev">设备</param>
    public delegate void DeviceConnectedDelegate(IDevice dev, bool isOnline);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/4/7 14:52:02
 * 类功能说明：
 * 1. ADB请求返回结果
 *
 *************************************************/

namespace XLY.SF.Project.Devices.AdbSocketManagement
{
    public class AdbResponse
    {
        /// <summary>
        /// 返回数据是否okay
        /// </summary>
        public bool IsOkay { get; set; }

        /// <summary>
        /// 应答的数据信息
        /// </summary>
        public string Data { get; private set; }
        
        /// <summary>
        ///  AdbResponse-构造函数（初始化）
        /// </summary>
        public AdbResponse(string data)
        {
            this.Data = data;
        }
    }
}

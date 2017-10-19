using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.Services.ADB
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

        #region AdbResponse-构造函数（初始化）

        /// <summary>
        ///  AdbResponse-构造函数（初始化）
        /// </summary>
        public AdbResponse(string data)
        {
            this.Data = data;
        }

        #endregion
    }
}

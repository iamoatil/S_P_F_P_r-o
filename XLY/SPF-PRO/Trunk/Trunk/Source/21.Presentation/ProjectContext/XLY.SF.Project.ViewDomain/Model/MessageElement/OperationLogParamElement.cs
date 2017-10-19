using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewDomain.Model
{
    /// <summary>
    /// 操作日志参数
    /// </summary>
    public class OperationLogParamElement
    {
        /// <summary>
        /// 操作内容
        /// </summary>
        public string OperationContent { get; set; }

        /// <summary>
        /// 对应功能模块
        /// </summary>
        public string FunctionModule { get; set; }

        /// <summary>
        /// 截图路径【未截图，则为空】
        /// </summary>
        public string ScreenShotPath { get; set; }
    }
}

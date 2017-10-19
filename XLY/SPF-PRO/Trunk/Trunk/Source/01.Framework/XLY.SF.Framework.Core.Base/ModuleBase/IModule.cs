using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/1 11:41:44
 * 接口功能说明：
 * 用于模块加载时调用
 * 需要模块加载时继承此接口然后添加导出标记
 * 不一定必须继承此接口
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.ModuleBase
{
    public interface IModule
    {
        /// <summary>
        /// 初始化模块
        /// </summary>
        void InitModule();

        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="parameter"></param>
        void LoadModule(dynamic parameter);
    }
}

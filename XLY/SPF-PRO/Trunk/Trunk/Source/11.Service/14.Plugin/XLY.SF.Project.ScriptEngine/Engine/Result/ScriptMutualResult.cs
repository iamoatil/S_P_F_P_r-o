using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json;

namespace XLY.SF.Project.ScriptEngine.Engine
{
    /// <summary>
    /// 脚本交互返回值
    /// </summary>
    public class ScriptMutualResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public IEnumerable<dynamic> Data { get; set; }

        /// <summary>
        /// 将对象转化为Json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

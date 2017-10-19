using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{/// <summary>
 /// 特征匹配
 /// </summary>
    class DefaultPluginFeatureMathch : IPluginFeatureMathch
    {
        /// <summary>
        /// APP名字
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 操作系统类型
        /// </summary>
        public EnumOSType OSType { get; set; }

        /// <summary>
        /// 手机厂商
        /// </summary>
        public string Manufacture { get; set; }

        /// <summary>
        /// 匹配规则
        /// </summary>
        public IList<IPluginFeatureRule> Rules { get; set; }

        public DefaultPluginFeatureMathch()
        {
            Rules = new List<IPluginFeatureRule>();
        }

        /// <summary>
        /// 匹配特征
        /// </summary>
        /// <param name="taskSourcePath">数据源路径</param>
        /// <returns></returns>
        public PluginFeatureMathchResult TryMathch(string taskSourcePath)
        {
            try
            {
                //从第一条Rule开始匹配
                IPluginFeatureRule rule = Rules.FirstOrDefault((r) => r.Id == "1");

                while (null != rule)
                {
                    //验证Rule
                    var result = rule.TryMathch(taskSourcePath);

                    //解析验证结果
                    var arr = result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 1 && arr[0] == "Null")
                    {//特征匹配失败
                        return new PluginFeatureMathchResult() { IsSuccessed = false };
                    }
                    else if (arr.Length == 2 && arr[0] == "Next")
                    {//执行下一个Rule的验证
                        rule = Rules.FirstOrDefault((f) => f.Id == arr[1]);
                    }
                    else if (arr.Length == 3 && arr[0] == "App")
                    {//特征匹配成功
                        return new PluginFeatureMathchResult() { OSType = OSType, Manufacture = Manufacture, AppName = arr[1], AppVersion = arr[2], IsSuccessed = true };
                    }
                    else
                    {
                        rule = null;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(string.Format("特征匹配异常，错误信息：{0}", ex));
                return null;
            }
        }

    }
}

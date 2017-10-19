using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Domains;
using XLY.SF.Project.ViewDomain.ICommon;
using XLY.SF.Project.ViewDomain.Model;

/*************************************************
 * 创建人：Bob
 * 创建时间：2017/9/12
 * 类功能说明：
 * 1. 推荐方案策略
 *
 *************************************************/

namespace XLY.SF.Project.Extension.Function.StrategyRecommend
{
    /// <summary>
    /// 手机品牌策略
    /// </summary>
    public class StrategyProposedByPhoneBrand : ISolutionAdapter
    {
        public StrategyElement[] SolutionAdaptation(Device dev, StrategyElement[] solutionSource)
        {
            return solutionSource.Where((t) => t.Filter.Any(d => string.IsNullOrWhiteSpace(d.PhoneBrand) || d.PhoneBrand == dev.Brand)).ToArray();
        }
    }

    /// <summary>
    /// 手机型号策略
    /// </summary>
    public class StrategyProposedByPhoneModel : ISolutionAdapter
    {
        public StrategyElement[] SolutionAdaptation(Device dev, StrategyElement[] solutionSource)
        {
            return solutionSource.Where((t) => t.Filter.Any(d => string.IsNullOrWhiteSpace(d.DevModel) || d.DevModel == dev.Model)).ToArray();
        }
    }

    /// <summary>
    /// 操作系统策略
    /// </summary>
    public class StrategyPropsedBy0SType : ISolutionAdapter
    {
        public StrategyElement[] SolutionAdaptation(Device dev, StrategyElement[] solutionSource)
        {
            return solutionSource.Where((t) => t.Filter.Any(d => string.IsNullOrWhiteSpace(d.DevType) || d.DevType == dev.OSType.ToString())).ToArray();
        }
    }

    /// <summary>
    /// 系统版本策略
    /// </summary>
    public class StrategyProposedByOSVersion : ISolutionAdapter
    {
        public StrategyElement[] SolutionAdaptation(Device dev, StrategyElement[] solutionSource)
        {
            return solutionSource.Where((t) => t.Filter.Any(d => string.IsNullOrWhiteSpace(d.DevOSVersion) || d.DevOSVersion == dev.OSVersion)).ToArray();
        }
    }

    /// <summary>
    /// CPU品牌策略，目前Device中未找到CPU的定义所以无法匹配
    /// </summary>
    public class StrategyProposedByCpuBrand : ISolutionAdapter
    {
        public StrategyElement[] SolutionAdaptation(Device dev, StrategyElement[] solutionSource)
        {
            return solutionSource;
            //return solutionSource.Where((t) => t.Filter.Any(d => string.IsNullOrWhiteSpace(d.CpuBrand) || d.CpuBrand == dev.OSVersion)).ToArray();
        }
    }

    /// <summary>
    /// CPU型号策略，目前Device中未找到CPU的定义所以无法匹配
    /// </summary>
    public class StrategyProposedByCpuModelNum : ISolutionAdapter
    {
        public StrategyElement[] SolutionAdaptation(Device dev, StrategyElement[] solutionSource)
        {
            return solutionSource;
        }
    }

    /// <summary>
    /// 是否ROOT策略
    /// </summary>
    public class StrategyProposedByPhoneIsRoot : ISolutionAdapter
    {
        public StrategyElement[] SolutionAdaptation(Device dev, StrategyElement[] solutionSource)
        {
            return solutionSource.Where((t) => t.Filter.Any(d => !d.IsRoot.HasValue || (d.IsRoot.HasValue && d.IsRoot.Value == dev.IsRoot))).ToArray();
        }
    }

    /// <summary>
    /// 是否打开调试模式策略
    /// </summary>
    public class StrategyProposedByOpenDebug : ISolutionAdapter
    {
        public StrategyElement[] SolutionAdaptation(Device dev, StrategyElement[] solutionSource)
        {
            return solutionSource;
        }
    }
}

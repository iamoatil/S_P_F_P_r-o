using ProjectExtend.Context;
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
 * 1. 根据设备信息适配推荐方案
 * 2. 目前适配通过创建者模式实现
 *
 *************************************************/

namespace XLY.SF.Project.Extension.Function.StrategyRecommend
{
    public class SolutionProposedManager
    {
        #region 适配策略

        /// <summary>
        /// 策略集合
        /// </summary>
        private List<ISolutionAdapter> _strategies;

        #endregion

        public SolutionProposedManager()
        {
            _strategies = new List<ISolutionAdapter>()
            {
                new StrategyProposedByPhoneBrand(),
                new StrategyProposedByPhoneModel(),
                new StrategyPropsedBy0SType(),
                new StrategyProposedByOSVersion(),
                new StrategyProposedByCpuBrand(),
                new StrategyProposedByCpuModelNum(),
                new StrategyProposedByPhoneIsRoot(),
                new StrategyProposedByOpenDebug()
            };
        }

        /// <summary>
        /// 创建推荐方案
        /// </summary>
        /// <param name="dev">匹配的设备</param>
        public StrategyElement[] BuildSolution(Device dev)
        {
            StrategyElement[] strategyItems = SystemContext.Instance.GetAllProposedSolution();
            foreach (var item in _strategies)
            {
                strategyItems = item.SolutionAdaptation(dev, strategyItems);
            }
            return strategyItems;
        }
    }
}

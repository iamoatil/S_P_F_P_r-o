using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Framework.Log4NetService.LoggerEnum;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 11:41:19
 * 类功能说明：
 *      1. 利用MEF做IOC容器
 *      2. 此类提供统一的IOC加载以及导出
 *
 *************************************************/

namespace XLY.SF.Framework.Core.Base.MefIoc
{
    public class IocManagerSingle
    {
        #region Property

        private CompositionContainer com;

        #endregion

        #region Single

        private static object _objLock = new object();

        private IocManagerSingle()
        {
        }

        private static IocManagerSingle _instance;

        public static IocManagerSingle Instance
        {
            get
            {
                if (_instance == null)
                    lock (_objLock)
                        if (_instance == null)
                            _instance = new IocManagerSingle();
                return _instance;
            }
        }

        #endregion

        #region 添加导入导出

        public void FFFF(object part)
        {
            com.ComposeParts(part);
        }

        public void LoadParts(params Assembly[] ass)
        {
            try
            {
                AggregateCatalog agg = new AggregateCatalog();
                DirectoryCatalog catalog = new DirectoryCatalog(Environment.CurrentDirectory,"XLY.*.dll");
                agg.Catalogs.Add(catalog);
                AssemblyCatalog ac = new AssemblyCatalog(this.GetType().Assembly);
                agg.Catalogs.Add(ac);

                if (ass != null)
                {
                    foreach (var item in ass)
                    {
                        agg.Catalogs.Add(new AssemblyCatalog(item));
                    }
                }

                com = new CompositionContainer(agg, true);
                com.ComposeParts(this);
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
        }

        #endregion

        #region 获取部件

        /// <summary>
        /// 获取指定协定名的多个部件
        /// </summary>
        /// <typeparam name="TPart">要导出的部件类型</typeparam>
        /// <param name="contractName">部件协定名</param>
        /// <returns>拥有此协定名的所有部件，惰性加载</returns>
        public IEnumerable<Lazy<TPart>> GetParts<TPart>(string contractName)
        {
            try
            {
                IEnumerable<Lazy<TPart>> tmpResult;
                if (string.IsNullOrEmpty(contractName))
                    tmpResult = com.GetExports<TPart>();
                else
                    tmpResult = com.GetExports<TPart>(contractName);
                return tmpResult;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
            return default(IEnumerable<Lazy<TPart>>);
        }

        /// <summary>
        /// 获取指定协定名的部件
        /// </summary>
        /// <typeparam name="TPart">要导出的部件类型</typeparam>
        /// <param name="contractName">部件协定名</param>
        /// <returns></returns>
        public TPart GetPart<TPart>(string contractName)
        {
            try
            {
                TPart tmpResult;
                if (string.IsNullOrEmpty(contractName))
                    tmpResult = com.GetExportedValueOrDefault<TPart>();
                else
                    tmpResult = com.GetExportedValueOrDefault<TPart>(contractName);
                if (tmpResult is TPart)
                    return (TPart)tmpResult;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
            return default(TPart);
        }

        /// <summary>
        /// 获取指定协定名的View
        /// </summary>
        /// <param name="contractName">View的协定名</param>
        /// <returns></returns>
        public UcViewBase GetViewPart(string contractName)
        {
            return this.GetPart<UcViewBase>(contractName);
        }

        /// <summary>
        /// 根据类型获取对应部件
        /// </summary>
        /// <typeparam name="TPart">部件类型</typeparam>
        /// <returns></returns>
        public TPart GetPart<TPart>()
        {
            return GetPart<TPart>(null);
        }

        /// <summary>
        /// 根据类型获取对应部件，（带元数据）
        /// </summary>
        /// <typeparam name="TPart"></typeparam>
        /// <typeparam name="TMeta"></typeparam>
        /// <param name="contractName"></param>
        /// <returns></returns>
        public IEnumerable<Lazy<TPart, TMeta>> GetMetaParts<TPart, TMeta>(string contractName = null)
        {
            try
            {
                IEnumerable<Lazy<TPart, TMeta>> tmpResult;
                if (string.IsNullOrEmpty(contractName))
                    tmpResult = com.GetExports<TPart, TMeta>();
                else
                    tmpResult = com.GetExports<TPart, TMeta>(contractName);
                return tmpResult;
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex);
            }
            return default(IEnumerable<Lazy<TPart, TMeta>>);
        }
        #endregion
    }
}

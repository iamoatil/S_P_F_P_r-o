using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 多视图预览接口
    /// </summary>
    public interface IXlyMultiView
    {
        /// <summary>
        /// 所属多视图控件
        /// </summary>
        XlyMultiView MultiViewOwner { get; set; }

        /// <summary>
        /// 关联控件
        /// </summary>
        IXlyMultiViewLinkedControl LinkedControl { get; set; }

        /// <summary>
        /// 所属领域对象
        /// </summary>
        object Doamin { get; set; }

        /// <summary>
        /// 数据源所有者
        /// </summary>
        object ItemsSourceOwner { get; set; }

        /// <summary>
        /// 数据源
        /// </summary>
        IEnumerable ItemsSource { get; set; }

        /// <summary>
        /// 数据源列信息
        /// </summary>
        IEnumerable ColumnsSource { get; set; }

        /// <summary>
        /// 多视图信息
        /// </summary>
        IEnumerable<XlyMultiViewInfo> ViewsSource { get; set; }

        /// <summary>
        /// 当前视图信息
        /// </summary>
        XlyMultiViewInfo CurrentViewInfo { get; set; }

        /// <summary>
        /// 是否支持某种数据类型,Type可能为null
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        bool IsSupport(Type type);

        /// <summary>
        /// 被激活时执行
        /// </summary>
        void OnActivation();

        /// <summary>
        /// 反激活时执行
        /// </summary>
        void OnDeactivation();
    }
}

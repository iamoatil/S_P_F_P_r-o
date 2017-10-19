using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;

namespace XLY.XDD.Control
{
    /// <summary>
    /// 列表数据移动辅助类
    /// </summary>
    public static class GridViewItemsSourceMoveHelper
    {
        /// <summary>
        /// 将选中项向上移动
        /// </summary>
        /// <typeparam name="T">数据源数据类型</typeparam>
        /// <param name="itemsSource">用于绑定的数据源</param>
        /// <param name="selectedSource">选中项数据源（该参数必须来自GridView.SelectedItems）</param>
        public static void MoveUp<T>(ObservableCollection<T> itemsSource, IList selectedSource)
        {
            if (itemsSource == null || itemsSource.Count == 0 || selectedSource == null || selectedSource.Count == 0)
                return;
            List<T> s_list = getSortSelected(itemsSource, selectedSource);
            int index = 0;
            foreach (T i in s_list)
            {
                index = itemsSource.IndexOf(i);
                if (index > 0)
                {
                    if (selectedSource.Contains(itemsSource[index - 1]))
                        continue;
                    itemsSource.Remove(i);
                    itemsSource.Insert(index - 1, i);
                }
            }
            foreach (T i in s_list)
            {
                if (!selectedSource.Contains(i))
                {
                    selectedSource.Add(i);
                }
            }
        }

        /// <summary>
        /// 将选中项下移
        /// </summary>
        /// <typeparam name="T">数据源数据类型</typeparam>
        /// <param name="itemsSource">用于绑定的数据源</param>
        /// <param name="selectedSource">选中项数据源（该参数必须来自GridView.SelectedItems）</param>
        public static void MoveDown<T>(ObservableCollection<T> itemsSource, IList selectedSource)
        {
            if (itemsSource == null || itemsSource.Count == 0 || selectedSource == null || selectedSource.Count == 0)
                return;
            List<T> s_list = getSortSelected(itemsSource, selectedSource);
            s_list.Reverse();
            int index = 0;
            foreach (T i in s_list)
            {
                index = itemsSource.IndexOf(i);
                if (index < itemsSource.Count - 1)
                {
                    if (selectedSource.Contains(itemsSource[index + 1]))
                        continue;
                    itemsSource.Remove(i);
                    itemsSource.Insert(index + 1, i);
                }
            }
            foreach (T i in s_list)
            {
                if (!selectedSource.Contains(i))
                {
                    selectedSource.Add(i);
                }
            }
        }

        /// <summary>
        /// 将选中项移至顶部
        /// </summary>
        /// <typeparam name="T">数据源数据类型</typeparam>
        /// <param name="itemsSource">用于绑定的数据源</param>
        /// <param name="selectedSource">选中项数据源（该参数必须来自GridView.SelectedItems）</param>
        public static void MoveTop<T>(ObservableCollection<T> itemsSource, IList selectedSource)
        {
            if (itemsSource == null || itemsSource.Count == 0 || selectedSource == null || selectedSource.Count == 0)
                return;
            List<T> s_list = getSortSelected(itemsSource, selectedSource);
            s_list.Reverse();
            foreach (T i in s_list)
            {
                itemsSource.Remove(i);
            }
            foreach (T i in s_list)
            {
                itemsSource.Insert(0, i);
            }
            foreach (T i in s_list)
            {
                if (!selectedSource.Contains(i))
                {
                    selectedSource.Add(i);
                }
            }
        }

        /// <summary>
        /// 将选中项移至底部
        /// </summary>
        /// <typeparam name="T">数据源数据类型</typeparam>
        /// <param name="itemsSource">用于绑定的数据源</param>
        /// <param name="selectedSource">选中项数据源（该参数必须来自GridView.SelectedItems）</param>
        public static void MoveBottom<T>(ObservableCollection<T> itemsSource, IList selectedSource)
        {
            if (itemsSource == null || itemsSource.Count == 0 || selectedSource == null || selectedSource.Count == 0)
                return;
            List<T> s_list = getSortSelected(itemsSource, selectedSource);
            foreach (T i in s_list)
            {
                itemsSource.Remove(i);
            }
            foreach (T i in s_list)
            {
                itemsSource.Add(i);
            }
            foreach (T i in s_list)
            {
                if (!selectedSource.Contains(i))
                {
                    selectedSource.Add(i);
                }
            }
        }

        /// <summary>
        /// 获取排序之后的选中项
        /// </summary>
        private static List<T> getSortSelected<T>(ObservableCollection<T> itemsSource, IList selectedSource)
        {
            List<T> temp = new List<T>();
            foreach (T t in selectedSource)
            {
                temp.Add(t);
            }
            var q = from i in itemsSource
                    from i_s in temp
                    where i.Equals(i_s)
                    orderby itemsSource.IndexOf(i)
                    select i;
            return q.ToList();
        }
    }
}

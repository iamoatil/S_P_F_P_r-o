using System;
using System.Collections.Generic;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Search;

namespace XLY.SF.Project.Services
{
    public class SearchService
    {
        /// <summary>
        /// 根据搜索条件获取搜索结果
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public SearchResult GetSearchResult(IDataSource ds, SearchCondition condition)
        {
            if(String.IsNullOrEmpty(condition.Keyword))
            {
                return null;
            }
            return null;
        }


        /// <summary>
        /// 搜索xly自定义格式数据的数据源
        /// </summary>
        private void SearchXLYDataSource(List<KeyValueItem> items, IDataSource ds)
        {

        }

        /// <summary>
        /// 搜索动态类型的数据源
        /// </summary>
        private void SearchDynamicDataSource(List<KeyValueItem> items, IDataSource ds)
        {

        }

        private void SearchTreeDataSource(List<KeyValueItem> items, IDataSource ds)
        {

        }

        private void SearchTreeDataSource(List<KeyValueItem> items, TreeNode node, IDataSource ds)
        {

        }

        private void SearchTreeItems(List<KeyValueItem> items, TreeNode node, IDataSource ds)
        {

        }

    }
}

using System.Collections.Generic;

namespace XLY.SF.Project.Views.PreviewFile
{
    /// <summary>
    /// Path集合
    /// 安全性：非线程安全。
    /// </summary>
    public class PathCollection
    {
        private List<string> _listPath = new List<string>();
        private int _curIndex;

        /// <summary>
        /// 向集合中添加path
        /// </summary>
        /// <param name="paths"></param>
        public void AddPaths(string[] paths)
        {
            foreach (var item in paths)
            {
                _listPath.Add(item);
            }
        }

        /// <summary>
        /// 从集合中删除Path
        /// </summary>
        public void DeletePaths(string[] paths)
        {
            foreach (var item in paths)
            {
                _listPath.Remove(item);
            }
        }

        /// <summary>
        /// 返回集合中的path列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetPathList()
        {
            return _listPath;
        }

        /// <summary>
        /// 返回下一个Path
        /// </summary>
        /// <returns></returns>
        public string GetNextPath()
        {
            if (_listPath.Count == 0)
            {
                return null;
            }

            int nextIndex = _curIndex + 1;
            if (nextIndex > _listPath.Count - 1)
            {
                nextIndex -= 1;
            }
            _curIndex = nextIndex;

            return GetPathByIndex(_curIndex);
        }

        /// <summary>
        /// 返回上一个Path
        /// </summary>
        /// <returns></returns>
        public string GetPreviousPath()
        {
            if (_listPath.Count == 0)
            {
                return null;
            }

            int nextIndex = _curIndex - 1;
            if (nextIndex < 0)
            {
                nextIndex = 0;
            }
            _curIndex = nextIndex;

            return GetPathByIndex(_curIndex);
        }

        /// <summary>
        /// 返回指定索引位置的Path
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetPathByIndex(int index)
        {
            if (index < 0
                || index > _listPath.Count - 1)
            {
                return null;
            }
            return _listPath[index];
        }
    }
}

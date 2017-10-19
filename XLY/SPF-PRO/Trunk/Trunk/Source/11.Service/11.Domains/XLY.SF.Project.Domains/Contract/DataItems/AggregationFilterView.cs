using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.DataFilter;
using XLY.SF.Project.DataFilter.Views;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Domains
{
    public class AggregationFilterView<TResult> : FilterView<TResult>
        where TResult : AbstractDataItem
    {
        #region Fields

        private String[] _dateTimeColumnsName;

        private Int32 _cursor;

        private Boolean _isLocked;

        #endregion

        #region Constructors

        public AggregationFilterView(IFilterable source, string key)
            : base(source)
        {
            Key = key;
        }

        #endregion

        #region Properties

        #region Args

        private FilterArgs[] _args;

        public FilterArgs[] Args
        {
            get { return _args; }
            set
            {
                CheckValidation();
                _args = value;
            }
        }

        #endregion

        #region PageSize

        private Int32 _pageSize = 2;

        public Int32 PageSize
        {
            get { return _pageSize; }
            set
            {
                CheckValidation();
                if (value <= 0) throw new ArgumentException("Must be positive integer.");
                _pageSize = value;
            }
        }

        #endregion

        public Int32 PageCount { get; private set; }

        /// <summary>
        /// 指示该行数据属于哪个节点
        /// </summary>
        public string Key { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 开始一次新的过滤。 执行该方法后，可以调用NextPage获取下一页的视图数据。
        /// </summary>
        public override void Filter()
        {
            _isLocked = true;
            Expression = CreateExpression(false, Args ?? throw new ArgumentNullException("Args"));
            base.Filter();
            PageCount = (Count + PageSize - 1) / PageSize;
            NextPage();
        }

        /// <summary>
        /// 执行Filter之后，执行该方法可以获取下一页的视图数据。
        /// </summary>
        /// <returns>如果存在下一页数据就返回true；否则返回false。</returns>
        public Boolean NextPage()
        {
            if (!_isLocked) return false;
            Expression = CreateExpression(true, Args);
            base.Filter();
            //查询成功移动游标
            if (Count > 0)
            {
                _cursor += Count;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 执行Filter之后，执行该方法可以获取下前一页的视图数据。
        /// </summary>
        /// <returns>如果存在前一页数据就返回true；否则返回false。</returns>
        public Boolean PreviousPage()
        {
            if (!_isLocked) return false;
            if (Count == 0) return false;
            //暂时保存原来的游标
            Int32 temp = _cursor;
            _cursor -= Count;
            Expression = CreateExpression(true, Args);
            base.Filter();
            //查询失败回滚游标
            if (Count <= 0)
            {
                _cursor = temp;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 重置过滤器。若当前正在执行过滤，在开始下一次新的过滤之前，需要执行该方法。
        /// </summary>
        public void Reset()
        {
            _cursor = 0;
            PageCount = 0;
            _isLocked = false;
        }

        #endregion

        #region Private

        private Expression CreateExpression(Boolean paging, params FilterArgs[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("XLYKey = '{0}' ", Key);
            if (args.Length == 0) return Expression.Constant(sb.ToString());
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case FilterByStringContainsArgs keywordArg:
                        sb.AppendFormat("AND XLYJson LIKE '%{0}%' ", keywordArg.PatternText);
                        break;
                    case FilterByRegexArgs regexArg:
                        String temp = regexArg.Regex.ToString();
                        sb.AppendFormat("AND RegexMatch('$XLYJson','${0}') ", temp.Replace("{", "{{").Replace("}", "}}"));
                        break;
                    case FilterByDateRangeArgs dateRangeArg:
                        SetDateTimeRangeSql(dateRangeArg.StartTime, dateRangeArg.EndTime, sb);
                        break;
                    case FilterByBookmarkArgs bookMarkArg:
                        sb.AppendFormat("AND BookMarkId = {0} ", bookMarkArg.BookmarkId);
                        break;
                    case FilterByEnumStateArgs stateArg:
                        sb.AppendFormat("AND DataState = '{0}' ", stateArg.State.ToString());
                        break;
                    default:
                        break;
                }
            }
            if (paging)
            {
                sb.AppendFormat("LIMIT {0} OFFSET {1} ", PageSize, _cursor);
            }
            //sb = sb.Remove(0, 3);
            return Expression.Constant(sb.ToString());
        }

        private String[] GetDateTimeColumnsName()
        {
            if (_dateTimeColumnsName != null) return _dateTimeColumnsName;
            PropertyInfo[] proppertyInfos = typeof(TResult).GetProperties();
            String[] columnsName = proppertyInfos.Where(x =>
            {
                DisplayAttribute atrribute = x.GetCustomAttribute<DisplayAttribute>();
                return atrribute != null &&
                    (atrribute.ColumnType == EnumColumnType.DateTime ||
                    x.PropertyType == typeof(DateTime?) ||
                    x.PropertyType == typeof(DateTime));
            }).Select(y =>
            {
                DisplayAttribute atrribute = y.GetCustomAttribute<DisplayAttribute>();
                return String.IsNullOrWhiteSpace(atrribute.Text) ? y.Name : atrribute.Text;
            }).ToArray();
            _dateTimeColumnsName = columnsName;
            return columnsName;
        }

        private void SetDateTimeRangeSql(DateTime? start, DateTime? end, StringBuilder sb)
        {
            String[] dateColumnsName = GetDateTimeColumnsName();
            if (dateColumnsName != null && dateColumnsName.Length != 0)
            {
                if (start == null && end == null)
                {
                }
                else if (start == null && end != null)
                {
                    foreach (String str in dateColumnsName)
                    {
                        sb.AppendFormat($"AND {str} < '{end.Value.ToString("yyyy-MM-dd HH:mm:ss")}' ");
                    }
                }
                else if (start != null && end == null)
                {
                    foreach (String str in dateColumnsName)
                    {
                        sb.AppendFormat($"AND {str} >= '{start.Value.ToString("yyyy-MM-dd HH:mm:ss")}' ");
                    }
                }
                else
                {
                    foreach (String str in dateColumnsName)
                    {
                        sb.AppendFormat($"AND {str} >= '{start.Value.ToString("yyyy-MM-dd HH:mm:ss")}'AND {str} < '{end.Value.ToString("yyyy-MM-dd HH:mm:ss")}' ");
                    }
                }
            }
        }

        private void CheckValidation()
        {
            if (_isLocked)
            {
                throw new InvalidOperationException("Can't change value,because filter is runing,please call Reset first.");
            }
        }

        #endregion

        #endregion
    }
}

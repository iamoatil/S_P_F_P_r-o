using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.DataFilter.Asist
{
    /// <summary>
    /// 表达式转SQL语句辅助类。
    /// </summary>
    internal static class SQLExpressionConverter
    {
        public static String GetSelectionSql(Expression expression,String tableName)
        {
            if (expression.NodeType != ExpressionType.Constant) return null;
            if (expression.Type != typeof(String)) return null;
            ConstantExpression constantExpression = (ConstantExpression)expression;
            String str = constantExpression.Value as String;
            if (String.IsNullOrWhiteSpace(str))
            {
                return $"SELECT * FROM {tableName}";
            }
            return $"SELECT * FROM {tableName} WHERE {str}";
        }

        public static String GetCountSql(Expression expression, String tableName)
        {
            if (expression.NodeType != ExpressionType.Constant) return null;
            if (expression.Type != typeof(String)) return null;
            ConstantExpression constantExpression = (ConstantExpression)expression;
            String str = (String)constantExpression.Value;
            if (String.IsNullOrWhiteSpace(str))
            {
                return $"SELECT COUNT(*) FROM {tableName}";
            }
            return $"SELECT COUNT(*) FROM {tableName} WHERE {str}";
        }

        #region Private

        private static String ConvertBinaryExpressionToSql(Expression expression)
        {
            String conjunction = null;
            switch (expression.NodeType)
            {
                //case ExpressionType.Call when expression.Method.Name == "Contains":
                //    conjunction = "LIKE";
                //    break;
                case ExpressionType.Or:
                    conjunction = "OR";
                    break;
                case ExpressionType.And:
                    conjunction = "AND";
                    break;
                case ExpressionType.Equal:
                    conjunction = "=";
                    break;
                case ExpressionType.GreaterThan:
                    conjunction = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    conjunction = ">=";
                    break;
                case ExpressionType.LessThan:
                    conjunction = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    conjunction = "<=";
                    break;
                case ExpressionType.NotEqual:
                    conjunction = "<>";
                    break;
                default:
                    throw new InvalidCastException(expression.NodeType.ToString());
            }
            return conjunction;
        }

        #endregion
    }
}

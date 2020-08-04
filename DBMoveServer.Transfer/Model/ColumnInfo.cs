using System.Collections;
using System.Collections.Generic;

namespace DBMoveServer.Transfer.Model
{
    public class ColumnInfo
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 是否可null
        /// </summary>
        public string Nullable { get; set; }

        /// <summary>
        /// 精度
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsIdentity { get; set; } = false;

        public string Comment { get; set; }

        public bool DefaultValueCheck
        {
            get
            {
                if (DefaultValue == null)
                    return false;

                if (TypeName == "int")
                    return int.TryParse(DefaultValue.Trim('\''), out _);

                if (TypeName == "decimal")
                    return double.TryParse(DefaultValue.Trim('\''), out _);

                return true;
            }
        }

        public bool DefaultValueHasQuotationMarks
        {
            get
            {
                return TypeName != "int" && TypeName != "decimal" && TypeName != "bit" && TypeName != "boolean";
            }
        }
    }

    public class ColumnCollection : IEnumerable
    {
        public ColumnCollection()
        {
            columns = new Dictionary<string, ColumnInfo>();
        }

        private Dictionary<string, ColumnInfo> columns;

        public int Count => columns.Count;

        public void AddColumn(ColumnInfo column)
        {
            if (!columns.ContainsKey(column.Name))
                columns.Add(column.Name, column);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return columns.GetEnumerator();
        }

        internal void SetComment(CommentInfo commentInfo)
        {
            if (columns.ContainsKey(commentInfo.ColumnName))
                columns[commentInfo.ColumnName].Comment = commentInfo.Comment;
        }
    }
}

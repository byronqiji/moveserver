using DBMoveServer.Transfer.Model;
using System.Collections.Generic;
using System.Text;

namespace DBMoveServer.Transfer.TargetServer
{
    public class TargetMysqlServer : ITargetServer
    {
        public DatabaseInfo DatabaseInfo { get; set; }

        public string CreateSql()
        {
            StringBuilder sqlSB = new StringBuilder();
            foreach (TableInfo table in DatabaseInfo)
            {
                sqlSB.AppendLine($"drop table if exists `{table.Name.ToLower()}`;");
                sqlSB.AppendLine($"create table `{table.Name.ToLower()}`(");

                int i = 0;
                foreach (KeyValuePair<string, ColumnInfo> column in table.ColumnCollection)
                {
                    if (i < table.ColumnCollection.Count - 1)
                        sqlSB.AppendLine(CreateColumn(column.Value) + ",");
                    else
                        sqlSB.AppendLine(CreateColumn(column.Value));

                    ++i;
                }
                sqlSB.AppendLine(");");

                foreach (IndexInfo index in table.IndexCollection)
                {
                    sqlSB.AppendLine(CreateIndex(index));
                }

                if (!string.IsNullOrEmpty(table.Comment))
                {
                    sqlSB.AppendLine($"alter table `{table.Name.ToLower()}` comment '{table.Comment}';");
                }

                sqlSB.AppendLine();
            }

            return sqlSB.ToString();
        }

        private string CreateIndex(IndexInfo index)
        {
            if (index.IndexType.Contains("clustered") && index.IndexType.Contains("unique") && index.IndexType.Contains("primary"))
            {
                return $"alter table `{index.Table.Name.ToLower()}` add primary key(`{string.Join("`,`", index.IndexKyeArr).Replace(" ", string.Empty).ToLower()}`);";
            }

            if (index.IndexType.Contains("unique"))
            {
                return $"create unique index index_{index.IndexKeys.Replace(",", "_").Replace(" ", string.Empty)}_{index.Table.Name} on `{index.Table.Name.ToLower()}`(`{string.Join("`,`", index.IndexKyeArr).Replace(" ", string.Empty).ToLower()}`);";
            }

            return $"create index index_{index.IndexKeys.Replace(",", "_").Replace(" ", string.Empty)}_{index.Table.Name} on `{index.Table.Name.ToLower()}`(`{string.Join("`,`", index.IndexKyeArr).Replace(" ", string.Empty).ToLower()}`);";
        }

        private string CreateColumn(ColumnInfo column)
        {
            string defaultInfo = string.Empty;
            string typeInfo;
            if (column.TypeName == "nvarchar" || column.TypeName == "varchar")
                typeInfo = $"varchar({column.Length})";
            else if (column.TypeName == "decimal")
                typeInfo = $"decimal({column.Length},{column.Precision})";
            else if (column.IsIdentity)
                typeInfo = "SERIAL";
            else if (column.TypeName == "uniqueidentifier")
                typeInfo = "char(36)";
            else if (column.TypeName == "binary")
                typeInfo = "blob";
            else if (column.TypeName == "bit")
                typeInfo = $"bit({column.Length})";
            else
                typeInfo = column.TypeName;

            if (column.DefaultValueCheck)
            {
                if (column.DefaultValue.Contains("'"))
                    defaultInfo = $"default {column.DefaultValue}";
                else
                    defaultInfo = $"default '{column.DefaultValue}'";

                if (!column.DefaultValueHasQuotationMarks)
                    defaultInfo = defaultInfo.Replace("'", string.Empty);

                if (column.TypeName == "text")
                    defaultInfo = string.Empty;
            }

            string comment = string.Empty;
            if (!string.IsNullOrEmpty(column.Comment))
                comment = $"comment '{column.Comment}'";

            return $"`{column.Name.ToLower()}` {typeInfo} {column.Nullable} {defaultInfo} {comment}";
        }
    }
}

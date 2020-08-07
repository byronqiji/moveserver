using DBMoveServer.Transfer.Model;
using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;
using System.Text;

namespace DBMoveServer.Transfer.TargetServer
{
    public class TargetMSServer : ITargetServer
    {
        public DatabaseInfo DatabaseInfo { get; set; }

        public string CreateSql()
        {
            StringBuilder sqlSB = new StringBuilder();
            foreach (TableInfo table in DatabaseInfo)
            {
                sqlSB.AppendLine($"if object_id(N'{table.Name}', N'U') is not null drop table [{table.Name}];");
                sqlSB.AppendLine($"create table [{table.Name}](");

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

                foreach (CommentInfo comment in table.CommentCollection)
                {
                    sqlSB.AppendLine(CreateComment(comment));
                }

                if (!string.IsNullOrEmpty(table.Comment))
                {
                    sqlSB.AppendLine($"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{table.Comment}',  @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'{table.Name}';");
                }

                sqlSB.AppendLine();
            }

            return sqlSB.ToString();
        }

        private string CreateComment(CommentInfo comment)
        {
            return $"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{comment.Comment}',  @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'{comment.Table.Name}', @level2type=N'column', @level2name=N'{comment.ColumnName}';";
        }

        private string CreateIndex(IndexInfo index)
        {
            if (index.IndexType.Contains("clustered") && index.IndexType.Contains("unique") && index.IndexType.Contains("primary")) //
            {
                return $"alter table {index.Table.Name} add constraint pk_{index.Table.Name} primary key ([{string.Join("],[", index.IndexKyeArr).Replace(" ", string.Empty)}]);";
            }

            if (index.IndexType.Contains("unique"))
            {
                return $"create unique index {index.IndexName} on {index.Table.Name} ([{string.Join("],[", index.IndexKyeArr).Replace(" ", string.Empty)}] ASC) with(PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON);";
            }

            return $"create index {index.IndexName} on {index.Table.Name} ([{string.Join("],[", index.IndexKyeArr).Replace(" ", string.Empty)}] ASC) with(PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON);";
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
                typeInfo = "int identity(1,1)";
            else if (column.TypeName == "uniqueidentifier")
                typeInfo = "char(36)";
            else if (column.TypeName == "binary")
                typeInfo = "image";
            else if (column.TypeName == "boolean")
                typeInfo = "bit";
            else if (column.TypeName == "text")
                typeInfo = "nvarchar(max)";
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
                else if (column.TypeName == "boolean")
                {
                    if (column.DefaultValue.ToLower() == "false")
                        defaultInfo = "default 0";
                    else if (column.DefaultValue.ToLower() == "true")
                        defaultInfo = "default 1";
                }
            }

            return $"[{column.Name}] {typeInfo} {column.Nullable} {defaultInfo}";
        }
    }
}

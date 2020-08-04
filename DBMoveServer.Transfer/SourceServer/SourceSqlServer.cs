using DBMoveServer.Database;
using DBMoveServer.Transfer.Model;
using System;
using System.Data;

namespace DBMoveServer.Transfer.Source
{
    public class SourceSqlServer : ISourceServer
    {
        public string Connect { get; set; }
        public ITargetServer TargetServer { get; set; }

        private SqlServerHelper helper;

        public DatabaseInfo CreateDatabaseInfo()
        {
            DatabaseInfo database = new DatabaseInfo();
            using (helper = new SqlServerHelper(Connect))
            {
                string sql = "select name from sysobjects where xtype='u' order by name;";
                DataTable nameTable = helper.GetDataTable(sql);
                
                for (int i = 0; i < nameTable.Rows.Count; ++i)
                {
                    database.AddTable(CreateTable(nameTable.Rows[i][0].ToString()));
                }
            }
            return database;
        }

        private TableInfo CreateTable(string tableName)
        {
            TableInfo table = new TableInfo()
            {
                Name = tableName
            };
            string sql = $@"SELECT A.name AS table_name, C.value AS table_description
                            FROM sys.tables A
                            INNER JOIN sys.extended_properties C ON C.major_id = A.object_id AND C.minor_id = 0
                            WHERE A.name = '{tableName}';";
            DataTable commentDT = helper.GetDataTable(sql);
            if (commentDT.Rows.Count > 0)
                table.Comment = commentDT.Rows[0]["table_description"].ToString();

            sql = $"sp_columns {tableName}";

            DataTable columnTable = helper.GetDataTable(sql);
            for (int c = 0; c < columnTable.Rows.Count; ++c)
            {
                table.ColumnCollection.AddColumn(CreateColumn(columnTable.Rows[c]));
            }

            sql = $"sp_helpindex {tableName};";
            DataTable indexTable = helper.GetDataTable(sql);
            for (int r = 0; r < indexTable.Rows.Count; ++r)
            {
                table.IndexCollection.AddIndex(CreateIndex(indexTable.Rows[r], table));
            }

            sql = $@"SELECT B.name AS column_name, C.value AS column_description
                    FROM sys.tables A
                    INNER JOIN sys.columns B ON B.object_id = A.object_id
                    INNER JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id
                    WHERE A.name = '{tableName}';";
            DataTable commentTable = helper.GetDataTable(sql);
            for (int r = 0; r < commentTable.Rows.Count; ++r)
            {
                CommentInfo commentInfo = CreateCommentInfo(commentTable.Rows[r], table);
                table.CommentCollection.AddComment(commentInfo);
                table.ColumnCollection.SetComment(commentInfo);
            }

            return table;
        }

        private CommentInfo CreateCommentInfo(DataRow row, TableInfo table)
        {
            CommentInfo comment = new CommentInfo()
            {
                ColumnName = row["column_name"].ToString(),
                Comment = row["column_description"].ToString(),
                Table = table
            };

            return comment;
        }

        private IndexInfo CreateIndex(DataRow row, TableInfo table)
        {
            // clustered, nonclustered, unique, primary,
            IndexInfo index = new IndexInfo()
            {
                IndexKeys = row["index_keys"].ToString(),
                IndexType = row["index_description"].ToString().Replace(" key located on PRIMARY", string.Empty),
                Table = table
            };

            return index;
        }

        private ColumnInfo CreateColumn(DataRow row)
        {
            ColumnInfo column = new ColumnInfo()
            {
                Name = row["COLUMN_NAME"].ToString(),
                Nullable = row["NULLABLE"].ToString() == "0" ? "not null" : "null"
            };

            if (row["COLUMN_DEF"] != DBNull.Value &&
                row["COLUMN_DEF"].ToString().ToLower() != "(newid())" &&
                row["COLUMN_DEF"].ToString().ToLower() != "(getdate())")
                column.DefaultValue = row["COLUMN_DEF"].ToString().TrimStart('(').TrimEnd(')');
            else
                column.DefaultValue = null;

            if (row["PRECISION"] != DBNull.Value)
                column.Length = Convert.ToInt32(row["PRECISION"]);

            if (row["SCALE"] != DBNull.Value)
                column.Precision = Convert.ToInt32(row["SCALE"]);

            string typeName = row["TYPE_NAME"].ToString();
            if (typeName.Contains("identity"))
            {
                column.IsIdentity = true;
                typeName = typeName.Replace("identity", string.Empty);
            }

            if (typeName == "datetime2")
            {
                typeName = "datetime";
                column.Length = 7;
            }
            else if (typeName == "datetime")
                column.Length = 3;
            else if (typeName == "ntext")
                typeName = "text";
            else if (typeName == "image")
                typeName = "binary";

            column.TypeName = typeName;
            return column;
        }
    }
}

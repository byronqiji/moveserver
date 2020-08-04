using DBMoveServer.Database;
using DBMoveServer.Transfer.Model;
using System;
using System.Data;

namespace DBMoveServer.Transfer.SourceServer
{
    public class SourcePostgreServer : ISourceServer
    {
        public string Connect { get; set; }

        public ITargetServer TargetServer { get; set; }

        private PostgreSqlHelper helper;

        public DatabaseInfo CreateDatabaseInfo()
        {
            DatabaseInfo database = new DatabaseInfo();
            using (helper = new PostgreSqlHelper(Connect))
            {
                string sql = @"SELECT tablename FROM pg_tables 
                                WHERE tablename NOT LIKE 'pg%'  
                                AND tablename NOT LIKE 'sql_%'
                                ORDER BY tablename;";
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

            string sql = $@"select 
                            ordinal_position as Colorder,
                            column_name as ColumnName,
                            data_type as TypeName,
                            coalesce(character_maximum_length,numeric_precision,-1) as Length,
                            numeric_scale as Scale,
                            case is_nullable when 'NO' then 0 else 1 end as CanNull,
                            column_default as DefaultVal,
                            case  when position('nextval' in column_default)>0 then 1 else 0 end as IsIdentity, 
                            case when b.pk_name is null then 0 else 1 end as IsPK,
                            c.DeText as colcomment
                            from information_schema.columns 
                            left join (
                                select pg_attr.attname as colname,pg_constraint.conname as pk_name from pg_constraint  
                                inner join pg_class on pg_constraint.conrelid = pg_class.oid 
                                inner join pg_attribute pg_attr on pg_attr.attrelid = pg_class.oid and  pg_attr.attnum = pg_constraint.conkey[1] 
                                inner join pg_type on pg_type.oid = pg_attr.atttypid
                                where pg_class.relname = '{tableName}' and pg_constraint.contype='p' 
                            ) b on b.colname = information_schema.columns.column_name
                            left join (
                                select attname,description as DeText from pg_class
                                left join pg_attribute pg_attr on pg_attr.attrelid= pg_class.oid
                                left join pg_description pg_desc on pg_desc.objoid = pg_attr.attrelid and pg_desc.objsubid=pg_attr.attnum
                                where pg_attr.attnum>0 and pg_attr.attrelid=pg_class.oid and pg_class.relname='{tableName}'
                            )c on c.attname = information_schema.columns.column_name
                            where table_schema='public' and table_name='{tableName}' order by ordinal_position asc;";

            DataTable columnTable = helper.GetDataTable(sql);
            for (int c = 0; c < columnTable.Rows.Count; ++c)
            {
                ColumnInfo column = CreateColumn(columnTable.Rows[c]);
                table.ColumnCollection.AddColumn(column);
                table.CommentCollection.AddComment(CreateComment(column, table));
            }

            sql = $"select * from pg_indexes where tablename='{tableName.ToLower()}'";
            DataTable indexTable = helper.GetDataTable(sql);
            for (int i = 0; i < indexTable.Rows.Count; ++i)
            {
                table.IndexCollection.AddIndex(CreateIndex(indexTable.Rows[i], table));
            }

            return table;
        }

        private IndexInfo CreateIndex(DataRow row, TableInfo table)
        {
            IndexInfo index = new IndexInfo()
            {
                Table = table,
                IndexType = string.Empty
            };

            string def = row["indexdef"].ToString();
            index.IndexKeys = def.Split('(')[1].TrimEnd(')');

            // if (index.IndexType.Contains("clustered") && index.IndexType.Contains("unique") && index.IndexType.Contains("primary"))
            // CREATE UNIQUE INDEX loginusers_pkey ON public.loginusers USING btree (clusterid)
            if (def.Contains("_pkey"))
            {
                index.IndexType = "clustered unique primary";
            }
            else if (def.ToLower().Contains("unique"))
            {
                index.IndexType = "unique";
            }

            return index;
        }

        private CommentInfo CreateComment(ColumnInfo column, TableInfo table)
        {
            CommentInfo comment = new CommentInfo()
            {
                Table = table,
                ColumnName = column.Name,
                Comment = column.Comment
            };

            return comment;
        }

        private ColumnInfo CreateColumn(DataRow row)
        {
            ColumnInfo column = new ColumnInfo()
            {
                Name = row["columnname"].ToString(),
                Nullable = row["cannull"].ToString() == "0" ? "not null" : "null",
                IsIdentity = row["isidentity"].ToString() == "1"
            };

            column.Length = Convert.ToInt32(row["length"]);
            if (row["scale"] != DBNull.Value)
                column.Precision = Convert.ToInt32(row["scale"]);

            switch (row["typename"].ToString().ToLower())
            {
                case "uuid":
                    column.TypeName = "uniqueidentifier";
                    break;
                case "timestamp without time zone":
                    column.TypeName = "datetime";
                    break;
                case "character varying":
                    column.TypeName = "varchar";
                    break;
                case "integer":
                    column.TypeName = "int";
                    break;
                case "numeric":
                    column.TypeName = "decimal";
                    break;
                case "bytea":
                    column.TypeName = "binary";
                    break;
                default:
                    column.TypeName = row["typename"].ToString().ToLower();
                    break;
            }

            if (row["colcomment"] != DBNull.Value)
                column.Comment = row["colcomment"].ToString();

            if (row["defaultval"] != DBNull.Value && !column.IsIdentity)
            {
                string def = row["defaultval"].ToString();
                if (def.Contains("'::"))
                {
                    def = def.Split(':')[0];
                }

                column.DefaultValue = def;
            }

            return column;
        }
    }
}

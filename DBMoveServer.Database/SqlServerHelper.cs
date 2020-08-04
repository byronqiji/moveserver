using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DBMoveServer.Database
{
    public class SqlServerHelper : IDisposable
    {
        SqlConnection conn;
        SqlTransaction tran;
        SqlCommand cmd;

        /// <summary>
        /// 创建PostgreSqlHelper
        /// </summary>
        /// <param name="connstring">数据库连接字符串</param>
        /// <returns></returns>
        public static SqlServerHelper Create(string connstring)
        {
            return new SqlServerHelper(connstring);
        }

        public SqlServerHelper(string connStr)
        {
            conn = new SqlConnection(connStr);

            cmd = new SqlCommand()
            {
                Connection = conn,
                CommandType = CommandType.Text
            };
        }

        public void Commit()
        {
            if (tran != null)
                tran.Commit();
        }

        public void Rollback()
        {
            if (tran != null)
                tran.Rollback();
        }

        public void BeginTransaction()
        {
            if (conn != null)
                tran = conn.BeginTransaction();
        }

        public void Dispose()
        {
            if (conn != null)
                conn.Dispose();
        }

        /// <summary>
        /// 打开服务器
        /// </summary>
        public bool Open()
        {
            bool isOpen = false;
            if (conn.State != ConnectionState.Open)
                conn.Open();

            if (conn.State == ConnectionState.Open)
                isOpen = true;

            return isOpen;
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Close()
        {
            if (conn.State != ConnectionState.Closed)
                conn.Close();
        }

        public int ExecuteNonQuery(string sql, List<SqlParameter> paramList = null)
        {
            cmd.CommandText = sql;
            SetParameters(paramList);

            return cmd.ExecuteNonQuery();
        }

        public DataTable GetDataTable(string sql, List<SqlParameter> paramList = null)
        {
            cmd.CommandText = sql;
            SetParameters(paramList);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        private void SetParameters(List<SqlParameter> paramList)
        {
            cmd?.Parameters?.Clear();
            if (paramList != null)
            {
                foreach (SqlParameter param in paramList)
                {
                    cmd.Parameters.Add(param);
                }
            }
        }
        internal DataSet GetDataSet(string sql, List<SqlParameter> paramList = null)
        {
            cmd.CommandText = sql;
            SetParameters(paramList);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return ds;
        }

        internal object ExecuteScalar(string sql, List<SqlParameter> paramList = null)
        {
            cmd.CommandText = sql;
            SetParameters(paramList);

            return cmd.ExecuteScalar();
        }
    }
}

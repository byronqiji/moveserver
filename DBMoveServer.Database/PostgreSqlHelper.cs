using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace DBMoveServer.Database
{
    public class PostgreSqlHelper : IDisposable
    {
        NpgsqlConnection conn;
        NpgsqlTransaction tran;
        NpgsqlCommand cmd;

        /// <summary>
        /// 创建PostgreSqlHelper
        /// </summary>
        /// <param name="connstring">数据库连接字符串</param>
        /// <returns></returns>
        internal static PostgreSqlHelper Create(string connstring)
        {
            return new PostgreSqlHelper(connstring);
        }

        public PostgreSqlHelper(string connStr)
        {
            conn = new NpgsqlConnection(connStr);

            cmd = new NpgsqlCommand()
            {
                Connection = conn,
                CommandType = CommandType.Text
            };
        }

        internal void Commit()
        {
            if (tran != null)
                tran.Commit();
        }

        internal void Rollback()
        {
            if (tran != null)
                tran.Rollback();
        }

        internal void BeginTransaction()
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

        internal int ExecuteNonQuery(string sql, List<NpgsqlParameter> paramList = null)
        {
            cmd.CommandText = sql;
            SetParameters(paramList);

            return cmd.ExecuteNonQuery();
        }

        public DataTable GetDataTable(string sql, List<NpgsqlParameter> paramList = null)
        {
            cmd.CommandText = sql;
            SetParameters(paramList);

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        private void SetParameters(List<NpgsqlParameter> paramList)
        {
            cmd?.Parameters?.Clear();
            if (paramList != null)
            {
                foreach (NpgsqlParameter param in paramList)
                {
                    cmd.Parameters.Add(param);
                }
            }
        }
    }
}

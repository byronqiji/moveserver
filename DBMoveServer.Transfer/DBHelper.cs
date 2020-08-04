using DBMoveServer.Transfer.APIModel;
using System;
using System.IO;

namespace DBMoveServer.Transfer
{
    public class DBHelper
    {
        public string CreateTables(TransferRequest connect)
        {
            ISourceServer source = SourceFactory.Instance[connect.SourceDB];
            ITargetServer target = TargetFactory.Instance[connect.TargetDB];

            source.Connect = connect.Connection;
            target.DatabaseInfo = source.CreateDatabaseInfo();

            string sql = target.CreateSql();

            string tempPath = DateTime.Now.ToString("yyyyMMdd");

            string fileName = $"{connect.SourceDB}2{connect.TargetDB}_{DateTime.Now:HHmmss}.sql";

            string dir = Path.Combine(AppConfiguration.Instance.SqlFilePath, tempPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string path = Path.Combine(AppConfiguration.Instance.SqlFilePath, tempPath, fileName);

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(sql);
            }

            return path;
        }
    }
}

using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;

namespace DBMoveServer.Transfer
{
    public class AppConfiguration
    {
        private static AppConfiguration single;

        public static AppConfiguration Instance
        {
            get
            {
                if (single != null)
                    return single;

                AppConfiguration temp = new AppConfiguration();
                Interlocked.CompareExchange(ref single, temp, null);

                return single;
            }
        }

        /// <summary>
        /// 站点运行基础路径
        /// </summary>
        public string BasePath { get; private set; }

        private AppConfiguration()
        {
            Type type = typeof(AppConfiguration);
            BasePath = Path.GetDirectoryName(type.Assembly.Location);
        }

        public void Init(IConfiguration configuration)
        {
            SqlFilePath = configuration.GetValue("SqlPath", "/sql/");
        }

        /// <summary>
        /// 数据库连接配置
        /// </summary>
        public string SqlFilePath { get; private set; }
    }
}

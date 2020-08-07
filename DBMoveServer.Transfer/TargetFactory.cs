using DBMoveServer.Transfer.Model;
using DBMoveServer.Transfer.TargetServer;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DBMoveServer.Transfer
{
    public class TargetFactory
    {
        private static TargetFactory single;
        public static TargetFactory Instance
        {
            get
            {
                if (single != null)
                    return single;

                TargetFactory temp = new TargetFactory();
                Interlocked.CompareExchange(ref single, temp, null);

                return single;
            }
        }

        readonly Dictionary<DatabaseType, Func<ITargetServer>> targetServerDic;

        private TargetFactory()
        {
            targetServerDic = new Dictionary<DatabaseType, Func<ITargetServer>>()
            {
                { DatabaseType.PostgreSql, () => { return new TargetPostgreServer(); } },
                { DatabaseType.MySql, () => { return new TargetMysqlServer(); } },
                { DatabaseType.SqlServer, () => { return new TargetMSServer(); } }
            };
        }

        public ITargetServer this[DatabaseType key]
        {
            get
            {
                if (targetServerDic.ContainsKey(key))
                    return targetServerDic[key]();

                return null;
            }
        }
    }
}

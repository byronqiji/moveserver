using DBMoveServer.Transfer.Model;
using DBMoveServer.Transfer.Source;
using DBMoveServer.Transfer.SourceServer;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DBMoveServer.Transfer
{
    public class SourceFactory
    {
        private static SourceFactory single;
        public static SourceFactory Instance
        {
            get
            {
                if (single != null)
                    return single;

                SourceFactory temp = new SourceFactory();
                Interlocked.CompareExchange(ref single, temp, null);

                return single;
            }
        }

        readonly Dictionary<DatabaseType, Func<ISourceServer>> sourceServerDic;

        private SourceFactory()
        {
            sourceServerDic = new Dictionary<DatabaseType, Func<ISourceServer>>()
            {
                { DatabaseType.SqlServer, () => { return new SourceSqlServer(); } },
                { DatabaseType.PostgreSql, () => { return new SourcePostgreServer(); } }
            };
        }

        public ISourceServer this[DatabaseType key]
        {
            get
            {
                if (sourceServerDic.ContainsKey(key))
                    return sourceServerDic[key]();

                return null;
            }
        }
    }
}

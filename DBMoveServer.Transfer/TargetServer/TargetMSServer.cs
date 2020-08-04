using DBMoveServer.Transfer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBMoveServer.Transfer.TargetServer
{
    public class TargetMSServer : ITargetServer
    {
        public DatabaseInfo DatabaseInfo { get; set; }

        public string CreateSql()
        {
            throw new NotImplementedException();
        }
    }
}

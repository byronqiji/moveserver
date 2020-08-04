using DBMoveServer.Transfer.Model;

namespace DBMoveServer.Transfer.APIModel
{
    public class TransferRequest
    {
        public string Connection { get; set; }

        public DatabaseType SourceDB { get; set; }

        public DatabaseType TargetDB { get; set; }
    }
}

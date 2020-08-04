using DBMoveServer.Transfer.Model;

namespace DBMoveServer.Transfer
{
    public interface ISourceServer
    {
        string Connect { get; set; }

        DatabaseInfo CreateDatabaseInfo();

        ITargetServer TargetServer { get; set; }
    }
}

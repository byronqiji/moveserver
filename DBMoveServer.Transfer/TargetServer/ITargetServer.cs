using DBMoveServer.Transfer.Model;

namespace DBMoveServer.Transfer
{
    public interface ITargetServer
    {
        DatabaseInfo DatabaseInfo { get; set; }

        string CreateSql();
    }
}

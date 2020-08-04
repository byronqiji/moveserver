using DBMoveServer.Transfer;
using DBMoveServer.Transfer.APIModel;
using DBMoveServer.Transfer.Model;
using NUnit.Framework;

namespace DBMoveServer.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            DBHelper helper = new DBHelper();
            //helper.CreateTables("Data Source=10.10.10.21;Initial Catalog=NewBMData2; UID=sa; PWD=bmsj@021;");

            TransferRequest tr = new TransferRequest()
            {
                Connection = "Server=10.10.10.134;Port=5432;Database=bmapidb_test;User ID=apiuser;Password=7LMg104uwFmY;pooling=true;",
                SourceDB = DatabaseType.SqlServer,
                TargetDB = DatabaseType.PostgreSql
            };

            helper.CreateTables(tr);
        }
    }
}
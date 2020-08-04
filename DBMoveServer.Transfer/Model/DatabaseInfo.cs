using System.Collections;
using System.Collections.Generic;

namespace DBMoveServer.Transfer.Model
{
    public class DatabaseInfo : IEnumerable<TableInfo>
    {
        public DatabaseInfo()
        {
            tables = new List<TableInfo>();
        }

        private List<TableInfo> tables { get; set; }

        public IEnumerator<TableInfo> GetEnumerator()
        {
            return tables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return tables.GetEnumerator();
        }

        internal void AddTable(TableInfo table)
        {
            tables.Add(table);
        }
    }
}

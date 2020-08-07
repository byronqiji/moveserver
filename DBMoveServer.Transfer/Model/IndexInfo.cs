using System.Collections;
using System.Collections.Generic;

namespace DBMoveServer.Transfer.Model
{
    public class IndexInfo
    {
        public string IndexType { get; set; }

        public string IndexKeys { get; set; }

        public string[] IndexKyeArr => IndexKeys?.Split(',');

        public TableInfo Table { get; set; }

        public string IndexName => $"index_{IndexKeys.Replace(",", "_").Replace(" ", string.Empty)}_{Table.Name}";

        public override string ToString()
        {
            return IndexType + IndexKeys;
        }
    }

    public class IndexCollection : IEnumerable<IndexInfo>
    {
        public IndexCollection()
        {
            indexs = new List<IndexInfo>();
        }

        private List<IndexInfo> indexs;

        public IEnumerator<IndexInfo> GetEnumerator()
        {
            return indexs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return indexs.GetEnumerator();
        }

        public int Count => indexs.Count;

        public void AddIndex(IndexInfo index)
        {
            string info = index.ToString();
            foreach (IndexInfo item in indexs)
            {
                if (info == item.ToString())
                    return;
            }

            indexs.Add(index);
        }
    }
}

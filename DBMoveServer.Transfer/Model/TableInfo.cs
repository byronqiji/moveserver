namespace DBMoveServer.Transfer.Model
{
    public class TableInfo
    {
        public TableInfo()
        {
            ColumnCollection = new ColumnCollection();
            IndexCollection = new IndexCollection();
            CommentCollection = new CommentCollection();
        }

        public string Name { get; set; }

        public string Comment { get; set; }

        public ColumnCollection ColumnCollection { get; set; }

        public IndexCollection IndexCollection { get; set; }

        public CommentCollection CommentCollection { get; set; }
    }
}

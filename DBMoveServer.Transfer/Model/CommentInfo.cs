using System.Collections;
using System.Collections.Generic;

namespace DBMoveServer.Transfer.Model
{
    public class CommentInfo
    {
        public string ColumnName { get; set; }

        public string Comment { get; set; }

        public TableInfo Table { get; set; }
    }

    public class CommentCollection : IEnumerable<CommentInfo>
    {
        public CommentCollection()
        {
            comments = new List<CommentInfo>();
        }

        private List<CommentInfo> comments;

        public int Count => comments?.Count ?? 0;

        public void AddComment(CommentInfo comment)
        {
            comments.Add(comment);
        }

        public IEnumerator<CommentInfo> GetEnumerator()
        {
            return comments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return comments.GetEnumerator();
        }
    }
}

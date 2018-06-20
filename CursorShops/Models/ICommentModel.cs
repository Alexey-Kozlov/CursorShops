using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursorShops.Models
{
    public interface ICommentModel
    {
        DateTime CommentDate { get; set; }
        string Author { get; set; }
        string Text { get; set; }
        string SiteID { get; set; }
        string WebID { get; set; }
        string ListID { get; set; }
        string ID { get; set; }
    }
}

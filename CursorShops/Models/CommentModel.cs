using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CursorShops.Models
{
    public class CommentModel : ICommentModel
    {
        public DateTime CommentDate { get; set; }
        [Display(Name = "Автор задания")]
        public string Author { get; set; }
        public string AuthorDep { get; set; }
        public string Text { get; set; }
        public string SiteID { get; set; }
        public string WebID { get; set; }
        public string ListID { get; set; }
        public string ID { get; set; }
    }
}

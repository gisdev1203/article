using System;
using System.Data;

namespace DomainModel.ObjectList
{
    public class ArticleComments
    {
        public int Id { get; set; }
        public int Id_article { get; set; }
        public string Comments { get; set; }
        public DateTime Created_at { get; set; }
        public string Id_conversation { get; set; }
        public string Id_comment { get; set; }
        public DateTime Modified_at { get; set; }
        public int User_Id { get; set; }
    }
}

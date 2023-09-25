using System;
using System.Data;

namespace DomainModel.ObjectList
{
    public class ArticleComments
    {
        public int Id { get; set; }
        public int Article_Id { get; set; }
        public string Comments { get; set; }
        public DateTime Created_At { get; set; }
        public string Conversation_Uid { get; set; }
        public string Comment_Uid { get; set; }
        public DateTime Modified_At { get; set; }
    }
}

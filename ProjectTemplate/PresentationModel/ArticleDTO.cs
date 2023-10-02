using DomainModel.ObjectList;
using System.Collections.Generic;

namespace ProjectTemplate.PresentationModel
{
    public class ArticleDTO
    {
        public List<Article> Articles { get; set; } = new List<Article>();
        public Article Article { get; set; }
        public ArticleTemp ArticleTemp { get; set; }
        public List<ArticleForm> ArticleType { get; set; }
    }
}

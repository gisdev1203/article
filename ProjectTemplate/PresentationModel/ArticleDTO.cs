using DomainModel.ObjectList;
using System.Collections.Generic;

namespace ProjectTemplate.PresentationModel
{
    public class ArticleDTO
    {
        public List<ArticleObject> ArticleObjects { get; set; } = new List<ArticleObject>();
    }
}

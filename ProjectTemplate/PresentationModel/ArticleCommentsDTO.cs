using DomainModel.ObjectList;
using System;
using System.Collections.Generic;

namespace ProjectTemplate.PresentationModel
{
    public class ArticleCommentsDTO
    {
        public List<ArticleComments> ArticleComments { get; set; } = new List<ArticleComments>();
        public ArticleComments ArticleComment { get; set; }
    }
}

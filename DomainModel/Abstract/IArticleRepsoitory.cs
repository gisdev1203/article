using DomainModel.ObjectList;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel.Abstract
{
    public interface IArticleRepository
    {
        List<TestObject> GetDataFromTestTable(int id);
        List<Article> ListArticles(ArticleFilter filter);
        List<Article> GetArticle(int id);
        Article GetDataFromArticleTableLastOrDefault();
        int CreateNewArticle();
    }
}

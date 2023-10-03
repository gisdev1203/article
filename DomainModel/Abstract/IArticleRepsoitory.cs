using DomainModel.ObjectList;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel.Abstract
{
    public interface IArticleRepository
    {
        List<UserCustom> GetUserDataList();
        UserCustom GetUserCustom(int id);
        List<TestObject> GetDataFromTestTable(int id);
        List<Article> ListArticles(ArticleFilter filter);
        List<Article> GetArticle(int id, int user_id);
        Article GetDataFromArticleTableLastOrDefault();
        int CreateNewArticle(int article_type_id, int user_id);
        List<ArticleForm> ListArticleForm(ArticleFormFilter filter);
        List<ArticleForm> GetAllArticleType();
        ArticleForm GetArticleFormById(ArticleFormFilter filter);
        void UpdateArticle(Article article);
        void UpdateArticleForm(Article article);
        ArticleTemp GetArticleTemp(int id, int user_id);
        void UpdateArticleTemp(ArticleTemp articleTemp);
        void CreateNewArticleTemp(int id_article, int user_id);
        bool DeleteArticleTemp(int id_article, int user_id);
        int CreateArticleComments(ArticleComments articleComments);
        int EditArticleComments(ArticleComments articleComments);
        int ReplyArticleComments(ArticleComments articleComments);
        List<ArticleComments> ListArticleComments(ArticleCommentsFilter articleCommentsFilter);
        bool DeleteArticleComments(ArticleCommentsFilter articleComments);
        bool DeleteArticleConversation(ArticleCommentsFilter articleCommentsFilter);
        bool DeleteArticleConversations(ArticleCommentsFilter articleCommentsFilter);
    }
}
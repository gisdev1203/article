﻿using DomainModel.ObjectList;
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
        int CreateNewArticle(int article_type);
        List<ArticleForm> ListArticleForm(ArticleFormFilter filter);
        ArticleForm GetArticleFormById(ArticleFormFilter filter);
        void UpdateArticle(Article article);
        void UpdateArticleForm(Article article);
        ArticleTemp GetArticleTemp(int id);
        void UpdateArticleTemp(ArticleTemp articleTemp);
        void CreateNewArticleTemp(int articleId);
        int CreateArticleComments(ArticleComments articleComments);
        int EditArticleComments(ArticleComments articleComments);
        List<ArticleComments> ListArticleComments(ArticleCommentsFilter articleCommentsFilter);
    }
}
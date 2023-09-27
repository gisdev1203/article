using DomainModel.Abstract;
using DomainModel.ObjectList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Npgsql;
using NpgsqlTypes;
using static System.Net.WebRequestMethods;

namespace DomainModel.Concrete
{
    public class ArticleRepository : BaseRepository, IArticleRepository
    {
        public List<TestObject> GetDataFromTestTable(int id)
        {
            string sql = "SELECT id, name FROM test_table WHERE id = @A";
            return Query<TestObject>(sql, new { A = id });
        }

        public List<Article> ListArticles(ArticleFilter filter)
        {
            dynamic parameters = new ExpandoObject();
            string sql = "SELECT id, name, content, form_data FROM article WHERE is_deleted = false";

            if (filter.Id.HasValue)
            {
                parameters.A = filter.Id.Value;
                sql += " AND id = @A";
            }
            return Query<Article>(sql, parameters);
        }

        public List<Article> GetArticle(int id)
        {
            string sql = "SELECT id, name, content FROM article WHERE id = @A";
            return Query<Article>(sql, new { A = id });
        }

        public Article GetDataFromArticleTableLastOrDefault()
        {
            string sql = "SELECT id, name, content FROM article order by 1 desc limit 1";
            return QueryFirstOrDefault<Article>(sql);
        }

        public int CreateNewArticle(int article_type)
        {
            var newArticleName = "Article test";

            //TODO: We will change the article name later
            string sql = "INSERT INTO article(name, is_deleted, id_form) VALUES(@A, false, @B) RETURNING Id";
            return ExecuteScalar(sql, new { A = newArticleName, B = article_type });
        }

        public List<ArticleForm> ListArticleForm(ArticleFormFilter filter)
        {
            dynamic parameters = new ExpandoObject();
            string sql = "SELECT id, type, form_definition FROM article_form";
            if (!string.IsNullOrEmpty(filter.type))
            {
                parameters.A = filter.type;
                sql += " WHERE type = @A";
            }
            return Query<ArticleForm>(sql, parameters);
        }

        public ArticleForm GetArticleFormById(ArticleFormFilter filter)
        {
            dynamic parameters = new ExpandoObject();
            string sql = "SELECT id, type, form_definition FROM article_form";
            if (!string.IsNullOrEmpty(filter.type))
            {
                parameters.A = filter.type;
                sql += " WHERE id = @A";
            }
            return QueryFirstOrDefault<ArticleForm>(sql, parameters);
        }

        public void UpdateArticle(Article article)
        {
            string sql = "update article set content = @A where id = @B";
            Execute(sql, new { A = article.Content, B = article.Id });
        }

        public void UpdateArticleForm(Article article)
        {
            string sql = "update article set form_data = @A::json where id = @B";
            Execute(sql, new { A = article.Form_Data, B = article.Id });
        }

        public void CreateNewArticleTemp(int id_article)
        {
            string sql = "INSERT INTO article_temp(id_article, content) VALUES(@A, @B)";
            Execute(sql, new { A = id_article, B = "" });
        }

        public ArticleTemp GetArticleTemp(int id)
        {
            string sql = "SELECT id_article, content FROM article_temp WHERE id_article = @A";
            return QueryFirstOrDefault<ArticleTemp>(sql, new { A = id });
        }

        public void UpdateArticleTemp(ArticleTemp articleTemp)
        {
            string sql = "update article_temp set content = @A where id_article = @B";
            Execute(sql, new { A = articleTemp.Content, B = articleTemp.Id_article });
        }

        public int CreateArticleComments(ArticleComments articleComments)
        {
            string sql = "INSERT INTO article_comments(id_article, comments, created_at, id_conversation, id_comment, modified_at) " +
                            "VALUES(@A, @B, @C, @D, @E, @F) RETURNING Id";
            return Execute(sql, new 
            { 
                A = articleComments.Id_article, 
                B = articleComments.Comments, 
                C = articleComments.Created_at,
                D = articleComments.Id_conversation, 
                E = articleComments.Id_comment,
                F = articleComments.Modified_at
            });
        }

        public List<ArticleComments> ListArticleComments(ArticleCommentsFilter filter)
        {
            dynamic parameters = new ExpandoObject();
            string sql = "SELECT id, id_article, comments, created_at, id_conversation, id_comment, modified_at FROM article_comments";
            if (filter.Id != null)
            {
                parameters.A = filter.Id;
                parameters.B = filter.Id_conversation;
                sql += " WHERE id_article = @A and id_conversation  = @B";
            }
            return Query<ArticleComments>(sql, parameters);
        }

        public int EditArticleComments(ArticleComments articleComments)
        {
            dynamic parameters = new ExpandoObject();
            string sql = "update article_comments set comments = ";
            if(articleComments.Id_article > 0)
            {
                parameters.A = articleComments.Comments;
                parameters.B = articleComments.Id_article;
                parameters.C = articleComments.Id_conversation;
                parameters.D = articleComments.Id_comment;
                parameters.E = articleComments.Modified_at;
                sql += "@A WHERE id_article = @B and  id_conversation = @C and id_comment = @D and id_conversation = @E";
            }

            return Execute(sql, parameters);
        }

        public int ReplyArticleComments(ArticleComments articleComments)
        {
            string sql = "INSERT INTO article_comments(id_article, comments, created_at, id_conversation, id_comment, modified_at) " +
                            "VALUES(@A, @B, @C, @D, @E, @F) RETURNING Id";
            return Execute(sql, new
            {
                A = articleComments.Id_article,
                B = articleComments.Comments,
                C = articleComments.Created_at,
                D = articleComments.Id_conversation,
                E = articleComments.Id_comment,
                F = articleComments.Modified_at
            });
        }

        public bool DeleteArticleComments(ArticleCommentsFilter articleCommentsFilter)
        {
            string sql = "delete from article_comments where id_article = @A and id_conversation = @B and id_comment = @C";
            Execute(sql, new
            {
                A = articleCommentsFilter.Id,
                B = articleCommentsFilter.Id_conversation,
                C = articleCommentsFilter.Id_comment
            });

            return true; //TODO: need to handle delete
        }

        public bool DeleteArticleConversation(ArticleCommentsFilter articleCommentsFilter)
        {
            string sql = "delete from article_comments where id_article = @A and id_conversation = @B";
            Execute(sql, new
            {
                A = articleCommentsFilter.Id,
                B = articleCommentsFilter.Id_conversation
            });
            return true; //TODO: need to handle delete
        }

        public bool DeleteArticleConversations(ArticleCommentsFilter articleCommentsFilter)
        {
            string sql = "delete from article_comments where id_article = @A";
            Execute(sql, new
            {
                A = articleCommentsFilter.Id
            });
            return true; //TODO: need to handle delete
        }
    }
}

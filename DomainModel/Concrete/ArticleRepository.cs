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
            var articleObject = GetDataFromArticleTableLastOrDefault();
            var newArticleNumber = articleObject == null ? 1 : articleObject.Id + 1;
            var newArticleName = "Article test " + newArticleNumber;

            //TODO: We will change the article name later
            string sql = "INSERT INTO article(name, is_deleted, article_form_id) VALUES(@A, false, @B) RETURNING Id";
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

        public void CreateNewArticleTemp(int articleId)
        {
            string sql = "INSERT INTO article_temp(article_id, content) VALUES(@A, @B)";
            Execute(sql, new { A = articleId, B = "" });
        }

        public ArticleTemp GetArticleTemp(int id)
        {
            string sql = "SELECT article_id, content FROM article_temp WHERE article_id = @A";
            return QueryFirstOrDefault<ArticleTemp>(sql, new { A = id });
        }

        public void UpdateArticleTemp(ArticleTemp articleTemp)
        {
            string sql = "update article_temp set content = @A where article_id = @B";
            Execute(sql, new { A = articleTemp.Content, B = articleTemp.Article_Id });
        }

        public int CreateArticleComments(ArticleComments articleComments)
        {
            string sql = "INSERT INTO article_comments(article_id, comments, created_at, conversation_uid, comment_uid, modified_at) " +
                            "VALUES(@A, @B, @C, @D, @E, @F) RETURNING Id";
            return Execute(sql, new 
            { 
                A = articleComments.Article_Id, 
                B = articleComments.Comments, 
                C = articleComments.Created_At,
                D = articleComments.Conversation_Uid, 
                E = articleComments.Comment_Uid,
                F = articleComments.Modified_At
            });
        }

        public List<ArticleComments> ListArticleComments(ArticleCommentsFilter filter)
        {
            dynamic parameters = new ExpandoObject();
            string sql = "SELECT id, article_id, comments, created_at,conversation_uid, comment_uid, modified_at FROM article_comments";
            if (filter.Id != null)
            {
                parameters.A = filter.Id;
                parameters.B = filter.Conversation_Id;
                sql += " WHERE article_Id = @A and conversation_uid  = @B";
            }
            return Query<ArticleComments>(sql, parameters);
        }

        public int EditArticleComments(ArticleComments articleComments)
        {
            dynamic parameters = new ExpandoObject();
            string sql = "update article_comments set comments = ";
            if(articleComments.Article_Id > 0)
            {
                parameters.A = articleComments.Comments;
                parameters.B = articleComments.Article_Id;
                parameters.C = articleComments.Conversation_Uid;
                parameters.D = articleComments.Comment_Uid;
                parameters.E = articleComments.Modified_At;
                sql += "@A WHERE article_Id = @B and  conversation_uid = @C and comment_uid = @D and conversation_uid = @E";
            }

            return Execute(sql, parameters);
        }

        public int ReplyArticleComments(ArticleComments articleComments)
        {
            string sql = "INSERT INTO article_comments(article_id, comments, created_at, conversation_uid, comment_uid, modified_at) " +
                            "VALUES(@A, @B, @C, @D, @E, @F) RETURNING Id";
            return Execute(sql, new
            {
                A = articleComments.Article_Id,
                B = articleComments.Comments,
                C = articleComments.Created_At,
                D = articleComments.Conversation_Uid,
                E = articleComments.Comment_Uid,
                F = articleComments.Modified_At
            });
        }

        public bool DeleteArticleComments(ArticleCommentsFilter articleCommentsFilter)
        {
            string sql = "delete from article_comments where article_id = @A and conversation_uid = @B and comment_uid = @C";
            Execute(sql, new
            {
                A = articleCommentsFilter.Id,
                B = articleCommentsFilter.Conversation_Id,
                C = articleCommentsFilter.Comment_Uid
            });

            return true; //TODO: need to handle delete
        }

        public bool DeleteArticleConversation(ArticleCommentsFilter articleCommentsFilter)
        {
            string sql = "delete from article_comments where article_id = @A and conversation_uid = @B";
            Execute(sql, new
            {
                A = articleCommentsFilter.Id,
                B = articleCommentsFilter.Conversation_Id
            });
            return true; //TODO: need to handle delete
        }

        public bool DeleteArticleConversations(ArticleCommentsFilter articleCommentsFilter)
        {
            string sql = "delete from article_comments where article_id = @A";
            Execute(sql, new
            {
                A = articleCommentsFilter.Id
            });
            return true; //TODO: need to handle delete
        }
    }
}

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
using System.Reflection.Metadata;

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
            string sql = "SELECT id, name, content, form_data, user_id FROM article WHERE is_deleted = false AND user_id = @B";

            if (filter.Id.HasValue)
            {
                parameters.A = filter.Id.Value;
                
                sql += " AND id = @A ";
                //sql += " AND id = @A ";
            }
            parameters.B = filter.User_Id;

            return Query<Article>(sql, parameters);
        }


        public List<Article> GetArticle(int id, int user_id)
        {
            string sql = "SELECT id, name, content, user_id FROM article WHERE id = @A and user_id = @B";
            return Query<Article>(sql, new { A = id, B = user_id });
        }

        public Article GetDataFromArticleTableLastOrDefault()
        {
            string sql = "SELECT id, name, content FROM article order by 1 desc limit 1";
            return QueryFirstOrDefault<Article>(sql);
        }

        public int CreateNewArticle(int article_type, int user_id)
        {
            var newArticleName = "Article test";
            //TODO: We will change the article name later
            string sql = "INSERT INTO article(name, is_deleted, id_form, user_id) VALUES(@A, false, @B, @C) RETURNING Id";
            return ExecuteScalar(sql, new { A = newArticleName, B = article_type, C = user_id });
        }

        public List<ArticleForm> ListArticleForm(ArticleFormFilter filter)
        {
            dynamic parameters = new ExpandoObject();
            string sql = "SELECT id, type, form_definition FROM article_form";
            if (filter.Id > 0)
            {
                parameters.A = filter.Id;
                sql += " WHERE id = @A";
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

        public List<ArticleForm> GetAllArticleType()
        {
            dynamic parameters = new ExpandoObject();
            string sql = "SELECT id, type FROM article_form";
            return Query<ArticleForm>(sql, parameters);
        }

        public void UpdateArticle(Article article)
        {
            string sql = "update article set content = @A, form_data = @C::json where id = @B and user_id = @D";

            Execute(sql, new { A = article.Content, B = article.Id, C = article.Form_Data, D = article.User_Id });
        }

        public void UpdateArticleForm(Article article)
        {
            string sql = "update article set form_data = @A::json where id = @B and user_id = @C";
            Execute(sql, new { A = article.Form_Data, B = article.Id, C = article.User_Id });
        }

        public void CreateNewArticleTemp(int id_article, int user_id)
        {
            string sql = "INSERT INTO article_temp(id_article, content, user_id) VALUES(@A, @B, @C)";
            Execute(sql, new { A = id_article, B = "", C = user_id });
        }
        public bool DeleteArticleTemp(int id_article, int user_id)
        {
            string sql = "delete from article_temp where id_article = @A AND user_id = @B";
            Execute(sql, new
            {
                A = id_article,  
                B = user_id,
            });

            return true; //TODO: need to handle delete
        }

        public ArticleTemp GetArticleTemp(int id, int user_id)
        {
            string sql = "SELECT id_article, content, user_id FROM article_temp WHERE id_article = @A AND user_id = @B";
            return QueryFirstOrDefault<ArticleTemp>(sql, new { A = id, B = user_id });
        }

        public void UpdateArticleTemp(ArticleTemp articleTemp)
        {
            string sql = "update article_temp set content = @A where id_article = @B and user_id = @C";
            Execute(sql, new { A = articleTemp.Content, B = articleTemp.Id_article, C = articleTemp.User_Id });
        }

        public int CreateArticleComments(ArticleComments articleComments)
        {
            string sql = "INSERT INTO article_comments(id_article, comments, created_at, id_conversation, id_comment, modified_at, user_id) " +
                            "VALUES(@A, @B, @C, @D, @E, @F, @G) RETURNING Id";
            return Execute(sql, new 
            { 
                A = articleComments.Id_article, 
                B = articleComments.Comments, 
                C = articleComments.Created_at,
                D = articleComments.Id_conversation, 
                E = articleComments.Id_comment,
                F = articleComments.Modified_at,
                G = articleComments.User_Id,
            });
        }

        public List<ArticleComments> ListArticleComments(ArticleCommentsFilter filter)
        {
            dynamic parameters = new ExpandoObject();
            string sql = "SELECT id, id_article, comments, created_at, id_conversation, id_comment, modified_at, user_id FROM article_comments";
            if (filter.Id != null)
            {
                parameters.A = filter.Id;
                parameters.B = filter.Id_conversation;
                parameters.C = filter.User_Id;

                sql += " WHERE id_article = @A and id_conversation  = @B and user_id = @C ORDER BY id";
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
                parameters.E = articleComments.Modified_at;
                parameters.D = articleComments.Id_comment;
                parameters.F = articleComments.User_Id;

                if (articleComments.Id_comment == null)
                {                    
                    sql += "@A, modified_at = @E WHERE id_article = @B and  id_conversation = @C and  user_id = @F and id_comment is NULL";
                }
                else sql += "@A, modified_at = @E WHERE id_article = @B and  id_conversation = @C and  id_comment = @D user_id = @F";
            }

            return Execute(sql, parameters);
        }

        public int ReplyArticleComments(ArticleComments articleComments)
        {
            dynamic parameters = new ExpandoObject();

            if (articleComments.Id_article > 0)
            {
                parameters.A = articleComments.Id_article;
                parameters.B = articleComments.Comments;
                parameters.C = articleComments.Created_at;
                parameters.D = articleComments.Id_conversation;
                parameters.E = articleComments.Id_comment;
                parameters.F = articleComments.Modified_at;
                parameters.G = articleComments.User_Id;                
            }

            string sql = "INSERT INTO article_comments(id_article, comments, created_at, id_conversation, id_comment, modified_at, user_id) " +
                            "VALUES(@A, @B, @C, @D, @E, @F, @G) RETURNING Id";

            return Execute(sql, parameters);
        }

        public bool DeleteArticleComments(ArticleCommentsFilter articleCommentsFilter)
        {
            string sql = "delete from article_comments where id_article = @A and id_conversation = @B and id_comment = @C and user_id = @D";
            Execute(sql, new
            {
                A = articleCommentsFilter.Id,
                B = articleCommentsFilter.Id_conversation,
                C = articleCommentsFilter.Id_comment,
                D = articleCommentsFilter.User_Id
            });

            return true; //TODO: need to handle delete
        }

        public bool DeleteArticleConversation(ArticleCommentsFilter articleCommentsFilter)
        {
            string sql = "delete from article_comments where id_article = @A and id_conversation = @B and user_id = @C";
            Execute(sql, new
            {
                A = articleCommentsFilter.Id,
                B = articleCommentsFilter.Id_conversation,
                C = articleCommentsFilter.User_Id,
            });
            return true; //TODO: need to handle delete
        }

        public bool DeleteArticleConversations(ArticleCommentsFilter articleCommentsFilter)
        {
            string sql = "delete from article_comments where id_article = @A and user_id = @B";
            Execute(sql, new
            {
                A = articleCommentsFilter.Id,
                B = articleCommentsFilter.User_Id,
            });
            return true; //TODO: need to handle delete
        }
    }
}

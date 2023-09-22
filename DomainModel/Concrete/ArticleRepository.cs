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
            Execute(sql, new { A = article.Content, B = article.Id});
        }
        
        public void UpdateArticleForm(Article article)
        {
            string sql = "update article set form_data = @A::json where id = @B";
            Execute(sql, new { A = article.Form_Data, B = article.Id});
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
    }
}

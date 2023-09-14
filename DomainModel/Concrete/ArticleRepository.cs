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
            string sql = "SELECT id, name FROM article WHERE is_deleted = false";

            if (filter.Id.HasValue)
            {
                parameters.A = filter.Id.Value;
                sql += " AND id = @A";
            }
            return Query<Article>(sql, parameters);
        }
        
        public List<Article> GetArticle(int id)
        {
            string sql = "SELECT id, name FROM article WHERE id = @A";
            return Query<Article>(sql, new { A = id });
        }

        public Article GetDataFromArticleTableLastOrDefault()
        {
            string sql = "SELECT id, name FROM article order by 1 desc limit 1";
            return QueryFirstOrDefault<Article>(sql);
        }

        public int CreateNewArticle()
        {
            var articleObject = GetDataFromArticleTableLastOrDefault();
            var newArticleNumber = articleObject == null ? 1 : articleObject.Id + 1;
            var newArticleName = "Article test " + newArticleNumber;

            //TODO: We will change the article name later
            string sql = "INSERT INTO article(name, is_deleted) VALUES(@A, false) RETURNING Id";
            return ExecuteScalar(sql, new { A = newArticleName });
        }
    }
}

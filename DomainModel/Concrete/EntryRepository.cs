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
    public class EntryRepository : BaseRepository, IEntryRepository
    {
        public List<TestObject> GetDataFromTestTable(int id)
        {
            string sql = "SELECT id, name FROM test_table WHERE id = @A";
            return Query<TestObject>(sql, new { A = id });
        }

        public List<ArticleObject> GetDataFromArticleTable()
        {
            string sql = "SELECT id, name FROM article";
            return Query<ArticleObject>(sql);
        }
        
        public List<ArticleObject> GetDataFromArticleTableById(int id)
        {
            string sql = "SELECT id, name FROM article WHERE id = @A";
            return Query<ArticleObject>(sql, new { A = id });
        }

        public ArticleObject GetDataFromArticleTableLastOrDefault()
        {
            string sql = "SELECT id, name FROM article order by 1 desc limit 1";
            return QueryFirstOrDefault<ArticleObject>(sql);
        }

        public int CreateNewArticle()
        {
            var articleObject = GetDataFromArticleTableLastOrDefault();
            var newArticleNumber = articleObject == null ? 1 : articleObject.Id + 1;
            var newArticleName = "Article test " + newArticleNumber;

            //TODO: We will change the article name later
            string sql = "INSERT INTO public.article(name) VALUES('" + newArticleName + "')";
            ExecuteScalar<ArticleObject>(sql);

            return GetDataFromArticleTableLastOrDefault().Id;
        }
    }
}

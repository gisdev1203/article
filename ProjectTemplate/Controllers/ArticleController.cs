using DomainModel.Abstract;
using DomainModel.ObjectList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.PresentationModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ProjectTemplate.Controllers
{
    public class ArticleController : Controller
    {
        private IArticleRepository articleRepository = null;
        public ArticleController(IArticleRepository articleRepository)
        {
            this.articleRepository = articleRepository;
        }

        // GET: ArticleController
        public IActionResult Articles()
        {
            ArticleDTO dto = new ArticleDTO();
            dto.Articles = articleRepository.ListArticles(new ArticleFilter { });
            return View("Articles", dto);
        }

        [HttpPost]
        public JsonResult CreateNewArticle(int article_type)
        {
            int id = articleRepository.CreateNewArticle(article_type);
           
            return Json(new { Id_article = id });
        }

        // GET: ArticleController/Edit/5
        public IActionResult EditArticle(int id)
        {
            ArticleDTO dto = new ArticleDTO();
            dto.Articles = articleRepository.ListArticles(new ArticleFilter { Id = id });
            return View("Edit", dto);
        }

        public IActionResult AutoSaveArticle(string content, string comments, int id)
        {
            ArticleTemp article_temp = articleRepository.GetArticleTemp(id);
            if (article_temp == null)
            {
                articleRepository.CreateNewArticleTemp(id);
                article_temp = articleRepository.GetArticleTemp(id);
            }
            article_temp.Content = content;

            articleRepository.UpdateArticleTemp(article_temp);
            return Json(new { success = true });
        }

        public IActionResult GetArticleTemp(int id)
        {
            ArticleTemp article_temp = articleRepository.GetArticleTemp(id);
            return Json(article_temp);
        }

        public IActionResult SaveArticle(string content, string comments, int id)
        {
            Article article = articleRepository.ListArticles(new ArticleFilter { Id = id }).FirstOrDefault();
            article.Content = content;

            articleRepository.UpdateArticle(article);
            articleRepository.DeleteArticleTemp(id);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult CreateArticleComments([FromBody] ArticleComments articleComment)
        {
            articleRepository.CreateArticleComments(articleComment);

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult GetArticleComments([FromBody] ArticleComments articleComment)
        {
            var id_article = articleComment.Id_article;
            var id_conversation = articleComment.Id_conversation;

            List<ArticleComments> article_comments = articleRepository.ListArticleComments(new ArticleCommentsFilter { Id = id_article, Id_conversation = id_conversation });
            return Json(article_comments);
        }

        [HttpPost]
        public IActionResult EditArticleComments([FromBody] ArticleComments articleComment)
        {
            articleRepository.EditArticleComments(articleComment);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ReplyArticleComments([FromBody] ArticleComments articleComment)
        {
            articleRepository.ReplyArticleComments(articleComment);

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteArticleComments([FromBody] ArticleComments articleComment)
        {
            bool result = articleRepository.DeleteArticleComments(new ArticleCommentsFilter
            {
                Id = articleComment.Id_article,
                Id_conversation = articleComment.Id_conversation,
                Id_comment = articleComment.Id_comment
            });
            return Json(new { success = result });
        }

        [HttpPost]
        public IActionResult DeleteArticleConversation([FromBody] ArticleComments articleComment)
        {
            bool result = articleRepository.DeleteArticleConversation(new ArticleCommentsFilter
            {
                Id = articleComment.Id_article,
                Id_conversation = articleComment.Id_conversation
            });
            return Json(new { success = result });
        }

        [HttpPost]
        public IActionResult DeleteArticleConversations([FromBody] ArticleComments articleComment)
        {
            var id_article = articleComment.Id_article;
            bool result = articleRepository.DeleteArticleConversations(new ArticleCommentsFilter { Id = id_article });
            return Json(new { success = result });
        }

        public IActionResult GetArticleFormDefinition(string type)
        {
            ArticleForm articleForms = articleRepository.ListArticleForm(new ArticleFormFilter { type = type }).FirstOrDefault();

            return Json(new
            {
                articleFormId = articleForms.Id,
                articleFormType = articleForms.Type,
                articleFormDefinition = articleForms.Form_Definition,
            });
        }

        public IActionResult SaveArticleForm(int id_article, string formData)
        {
            Article article = articleRepository.ListArticles(new ArticleFilter { Id = id_article }).FirstOrDefault();
            article.Form_Data = formData;

            articleRepository.UpdateArticleForm(article);
            return Json(new { success = true });
        }

        // POST: ArticleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Articles));
            }
            catch
            {
                return View();
            }
        }
    }
}

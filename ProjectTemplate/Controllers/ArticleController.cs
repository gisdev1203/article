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
        private IArticleRepository entryRepository = null;
        public ArticleController(IArticleRepository entryRepository)
        {
            this.entryRepository = entryRepository;
        }

        // GET: ArticleController
        public IActionResult Articles()
        {
            ArticleDTO dto = new ArticleDTO();
            dto.ArticleObjects = entryRepository.ListArticles(new ArticleFilter { });
            return View("Articles", dto);
        }

        [HttpPost]
        public JsonResult CreateNewArticle(int article_type)
        {
            int newArticleId = entryRepository.CreateNewArticle(article_type);
            entryRepository.CreateNewArticleTemp(newArticleId);
            return Json(new { articleId = newArticleId });
        }

        // GET: ArticleController/Edit/5
        public IActionResult EditArticle(int id)
        {
            ArticleDTO dto = new ArticleDTO();
            dto.ArticleObjects = entryRepository.ListArticles(new ArticleFilter { Id = id });
            return View("Edit", dto);
        }

        public IActionResult AutoSaveArticle(string content, string comments, int id)
        {
            ArticleDTO dto = new ArticleDTO();
            dto.ArticleTemp = entryRepository.GetArticleTemp(id);
            dto.ArticleTemp.Content = content;

            entryRepository.UpdateArticleTemp(dto.ArticleTemp);
            return Json(new { success = true });
        }

        public IActionResult SaveArticle(string content, string comments, int id)
        {
            ArticleDTO dto = new ArticleDTO();
            dto.Article = entryRepository.ListArticles(new ArticleFilter { Id = id }).FirstOrDefault();
            dto.Article.Content = content;

            entryRepository.UpdateArticle(dto.Article);
            return Json(new { success = true });
        }

        [HttpPost("/article/create_article_comments")]
        public IActionResult CreateArticleComments([FromBody] ArticleComments articleComment)
        {
            int articleId = articleComment.Article_Id;
            string articleComments = articleComment.Comments;
            DateTime comments_createdAt = articleComment.Created_At.ToUniversalTime();

            ArticleCommentsDTO dto = new ArticleCommentsDTO();
            dto.ArticleComment = new ArticleComments()
            {
                Article_Id = articleId,
                Comments = articleComments,
                Created_At = comments_createdAt.ToUniversalTime(),
            };

            entryRepository.CreateArticleComments(dto.ArticleComment);
            return Json(new { success = true });
        }

        public JsonResult GetArticleComments([FromBody] ArticleComments articleComment)
        {
            int articleId = articleComment.Article_Id;
            ArticleCommentsDTO articleCommentsDTO = new ArticleCommentsDTO();

            articleCommentsDTO.ArticleComments = entryRepository.ListArticleComments(new ArticleCommentsFilter { Id = articleId });
            return Json(articleCommentsDTO.ArticleComments);
        }

        [HttpPost("/article/Edit_article_comments")]
        public IActionResult EditArticleComments([FromBody] ArticleComments articleComment)
        {
            int articleId = articleComment.Article_Id;
            string articleComments = articleComment.Comments;
            DateTime comments_createdAt = articleComment.Created_At.ToUniversalTime();

            ArticleCommentsDTO dto = new ArticleCommentsDTO();
            dto.ArticleComment = new ArticleComments()
            {
                Article_Id = articleId,
                Comments = articleComments,
                Created_At = comments_createdAt.ToUniversalTime(),
                Id = 1 //TODO: need to get the current id
            };

            entryRepository.EditArticleComments(dto.ArticleComment);
            return Json(new { success = true });
        }

        public IActionResult GetArticleFormDefinition(string type)
        {
            ArticleFormDTO dto = new ArticleFormDTO();
            dto.ArticleForm = entryRepository.ListArticleForm(new ArticleFormFilter { type = type }).FirstOrDefault();

            return Json(new
            {
                articleFormId = dto.ArticleForm.Id,
                articleFormType = dto.ArticleForm.Type,
                articleFormDefinition = dto.ArticleForm.Form_Definition,
            });
        }

        public IActionResult SaveArticleForm(int articleId, string formData)
        {
            ArticleDTO dto = new ArticleDTO();
            dto.Article = entryRepository.ListArticles(new ArticleFilter { Id = articleId }).FirstOrDefault();
            dto.Article.Form_Data = formData;

            entryRepository.UpdateArticleForm(dto.Article);
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

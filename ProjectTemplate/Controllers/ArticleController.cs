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
             UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            ArticleDTO dto = new ArticleDTO();
            dto.Articles = articleRepository.ListArticles(new ArticleFilter { User_Id = user_id});
            dto.ArticleType = articleRepository.GetAllArticleType();
            return View("Articles", dto);
        }

        [HttpPost]
        public JsonResult CreateNewArticle(int article_type_id)
        {
             UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            int id = articleRepository.CreateNewArticle(article_type_id, user_id);           
            return Json(new { Id_article = id });
        }

        // GET: ArticleController/Edit/5
        public IActionResult EditArticle(int id)
        {
            UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            ArticleDTO dto = new ArticleDTO();
            dto.Articles = articleRepository.ListArticles(new ArticleFilter { Id = id, User_Id = user_id });
            dto.ArticleTemp = articleRepository.GetArticleTemp(id, user_id);
            return View("Edit", dto);
        }

        public IActionResult AutoSaveArticle(string content, int id)
        {
            UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            ArticleTemp article_temp = articleRepository.GetArticleTemp(id, user_id);

            if (article_temp == null)
            {
                articleRepository.CreateNewArticleTemp(id, user_id);
                article_temp = articleRepository.GetArticleTemp(id, user_id);
            }
            article_temp.Content = content;

            articleRepository.UpdateArticleTemp(article_temp);
            return Json(new { success = true });
        }

        public IActionResult GetArticleTemp(int id)
        {
             UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            ArticleTemp article_temp = articleRepository.GetArticleTemp(id, user_id);
            return Json(article_temp);
        }

        public IActionResult SaveArticle(string content, string formio_data, int id)
        {
             UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            Article article = articleRepository.ListArticles(new ArticleFilter { Id = id, User_Id = user_id }).FirstOrDefault();
            article.Content = content;
            article.Form_Data = formio_data;

            articleRepository.UpdateArticle(article);
            articleRepository.DeleteArticleTemp(id, user_id);
            return Json(new { success = true });
        }

        public IActionResult DeleteArticleTemp(int id)
        {
            UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            articleRepository.DeleteArticleTemp(id, user_id);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult CreateArticleComments([FromBody] ArticleComments articleComment)
        {
            UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            articleComment.User_Id = user_id;

            articleRepository.CreateArticleComments(articleComment);

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult GetArticleComments([FromBody] ArticleComments articleComment)
        {
             UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;

            var id_article = articleComment.Id_article;
            var id_conversation = articleComment.Id_conversation;

            List<ArticleComments> article_comments = articleRepository.ListArticleComments(new ArticleCommentsFilter { Id = id_article, Id_conversation = id_conversation, User_Id = user_id });
            return Json(article_comments);
        }

        [HttpPost]
        public JsonResult GetUserDataList()
        {
            List<UserCustom> users = articleRepository.GetUserDataList();
            return Json(users);
        }

        [HttpPost]
        public IActionResult EditArticleComments([FromBody] ArticleComments articleComment)
        {
             UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            articleComment.User_Id=user_id;
            articleRepository.EditArticleComments(articleComment);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ReplyArticleComments([FromBody] ArticleComments articleComment)
        {
            UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            articleComment.User_Id = user_id;
            articleRepository.ReplyArticleComments(articleComment);

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteArticleComments([FromBody] ArticleComments articleComment)
        {
             UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            bool result = articleRepository.DeleteArticleComments(new ArticleCommentsFilter
            {
                Id = articleComment.Id_article,
                Id_conversation = articleComment.Id_conversation,
                Id_comment = articleComment.Id_comment,
                User_Id = user_id
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

        public IActionResult GetArticleFormDefinition(int id)
        {
            ArticleForm articleForms = articleRepository.ListArticleForm(new ArticleFormFilter { Id= id }).FirstOrDefault();

            return Json(new
            {
                articleFormId = articleForms.Id,
                articleFormType = articleForms.Type,
                articleFormDefinition = articleForms.Form_Definition,
            });
        }

        public IActionResult SaveArticleForm(int id_article, string formData)
        {
             UserCustom user = articleRepository.GetUserCustom(1);
            int user_id = user.Id;
            Article article = articleRepository.ListArticles(new ArticleFilter { Id = id_article, User_Id = user_id }).FirstOrDefault();
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

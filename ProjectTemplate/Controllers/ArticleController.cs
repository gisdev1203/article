using DomainModel.Abstract;
using DomainModel.ObjectList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.PresentationModel;
using System.Linq;

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
            return Json(new { articleId = newArticleId });
        }

        // GET: ArticleController/Edit/5
        public IActionResult EditArticle(int id)
        {
            ArticleDTO dto = new ArticleDTO();
            dto.ArticleObjects = entryRepository.ListArticles(new ArticleFilter { Id = id });
            return View("Edit", dto);
        }

        public IActionResult SaveArticle(string content, string comments, int id)
        {
            ArticleDTO dto = new ArticleDTO();
            dto.Article = entryRepository.ListArticles(new ArticleFilter { Id = id }).FirstOrDefault();
            dto.Article.Content = content;

            entryRepository.UpdateArticle(dto.Article);
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

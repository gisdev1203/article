using DomainModel.Abstract;
using DomainModel.ObjectList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.PresentationModel;

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
        public JsonResult CreateNewArticle()
        {
            int newArticleId = entryRepository.CreateNewArticle();
            return Json(new { articleId = newArticleId });
        }

        // GET: ArticleController/Edit/5
        public ActionResult Edit(int id)
        {
            ArticleDTO dto = new ArticleDTO();
            dto.ArticleObjects = entryRepository.ListArticles(new ArticleFilter { Id = id });
            return View("Edit", dto);
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

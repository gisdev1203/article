using DomainModel.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.PresentationModel;

namespace ProjectTemplate.Controllers
{
    public class ArticleController : Controller
    {
        private IEntryRepository entryRepository = null;
        public ArticleController(IEntryRepository entryRepository)
        {
            this.entryRepository = entryRepository;
        }

        // GET: ArticleController
        public IActionResult Articles()
        {            
            ArticleDTO dto = new ArticleDTO();
            dto.ArticleObjects = entryRepository.GetDataFromArticleTable();
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
            dto.ArticleObjects = entryRepository.GetDataFromArticleTableById(id);
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

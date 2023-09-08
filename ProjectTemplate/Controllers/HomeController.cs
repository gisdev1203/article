using DomainModel.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectTemplate.Models;
using ProjectTemplate.PresentationModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTemplate.Controllers
{
    public class HomeController : Controller
    {
        private IEntryRepository entryRepository = null;
        public HomeController(IEntryRepository entryRepository)
        {
            this.entryRepository = entryRepository;
        }
      
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult PageTest()
        {
            TestDTO dto = new TestDTO();
            dto.TestObjects = entryRepository.GetDataFromTestTable(3);
            return View("PageTest", dto);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AreaTest.Areas.CSR.Controllers
{
    public class HomeController : Controller
    {
        // GET: CSR/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}
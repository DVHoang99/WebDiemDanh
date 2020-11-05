using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DiemDanhController : Controller
    {
        WEBATTENDANCEEntities data = new WEBATTENDANCEEntities();
        // GET: DiemDanh
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DiemDanh()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DiemDanh(Teacher t, Student s, CheckIn c)
        {
            DIEMDANH d = new DIEMDANH();

            d.MASINHVIEN = s.Id;
            d.TENSINHVIEN = s.TEN;
            d.MAGIANGVIEN = t.ID;



            return View();
        }
    }
}
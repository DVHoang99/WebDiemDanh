using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        WEBATTENDANCEEntities data = new WEBATTENDANCEEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Account a, string RePassWord)
        {
            int test1 = data.TAIKHOANs.Where(x => x.USERNAME.Equals(a.UserName)).Count();

            if (test1 == 0)
            {
                TAIKHOAN t = new TAIKHOAN();
                int test2 = data.GIANGVIENs.Where(w => w.ID.Equals(a.UserName)).Count();
                if(test2 == 1)
                {
                    t.USERNAME = a.UserName;
                    if (a.PassWord == RePassWord)
                    {
                        
                        t.PASSWORD = a.PassWord;
                        t.Name = a.Name;
                        t.ROLE1 = a.Role;
                        data.TAIKHOANs.Add(t);
                        data.SaveChanges();
                        ViewBag.a = "Đăng kí thành công !!!";
                        return View();
                        
                    }
                    else
                    {
                        ViewBag.a = "Mật khẩu và nhập lại mật khẩu không đúng !!!";
                        return View();
                    }
                }else
                {
                    ViewBag.a = "Nhập sai mã sinh viên hoặc giảng viên !!!";
                    return View();
                }
                


            }
            else
            {
                ViewBag.a = "Tên tài khoản đã tồn tại !!!";
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Account a)
        {
            var test1 = data.TAIKHOANs.Where(x => x.USERNAME.Equals(a.UserName) && x.PASSWORD.Equals(a.PassWord)).FirstOrDefault();
            if (test1 != null)
            {
                if (test1.ROLE1 == 1)
                {
                    Session["Login"] = test1;
                    return RedirectToAction("ManageAccount", "Manage");
                }
                else
                {
                    Session["Login"] = test1;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.a = "sai ten tai khoan mat khau";
            }
            return View();
        }
    }
}
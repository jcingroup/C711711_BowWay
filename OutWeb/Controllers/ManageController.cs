using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class ManageController : Controller
    {
        public ManageController()
        {
            ViewBag.IsFirstPage = false;
        }

        // GET: Manage
        public ActionResult Index()        {
            
            return View("Login");
        }

        // 登入頁
        public ActionResult Login()
        {
            return View();
        }

        // 最新消息
        public ActionResult NewsList()
        {
            return View();
        }
        public ActionResult NewsData()
        {
            return View();
        }

        // 活動剪影
        // 分類
        public ActionResult ActivityKind()
        {
            return View();
        }
        public ActionResult ActivityKindData()
        {
            return View();
        }
        // 相片
        public ActionResult ActivityList()
        {
            return View();
        }
        public ActionResult ActivityData()
        {
            return View();
        }

        // 修改密碼
        public ActionResult ChangePW()
        {
            return View();
        }
    }
}
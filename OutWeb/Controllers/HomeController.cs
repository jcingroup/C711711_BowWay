using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            ViewBag.IsFirstPage = false;
        }

        // all 靜態
        public ActionResult Index()
        {
            ViewBag.IsFirstPage = true;
            return View();            
        }

        // 公司簡介
        public ActionResult AboutUs()
        {
            return View();
        }

        // 客服中心
        public ActionResult ContactUs()
        {
            return View();
        }

        // 服務項目
        public ActionResult Service()
        {
            return View();
        }
        // 安全防護
        public ActionResult Security()
        {
            return View();
        }
        // 事務管理
        public ActionResult Affair()
        {
            return View();
        }
        // 設備維護
        public ActionResult Equipment()
        {
            return View();
        }
        // 環保清潔
        public ActionResult Clean()
        {
            return View();
        }
        // 停車管理
        public ActionResult Park()
        {
            return View();
        }

        // 菁英招募
        public ActionResult JoinUs()
        {
            return View();
        }
    }
}
using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class ActivityController : Controller
    {
        public ActivityController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return View("Album");
        }

        // 套程式-活動剪影
        // 相簿
        public ActionResult Album()
        {
            return View();
        }

        // 圖片列表
        public ActionResult Photo()
        {
            return View();
        }
    }
}
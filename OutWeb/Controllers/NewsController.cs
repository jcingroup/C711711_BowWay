using System.Web.Mvc;

namespace OutWeb.Controllers
{
    public class NewsController : Controller
    {
        public NewsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return View("List");
        }

        // 套程式-最新消息
        // 列表
        public ActionResult List()
        {
            return View();
        }

        // 內容
        public ActionResult Content()
        {
            return View();
        }
    }
}
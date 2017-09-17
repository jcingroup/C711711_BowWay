using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OutWeb.Service;
using Lib.Service;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
/*Json.NET相關的命名空間*/
using Newtonsoft.Json;

namespace OutWeb.Controllers
{
    public class NewsController : Controller
    {
        DBService DB = new DBService();

        public NewsController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // 套程式-最新消息
        // 列表
        public ActionResult List()
        {
            //定義變數
            DataTable d_news;


            //抓取消息資料
            d_news = DB.News_List("", "n_date desc", "Y", "", "", "", "");

            //設定傳值
            ViewData["d_news"] = d_news;

            return View();
        }

        // 內容
        public ActionResult NewsData(string n_id = "")
        {
            //定義變數
            DataTable d_news;

            d_news = DB.News_List(n_id, "", "Y", "", "", "", "");

            if (d_news.Rows.Count > 0)
            {
                ViewData["d_news"] = d_news;
                return View("Content");
            }
            else
            {
                return RedirectToAction("index");
            }
        }
    }
}
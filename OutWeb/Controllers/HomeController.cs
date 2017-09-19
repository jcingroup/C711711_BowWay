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
    public class HomeController : Controller
    {
        DBService DB = new DBService();

        public HomeController()
        {
            ViewBag.IsFirstPage = false;
        }

        // all 靜態
        public ActionResult Index()
        {
            ViewBag.IsFirstPage = true;
            //定義變數
            DataTable d_news;


            //抓取消息資料
            d_news = DB.News_List("", "n_date desc", "Y", "", "", "", "Y");

            //設定傳值
            ViewData["d_news"] = d_news;
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
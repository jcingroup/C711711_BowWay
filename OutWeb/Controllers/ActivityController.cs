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
    public class ActivityController : Controller
    {
        DBService DB = new DBService();

        public ActivityController()
        {
            ViewBag.IsFirstPage = false;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Album");
        }

        // 套程式-活動剪影
        // 相簿
        public ActionResult Album()
        {
            //定義變數
            DataTable d_album;

            //抓取消息資料
            d_album = DB.Activity_List("", "sort desc", "Y", "");
            
            //設定傳值
            ViewData["d_album"] = d_album;

            return View();
        }

        // 圖片列表
        public ActionResult Photo(string act_id = "")
        {
            //定義變數
            DataTable d_album;
            DataTable d_album_img;

            d_album = DB.Activity_List(act_id);
            d_album_img = DB.Img_List(act_id, "", "Activity");

            if (d_album.Rows.Count > 0)
            {
                ViewData["d_album"] = d_album;
                ViewData["d_album_img"] = d_album_img;
                return View();
            }
            else
            {
                return RedirectToAction("index");
            }
        }
    }
}
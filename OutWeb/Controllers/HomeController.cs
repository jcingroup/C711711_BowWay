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

        //發送MAIL
        [HttpPost]
        public ActionResult ToMail(string txt_name, string txt_email, string txt_tel,string txt_fax, string txt_question, string ValidCode)
        {
            string cmsg = "";
            string n_date = DateTime.Now.ToString("yyyy-MM-dd");
            string n_time = DateTime.Now.ToString("yyyy-MM-dd HH:mi:ss");
            string txt_body = "";
            string To_Mail = "yyuuix@gmail.com";
            //string To_Mail = "gary661116@gmail.com";
            string To_Mail_Name = "";
            string cHtmlBody = "";
            string cSubject = "";
            DataTable dt = new DataTable();
            try
            {
                Service.Service sc = new OutWeb.Service.Service();
                if (string.IsNullOrWhiteSpace(txt_name))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入姓名";
                }

                if (string.IsNullOrWhiteSpace(txt_email))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入e-mail";
                }

                if (string.IsNullOrWhiteSpace(txt_tel))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入聯絡電話";
                }
                if (string.IsNullOrWhiteSpace(txt_fax))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入傳真電話";
                }
                if (string.IsNullOrWhiteSpace(txt_question))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入問題";
                }
                if (cmsg.Trim().Length == 0)
                {
                    //比對驗證碼
                    if (ValidCode != Session["ValidateCode"].ToString())
                    {
                        if (cmsg.Trim().Length > 0)
                        {
                            cmsg = cmsg + "\n";
                        }
                        cmsg = cmsg + "驗證碼不正確";
                    }
                    else
                    {
                        //呼叫mail
                        cSubject = "網站系統 - 線上諮詢 [" + n_date + "]";
                        cHtmlBody = "";
                        cHtmlBody = cHtmlBody + "您好！" + "<br>";
                        cHtmlBody = cHtmlBody + "這是由網站系統所寄發的一封 - 線上諮詢！" + "<br>";
                        cHtmlBody = cHtmlBody + "以下為諮詢內容：" + "<br>";
                        cHtmlBody = cHtmlBody + "==========================================================" + "<br>";
                        cHtmlBody = cHtmlBody + "姓　　名：" + txt_name + "<br>";
                        cHtmlBody = cHtmlBody + "諮詢內容：" + "<br>";
                        cHtmlBody = cHtmlBody + txt_question + "<br>";
                        cHtmlBody = cHtmlBody + "E-Mail  ：" + txt_email + "<br>";
                        cHtmlBody = cHtmlBody + "聯絡電話：" + txt_tel + "<br>";
                        cHtmlBody = cHtmlBody + "傳真電話：" + txt_fax + "<br>";
                        cHtmlBody = cHtmlBody + "請立即處理此諮詢資料！" + "<br>";
                        cHtmlBody = cHtmlBody + "==========================================================" + "<br>";
                        dt = sc.Mail("WebAdm@BowWay.com", "Web_Adm", To_Mail, cSubject, cHtmlBody);
                    }
                }


                if (cmsg.Trim().Length > 0)
                {
                    TempData["message"] = cmsg;
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        if(dt.Rows[0]["status"].ToString() == "Y")
                        {
                            TempData["message"] = "已通知相關人員，請耐心等待回覆~~謝謝";
                        }
                        else
                        {
                            TempData["message"] = "通知失敗!!";
                        }
                    }
                }

                return View("ContactUs");

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
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

        //設置將生成的驗證碼存入Session，並輸出驗證碼圖片
        [AllowAnonymous]
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }
    }
}
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
    public class ManageController : Controller
    {
        DBService DB = new DBService();

        public ManageController()
        {
            ViewBag.IsFirstPage = false;
        }

        // GET: Manage
        // 後台首頁-導向Login
        public ActionResult Index()
        {
            if (Convert.ToString(Session["IsLogined"]) == "Y")
            {
                return RedirectToAction("NewsList");
            }
            else
            {
                return View("Login");
            }
        }

        // 登入頁
        public ActionResult Login()
        {
            return View();
        }

        //登入檢查
        [HttpPost]
        public ActionResult Login_Chk(string account, string pwd, string ValidCode)
        {
            DataTable user_info;
            string cmsg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(account))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入帳號";
                }

                if (string.IsNullOrWhiteSpace(pwd))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入密碼";
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
                        //抓取user資料
                        user_info = DB.User_Info(account);
                        //驗證使用者有無資料
                        if (user_info.Rows.Count == 0)
                        {
                            if (cmsg.Trim().Length > 0)
                            {
                                cmsg = cmsg + "\n";
                            }
                            cmsg = cmsg + "無此帳號，請再確認。";
                        }
                        else
                        {
                            if (user_info.Rows[0]["status"].ToString() == "N")
                            {
                                if (cmsg.Trim().Length > 0)
                                {
                                    cmsg = cmsg + "\n";
                                }
                                cmsg = cmsg + "此帳號停用，請再確認。";
                            }
                            else
                            {
                                if (pwd == user_info.Rows[0]["pwd"].ToString())
                                {
                                    //輸入值
                                    Session["IsLogined"] = "Y";
                                    Session["Account"] = account;
                                    Session["Account_Name"] = user_info.Rows[0]["account_name"].ToString();
                                    Session["Rank"] = user_info.Rows[0]["rank"].ToString();
                                }
                                else
                                {
                                    if (cmsg.Trim().Length > 0)
                                    {
                                        cmsg = cmsg + "\n";
                                    }
                                    cmsg = cmsg + "密碼錯誤，請重新輸入。";
                                }
                            }
                        }
                    }
                }

                if (cmsg.Trim().Length > 0)
                {
                    TempData["message"] = cmsg;
                    return View("Login");
                }
                else
                {
                    return RedirectToAction("NewsList");
                }

            }
            catch
            {
                return View("Login");
            }
        }

        //後台登出
        public ActionResult Logout()
        {
            //清除 Session();
            Session.Remove("IsLogined");
            Session.Remove("Account");
            Session.Remove("Account_Name");
            Session.Remove("Rank");

            return Redirect(Url.Content("~/Manage"));
        }

        #region 最新消息

        #region 消息陳列 NewList
        public ActionResult NewsList(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_start_date = "", string txt_end_date = "", string txt_show = "", string txt_index = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
           
            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取消息資料
            dt = DB.News_List("", c_sort, txt_show, txt_title_query, txt_start_date, txt_end_date,txt_index);
           
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_start_date"] = txt_start_date;
            ViewData["txt_end_date"] = txt_end_date;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;

            return View();
        }
        #endregion

        #region 最新消息新增 News_Add
        public ActionResult News_Add()
        {
            ViewData["action_sty"] = "add";

            return View("NewsData");
        }
        #endregion

        #region 最新消息修改 News_Edit
        public ActionResult News_Edit(string n_id = "")
        {
            DataTable d_news = DB.News_List(n_id);
            ViewData["d_news"] = d_news;
            ViewData["action_sty"] = "edit";

            return View("NewsData");
        }
        #endregion

        #region 最新消息刪除 News_Del
        public ActionResult News_Del(string n_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            DB.News_Del(n_id);
            return RedirectToAction("NewsList");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult News_Save(string action_sty, string n_id, string n_title, string n_date, string n_desc, string show, string hot, string sort)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    DB.News_Insert(n_title, n_date, n_desc, show, hot, sort);
                    break;
                case "edit":
                    DB.News_Update(n_id, n_title, n_date, n_desc, show, hot, sort);
                    break;
            }

            return RedirectToAction("NewsList");
        }

        #endregion

        #endregion

        // 活動剪影        
        public ActionResult ActivityList(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_show = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取消息資料
            dt = DB.Activity_List("", c_sort, txt_show, txt_title_query);

            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            return View();
        }

        public ActionResult Activity_Add()
        {
            DataTable d_activity = DB.Activity_List("","");
            DataTable d_activity_img = DB.Img_List("", "","Activity");

            ViewData["d_activity"] = d_activity;
            ViewData["d_activity_img"] = d_activity_img;
            ViewData["action_sty"] = "add";

            return View("ActivityData");
        }

        public ActionResult Activity_Edit(string act_id = "")
        {
            DataTable d_activity = DB.Activity_List(act_id);
            DataTable d_activity_img = DB.Img_List(act_id, "", "Activity");

            ViewData["d_activity"] = d_activity;
            ViewData["d_activity_img"] = d_activity_img;
            ViewData["action_sty"] = "edit";

            return View("ActivityData");
        }

        public ActionResult Activity_Del(string act_id = "")
        {
            DB.Activity_Del(act_id);
            return RedirectToAction("ActivityList");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Activity_Save(string action_sty, string act_id, string act_name, string act_sort, string show, string img_no,string[] img_id, string[] img_desc, string is_index, string img_count)
        {
            //string str_act_id = "";
            DataTable dt = new DataTable();
            string c_msg = "";
            string err_msg = "";
            int i_count = 0;
            string str_img_desc = "";
            string str_img_id = "";
            string str_index = "";
            string act_desc = "";
            i_count = Convert.ToInt32(img_count);

            act_desc = img_no;

            //Activity
            switch (action_sty)
            {
                case "add":
                    dt = DB.Activity_Insert(act_name, act_desc, show, img_no,act_sort);
                    break;
                case "edit":
                    dt = DB.Activity_Update(act_id, act_name, act_desc, show,act_sort);
                    break;
            }

            if(dt != null)
            {
                if(dt.Rows.Count > 0)
                {
                    act_id = dt.Rows[0]["act_id"].ToString();
                    c_msg = dt.Rows[0]["c_msg"].ToString();
                    err_msg = dt.Rows[0]["err_msg"].ToString();
                }
            }
            //Activity_Img
            str_img_id = "";
            str_img_desc = "";
            str_index = "";

            for(int i=0; i<i_count; i++)
            {
                str_img_id = img_id[i];
                str_img_desc = img_desc[i];
                if(is_index == str_img_id)
                {
                    str_index = "Y";
                }
                else
                {
                    str_index = "N";
                }

                DB.Img_Update(str_img_id, "",str_img_desc,str_index, "Activity");
            }

            //return RedirectToAction("ActivityList");
            return Redirect("~/Manage/Activity_Edit?act_id=" + act_id);
            //DataTable d_activity = DB.Activity_List(act_id);
            //DataTable d_activity_img = DB.Img_List(act_id, "", "Activity");

            //ViewData["d_activity"] = d_activity;
            //ViewData["d_activity_img"] = d_activity_img;
            //ViewData["action_sty"] = "edit";

            //return View("ActivityData");
        }
        // 修改密碼
        public ActionResult ChangePW()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePW(string now_pwd, string new_pwd, string chk_new_pwd)
        {
            string Account = Convert.ToString(Session["Account"]);
            string cmsg = "";
            DataTable user_info;
            //抓取資料
            user_info = DB.User_Info(Account);
            //檢查登入密碼是否正確
            if (now_pwd == user_info.Rows[0]["pwd"].ToString())
            {
                DB.User_Update(Account, new_pwd);
            }
            else
            {
                if (cmsg.Trim().Length > 0)
                {
                    cmsg = cmsg + "\n";
                }
                cmsg = cmsg + "密碼錯誤，請重新輸入。";
            }

            if (cmsg.Trim().Length == 0)
            {
                cmsg = "Y";
            }

            TempData["message"] = cmsg;
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

        public ActionResult Upload(string img_no, string img_sta)
        {
            DataTable img_file;
            DataTable chk_file;

            HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            string imgPath = "";
            string filename = "";
            string file_name = "";
            string file_path = "../Images/";
            string str_return = "";
            string[] files;
            string chk_sty = "";
            string pre_filename = "";
            int files_count = 0;

            if (hfc.Count > 0)
            {
                file_name = hfc[0].FileName;
                files = file_name.Split('\\');
                files_count = files.Count();
                filename = files[files_count - 1];

                imgPath = file_path + img_no + "_" + img_sta + "_" + filename;
                string PhysicalPath = Server.MapPath(imgPath);
                hfc[0].SaveAs(PhysicalPath);
            }

            chk_file = DB.Img_List(img_no, img_sta);

            chk_sty = "add";
            if (img_sta == "S")
            {
                if (chk_file.Rows.Count > 0)
                {
                    pre_filename = file_path + chk_file.Rows[0]["img_file"].ToString();

                    chk_sty = "update";
                }
            }

            switch (chk_sty)
            {
                case "add": //加入到資料庫
                    DB.Img_Insert(img_no, img_no + "_" + img_sta + "_" + filename, img_sta);
                    break;
                case "update":
                    DB.Img_Update(chk_file.Rows[0]["_SEQ_ID"].ToString(), img_no + "_" + img_sta + "_" + filename);

                    //刪除原本檔案
                    string Pre_Path = Server.MapPath(pre_filename);

                    // Delete a file by using File class static method...
                    if (System.IO.File.Exists(Pre_Path))
                    {
                        // Use a try block to catch IOExceptions, to
                        // handle the case of the file already being
                        // opened by another process.
                        try
                        {
                            System.IO.File.Delete(Pre_Path);
                        }
                        catch (System.IO.IOException e)
                        {
                            str_return = str_return + e.Message;
                        }
                    }
                    break;
            }

            //抓取資料
            img_file = DB.Img_List(img_no, img_sta);

            str_return = JsonConvert.SerializeObject(img_file, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);


        }

        public ActionResult Img_Del(string img_id, string img_sta, string img_no)
        {
            string str_return = "";
            DataTable img_file;
            DataTable chk_file;
            string filename = "";
            string file_path = "../Images/";
            string imgPath = "";

            chk_file = DB.Img_List(img_no, img_sta);
            filename = "";
            if (chk_file.Rows.Count > 0)
            {
                for (int i = 0; i < chk_file.Rows.Count; i++)
                {
                    if (img_id == chk_file.Rows[i]["img_id"].ToString())
                    {
                        filename = chk_file.Rows[i]["img_file"].ToString();
                        break;
                    }
                }
            }

            imgPath = file_path + filename;

            string PhysicalPath = Server.MapPath(imgPath);

            // Delete a file by using File class static method...
            if (System.IO.File.Exists(PhysicalPath))
            {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try
                {
                    System.IO.File.Delete(PhysicalPath);
                }
                catch (System.IO.IOException e)
                {
                    str_return = str_return + e.Message;
                }
            }

            DB.Img_Delete(img_id);
            //抓取資料
            img_file = DB.Img_List(img_no, img_sta);
            str_return = JsonConvert.SerializeObject(img_file, Newtonsoft.Json.Formatting.Indented);
            return Content(str_return);
        }


        /// <summary>
        /// ckeditor上傳圖片
        /// </summary>
        /// <param name="upload">預設參數叫upload</param>
        /// <param name="CKEditorFuncNum"></param>
        /// <param name="CKEditor"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPicture(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string result = "";
            if (upload != null && upload.ContentLength > 0)
            {
                //儲存圖片至Server
                upload.SaveAs(Server.MapPath("~/Images/" + upload.FileName));


                var imageUrl = Url.Content("~/Images/" + upload.FileName);

                var vMessage = string.Empty;

                result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";

            }

            return Content(result);
        }
    }
}
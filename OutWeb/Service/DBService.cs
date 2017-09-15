using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Lib.Service
{
    public class DBService
    {
        string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string csql = "";

        DataSet ds = new DataSet();

        #region 消息資料抓取 News_List
        public DataTable News_List(string news_id = "", string sort = "", string status = "", string title_query = "", string start_date = "", string end_date = "", string is_index = "")
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            string[] Array_news_id;
            string[] Array_title_query;

            Array_news_id = news_id.Split(',');
            Array_title_query = title_query.Split(',');

            csql = "select "
                 + "  a1.* "
                 + "from "
                 + "("
                 + "select distinct "
                 + "  a1.n_id, a1.n_title, convert(nvarchar(10),a1.n_date,23) as n_date, a1.n_url, a1.n_desc "
                 + ", a1.is_index, a1.sort, a1.status "
                 + "from "
                 + "   news a1 "
                 + "where "
                 + "  a1.status <> 'D' ";

            if (status.Trim().Length > 0)
            {
                csql = csql + " and a1.status = @status ";
            }

            if (news_id.Trim().Length > 0)
            {
                csql = csql + " and a1.n_id in (";
                for (int i = 0; i < Array_news_id.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + ",";
                    }
                    csql = csql + "@str_news_id" + i.ToString();
                }
                csql = csql + ") ";
            }

            if (title_query.Trim().Length > 0)
            {
                csql = csql + " and (";
                for (int i = 0; i < Array_title_query.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + " or ";
                    }
                    csql = csql + " a1.n_title like @str_title_query" + i.ToString() + " ";
                }
                csql = csql + ") ";
            }

            if (start_date.Trim().Length > 0)
            {

                csql = csql + "and a1.n_date >= @start_date ";
            }

            if (end_date.Trim().Length > 0)
            {

                csql = csql + "and a1.n_date <= @end_date ";
            }

            if (is_index.Trim().Length > 0)
            {
                csql = csql + "and a1.is_index = @is_index ";
            }

            csql = csql + ")a1 ";

            if (sort.Trim().Length > 0)
            {
                csql = csql + " order by " + sort + " ";
            }
            else
            {
                csql = csql + " order by a1.sort desc ";
            }

            cmd.CommandText = csql;

            //---------------------------------------------------------------//
            cmd.Parameters.Clear();
            if (status.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@status", status);
            }

            if (start_date.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@start_date", start_date);
            }

            if (end_date.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@end_date", end_date);
            }

            if (news_id.Trim().Length > 0)
            {
                for (int i = 0; i < Array_news_id.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_news_id" + i.ToString(), Array_news_id[i]);
                }
            }

            if (title_query.Trim().Length > 0)
            {
                for (int i = 0; i < Array_title_query.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_title_query" + i.ToString(), "%" + Array_title_query[i] + "%");
                }
            }

            if (is_index.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@is_index", is_index);
            }
            //--------------------------------------------------------------//

            if (ds.Tables["news"] != null)
            {
                ds.Tables["news"].Clear();
            }

            SqlDataAdapter news_ada = new SqlDataAdapter();
            news_ada.SelectCommand = cmd;
            news_ada.Fill(ds, "news");
            news_ada = null;

            cmd = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["news"];
        }
        #endregion

        #region 消息資料新增 News_Insert
        public string News_Insert(string n_title = "", string n_date = "", string n_desc = "", string is_show = "", string is_index = "", string sort = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"insert into News(n_title,n_date,n_desc,is_index,sort,status) "
                     + "values(@n_title,@n_date,@n_desc,@is_index,@sort,@is_show)";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_title", n_title);
                cmd.Parameters.AddWithValue("@n_date", n_date);
                cmd.Parameters.AddWithValue("@n_desc", n_desc);
                cmd.Parameters.AddWithValue("@is_index", is_index);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_show", is_show);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 消息資料更新 News_Update
        //更新內容
        public string News_Update(string n_id = "", string n_title = "", string n_date = "", string n_desc = "", string is_show = "", string is_index = "", string sort = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  news "
                     + "set "
                     + "  n_title = @n_title "
                     + ", n_date = @n_date "
                     + ", n_desc = @n_desc "
                     + ", status = @is_show "
                     + ", is_index = @is_index "
                     + ", sort = @sort "
                     + ", _UPD_ID = 'System' "
                     + ", _UPD_DT = getdate() "
                     + "where "
                     + "  n_id = @n_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_id", n_id);
                cmd.Parameters.AddWithValue("@n_title", n_title);
                cmd.Parameters.AddWithValue("@n_date", n_date);
                cmd.Parameters.AddWithValue("@n_desc", n_desc);
                cmd.Parameters.AddWithValue("@is_index", is_index);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@is_show", is_show);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;

        }
        #endregion

        #region 消息資料刪除 News_Del
        public string News_Del(string n_id = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"delete from "
                     + "  news "
                     + "where "
                     + "  n_id = @n_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_id", n_id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 首頁影片資料 Video_List
        public DataTable Video_List()
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            csql = "select "
                 + "  a1.* "
                 + "from "
                 + "  Advertisement a1 "
                 + "where "
                 + "  ad_title = 'img' ";

            cmd.CommandText = csql;

            if (ds.Tables["video"] != null)
            {
                ds.Tables["video"].Clear();
            }

            SqlDataAdapter video_ada = new SqlDataAdapter();
            video_ada.SelectCommand = cmd;
            video_ada.Fill(ds, "video");
            video_ada = null;

            cmd = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["video"];
        }
        #endregion

        #region 首頁影片資料更新 Video_Update
        public string Video_Update(string mv)
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "update "
                     + "  Advertisement "
                     + "set "
                     + "  ad_mv = @mv "
                     + "where "
                     + "  ad_id = 1";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@mv", mv);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;

        }
        #endregion

        #region 關於我們 Com_List
        //Com_List("AboutUs", lang)
        public DataTable Com_List(string category = "", string lang = "")
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            csql = "select "
                 + "  a1.* "
                 + "from "
                 + "  Company_Info a1 "
                 + "where "
                 + "    a1.category = @category "
                 + "and a1.lang = @lang";

            cmd.CommandText = csql;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@category", category);
            cmd.Parameters.AddWithValue("@lang", lang);

            if (ds.Tables["com_info"] != null)
            {
                ds.Tables["com_info"].Clear();
            }

            SqlDataAdapter com_info_ada = new SqlDataAdapter();
            com_info_ada.SelectCommand = cmd;
            com_info_ada.Fill(ds, "com_info");
            com_info_ada = null;

            cmd = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["com_info"];
        }
        #endregion

        #region 關於我們更新 Com_Update
        //DB.Com_Update("AboutUs", lang, com_desc);
        public string Com_Update(string category = "", string lang = "", string com_desc = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                //檢查是否有資料
                DataTable d_com_info = Com_List(category, lang);
                if (d_com_info.Rows.Count > 0)
                {
                    csql = "update "
                         + "  Company_Info "
                         + "set "
                         + "  com_desc = @com_desc "
                         + "where "
                         + "    lang = @lang "
                         + "and category = @category";
                }
                else
                {
                    csql = "insert into "
                         + "Company_Info(com_desc, lang, category) "
                         + "Values(@com_desc,@lang,@category) ";
                }

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@com_desc", com_desc);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.Parameters.AddWithValue("@category", category);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;

        }
        #endregion

        #region 圖片新增 Img_Insert
        public string Img_Insert(string img_no = "", string img_file = "", string img_sty = "",string c_dbf = "Activity")
        {
            string c_msg = "";
            string dbf_name = "";
            dbf_name = c_dbf + "_Img";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"insert into " + dbf_name + "(img_no, img_file, img_sty) "
                     + "values(@img_no ,@img_file ,@img_sty)";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_no", img_no);
                cmd.Parameters.AddWithValue("@img_file", img_file);
                cmd.Parameters.AddWithValue("@img_sty", img_sty);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 圖片刪除 Img_Delete
        public string Img_Delete(string img_id = "",string c_dbf = "Activity")
        {
            string c_msg = "";
            string dbf_name = c_dbf + "_Img";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"delete from " + dbf_name + " where img_id = @img_id ";

                cmd.CommandText = csql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_id", img_id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 圖片更新 Img_Update
        public string Img_Update(string img_id = "", string img_file = "",string img_desc = "",string is_index="",string c_dbf="Activity")
        {
            string c_msg = "";
            string dbf_name = "";
            string c_update = "";

            dbf_name = c_dbf + "_Img";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;


            try
            {
                if(img_file.Trim().Length > 0)
                {
                    if(c_update.Trim().Length > 0)
                    {
                        c_update += ",";
                    }
                    c_update += " img_file = @img_file ";
                }

                if (img_desc.Trim().Length > 0)
                {
                    if (c_update.Trim().Length > 0)
                    {
                        c_update += ",";
                    }
                    c_update += " img_desc = @img_desc ";
                }

                if(is_index.Trim().Length > 0)
                {
                    if (c_update.Trim().Length > 0)
                    {
                        c_update += ",";
                    }
                    c_update += " is_index = @is_index ";
                }

                csql = @"update "
                     + "  " + dbf_name + " "
                     + "set "
                     + c_update + " "
                     + "where "
                     + "  img_id = @img_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                
                cmd.Parameters.AddWithValue("@img_id", img_id);
                if (img_file.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@img_file", img_file);
                }

                if (img_desc.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@img_desc", img_desc);
                }

                if (is_index.Trim().Length > 0)
                {
                    cmd.Parameters.AddWithValue("@is_index", is_index);
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 圖片陳列 Img_List
        public DataTable Img_List(string img_no = "", string img_sty = "", string c_dbf = "Activity")
        {
            DataSet dt = new DataSet();
            DataTable d_t;
            SqlConnection conn = new SqlConnection(conn_str);
            string dbf_name = "";
            string[] cimg_no;
            string str_img_no = "";

            dbf_name = c_dbf + "_Img";
            cimg_no = img_no.Split(',');

            for (int i = 0; i < cimg_no.Length; i++)
            {
                if (i > 0)
                {
                    str_img_no = str_img_no + ",";
                }
                str_img_no = str_img_no + "'" + cimg_no[i] + "'";
            }

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            csql = "select * from " + dbf_name + " where status = 'Y' and img_no in (";
            for (int i = 0; i < cimg_no.Length; i++)
            {
                if (i > 0)
                {
                    csql = csql + ",";
                }
                csql = csql + "@str_img_no" + i.ToString() + " ";
            }
            csql = csql + ") ";
            if(img_sty.Trim().Length > 0)
            {
                csql = csql + "and img_sty= @img_sty ";
            }
            csql = csql + "order by ";
            csql = csql + "  img_id ";

            cmd.CommandText = csql;

            cmd.Parameters.Clear();
            if(img_sty.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@img_sty", img_sty);
            }

            for (int i = 0; i < cimg_no.Length; i++)
            {
                cmd.Parameters.AddWithValue("@str_img_no" + i.ToString(), cimg_no[i]);
            }


            if (dt.Tables["img"] != null)
            {
                dt.Tables["img"].Clear();
            }

            SqlDataAdapter img_ada = new SqlDataAdapter();
            img_ada.SelectCommand = cmd;
            img_ada.Fill(dt, "img");
            img_ada = null;

            d_t = dt.Tables["img"];

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;
            dt = null;

            return d_t;
        }
        #endregion

        #region 使用者資訊 User_Info
        public DataTable User_Info(string account = "")
        {
            DataSet dt = new DataSet();
            DataTable d_t;
            SqlConnection conn = new SqlConnection(conn_str);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            csql = @"select "
                 + "  * "
                 + "from "
                 + "  member "
                 + "where "
                 + "   status <> 'D' "
                 + "and account = @account "
                 + "order by "
                 + "  _SEQ_ID ";



            if (dt.Tables["user_info"] != null)
            {
                dt.Tables["user_info"].Clear();
            }

            SqlDataAdapter user_info_ada = new SqlDataAdapter();
            SqlCommand CMD = new SqlCommand(csql, conn);

            ////定義parameter型別
            CMD.Parameters.Clear();
            //CMD.Parameters.AddWithValue(@account, account);
            CMD.Parameters.Add("@account", SqlDbType.NVarChar, 15).Value = account; //(參數,宣考型態,長度)

            user_info_ada.SelectCommand = CMD;
            user_info_ada.Fill(dt, "user_info");
            user_info_ada = null;

            d_t = dt.Tables["user_info"];

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;
            dt = null;

            return d_t;
        }
        #endregion

        #region 活動花絮陳列 Activity_List
        public DataTable Activity_List(string act_id = "", string sort = "", string status = "", string activity_query = "")
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            string[] Array_activity_id;
            string[] Array_activity_query;


            Array_activity_id = act_id.Split(',');
            Array_activity_query = activity_query.Split(',');

            csql = "select "
                 + " a.*,b.img_file,isnull(c.img_count,0) as img_count "
                 + "from "
                 + "  activity_bt a "
                 //-----------------------------------------------------------//
                 + "left join Activity_Img b on Convert(nvarchar(50),a.act_id) = b.img_no and b.is_index = 'Y' "
                 + "left join ("
                 + " select "
                 + "    img_no, count(img_id) as img_count "
                 + " from "
                 + "    Activity_Img "
                 + " group by "
                 + "    img_no "
                 + ")c on Convert(nvarchar(50),a.act_id) = c.img_no "
                 + "where "
                 + "  a.status <> 'D' ";

            if (status.Trim().Length > 0)
            {
                csql = csql + " and a.status = @status ";
            }

            if (act_id.Trim().Length > 0)
            {
                csql = csql + " and a.act_id in (";
                for (int i = 0; i < Array_activity_id.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + ",";
                    }
                    csql = csql + "@str_act_id" + i.ToString();
                }
                csql = csql + ") ";
            }
            
            if (activity_query.Trim().Length > 0)
            {
                csql = csql + " and (";
                for (int i = 0; i < Array_activity_query.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + " or ";
                    }
                    csql = csql + " a.act_name like @str_activity_query" + i.ToString() + " ";
                }
                csql = csql + ") ";
            }

            if (sort.Trim().Length > 0)
            {
                csql = csql + " order by " + sort + " ";
            }

            cmd.CommandText = csql;

            //---------------------------------------------------------------//
            cmd.Parameters.Clear();
            if (status.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@status", status);
            }

            if (act_id.Trim().Length > 0)
            {
                for (int i = 0; i < Array_activity_id.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_act_id" + i.ToString(), Array_activity_id[i]);
                }
            }

            
            if (activity_query.Trim().Length > 0)
            {
                for (int i = 0; i < Array_activity_query.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_activity_query" + i.ToString(), "%" + Array_activity_query[i] + "%");
                }
            }
            //--------------------------------------------------------------//

            if (ds.Tables["activity"] != null)
            {
                ds.Tables["activity"].Clear();
            }

            SqlDataAdapter scenic_ada = new SqlDataAdapter();
            scenic_ada.SelectCommand = cmd;
            scenic_ada.Fill(ds, "activity");
            scenic_ada = null;

            cmd = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["activity"];
        }
        #endregion

        #region 活動花絮新增 Activity_Insert
        public DataTable Activity_Insert(string act_name = "", string act_desc = "", string show = "", string img_no = "",string sort = "")
        {
            string err_msg = "";
            string act_id = "";
            string c_msg = "";
            string c_status = "";

            DataTable dt = new DataTable();
            DataRow dw;

            dt.Columns.Add("act_id", System.Type.GetType("System.String"));
            dt.Columns.Add("c_msg", System.Type.GetType("System.String"));
            dt.Columns.Add("err_msg", System.Type.GetType("System.String"));

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                if(show == "on")
                {
                    c_status = "Y";
                }
                else
                {
                    c_status = "N";
                }

                csql = @"insert into activity_bt(act_name,act_desc,status,sort) "
                     + "values(@act_name,@act_desc,@show,@sort)";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@act_name", act_name);
                cmd.Parameters.AddWithValue("@act_desc", act_desc);
                cmd.Parameters.AddWithValue("@show", c_status);
                cmd.Parameters.AddWithValue("@sort", sort);

                cmd.ExecuteNonQuery();

                //抓取其編號
                csql = @"select distinct "
                     + "  act_id "
                     + "from "
                     + "   Activity_bt "
                     + "where "
                     + "    act_name = @act_name "
                     + "and act_desc = @act_desc "
                     + "and status = @show "
                     + "and sort = @sort";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@act_name", act_name);
                cmd.Parameters.AddWithValue("@act_desc", act_desc);
                cmd.Parameters.AddWithValue("@show", c_status);
                cmd.Parameters.AddWithValue("@sort", sort);

                if (ds.Tables["chk_scenic"] != null)
                {
                    ds.Tables["chk_scenic"].Clear();
                }

                SqlDataAdapter scenic_chk_ada = new SqlDataAdapter();
                scenic_chk_ada.SelectCommand = cmd;
                scenic_chk_ada.Fill(ds, "chk_scenic");
                scenic_chk_ada = null;

                if (ds.Tables["chk_scenic"].Rows.Count > 0)
                {
                    act_id = ds.Tables["chk_scenic"].Rows[0]["act_id"].ToString();
                    if (img_no.Trim().Length > 0)
                    {
                        csql = @"update "
                             + "  activity_img "
                             + "set "
                             + "  img_no = @act_id "
                             + "where "
                             + "  img_no = @img_no ";

                        cmd.CommandText = csql;

                        ////讓ADO.NET自行判斷型別轉換
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@act_id", act_id);
                        cmd.Parameters.AddWithValue("@img_no", img_no);

                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                err_msg = ex.Message;
            }
            finally
            {
                dw = dt.NewRow();
                dw["act_id"] = act_id;
                dw["c_msg"] = c_msg;
                dw["err_msg"] = err_msg;
                dt.Rows.Add(dw);

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return dt;
        }
        #endregion

        #region 活動花絮更新 Activity_Update
        //更新內容
        public DataTable Activity_Update(string act_id = "", string act_name = "", string act_desc = "", string show = "",string sort = "")
        {
            string err_msg = "";
            string c_msg = "";
            string c_status = "";
            DataTable dt = new DataTable();
            DataRow dw;

            dt.Columns.Add("act_id", System.Type.GetType("System.String"));
            dt.Columns.Add("c_msg", System.Type.GetType("System.String"));
            dt.Columns.Add("err_msg", System.Type.GetType("System.String"));

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                if (show == "on")
                {
                    c_status = "Y";
                }
                else
                {
                    c_status = "N";
                }

                csql = @"update "
                     + "  Activity_bt "
                     + "set "
                     + "  act_name = @act_name "
                     + ", act_desc = @act_desc "
                     + ", status = @show "
                     + ", sort = @sort "
                     + ", _UPD_ID = 'System' "
                     + ", _UPD_DT = getdate() "
                     + "where "
                     + "  act_id = @act_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@act_name", act_name);
                cmd.Parameters.AddWithValue("@act_desc", act_desc);
                cmd.Parameters.AddWithValue("@show", c_status);
                cmd.Parameters.AddWithValue("@act_id", act_id);
                cmd.Parameters.AddWithValue("@sort", sort);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                err_msg = ex.Message;
            }
            finally
            {
                dw = dt.NewRow();
                dw["act_id"] = act_id;
                dw["c_msg"] = c_msg;
                dw["err_msg"] = err_msg;
                dt.Rows.Add(dw);

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return dt;

        }
        #endregion

        #region 活動花絮刪除 Activity_Del
        public string Activity_Del(string act_id = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"delete from "
                     + "  activity_bt "
                     + "where "
                     + "  act_id = @act_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@act_id", act_id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 活動花絮狀態更新 Activity_Status_Upd
        public string Activity_Status_Upd(string act_id = "", string status = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  activity_bt "
                     + "set "
                     + "  status = @status "
                     + ", _UPD_ID = 'System' "
                     + ", _UPD_DT = getdate() "
                     + "where "
                     + "  act_id = @act_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@act_id", act_id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 變更密碼 User_Update
        public string User_Update(string account = "", string pwd = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  member "
                     + "set "
                     + "  pwd = @pwd "
                     + "where "
                     + "  account = @account ";


                cmd.CommandText = csql;

                ////定義parameter型別
                cmd.Parameters.Clear();

                cmd.Parameters.Add("@account", SqlDbType.NVarChar, 30).Value = account; //(參數,宣考型態,長度)
                cmd.Parameters.Add("@pwd", SqlDbType.NVarChar, 30).Value = pwd; //(參數,宣考型態,長度)

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 首頁廣告圖片陳列 Ad_Img_List
        public DataTable Ad_Img_List(string img_no = "")
        {
            DataSet dt = new DataSet();
            DataTable d_t;
            SqlConnection conn = new SqlConnection(conn_str);
            string[] cimg_no;
            string str_img_no = "";
            //if(img_no == "")
            //{
            //    imgno_count = -1;
            //}
            //else
            //{
            //    imgno_count = 0;
            //}

            cimg_no = img_no.Split(',');
            for (int i = 0; i < cimg_no.Length; i++)
            {
                if (i > 0)
                {
                    str_img_no = str_img_no + ",";
                }
                str_img_no = str_img_no + "'" + cimg_no[i] + "'";
            }


            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            csql = "select * from Advertisement where status = 'Y' and ad_title = 'img' ";
            //if(imgno_count == 0)
            //{
            //    csql = csql + "and ad_no in (";
            //    for (int i = 0; i < cimg_no.Length; i++)
            //    {
            //        if (i > 0)
            //        {
            //            csql = csql + ",";
            //        }
            //        csql = csql + "@str_img_no" + i.ToString() + " ";
            //    }
            //    csql = csql + ") ";
            //}

            cmd.CommandText = csql;
            //if(imgno_count == 0)
            //{
            //    cmd.Parameters.Clear();
            //    for (int i = 0; i < cimg_no.Length; i++)
            //    {
            //        cmd.Parameters.AddWithValue("@str_img_no" + i.ToString(), cimg_no[i]);
            //    }
            //}


            if (dt.Tables["img"] != null)
            {
                dt.Tables["img"].Clear();
            }

            SqlDataAdapter scenic_ada = new SqlDataAdapter();
            scenic_ada.SelectCommand = cmd;
            scenic_ada.Fill(dt, "img");
            scenic_ada = null;

            d_t = dt.Tables["img"];

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;
            dt = null;

            return d_t;
        }
        #endregion

        #region 首頁廣告圖片新增 Prod_Img_Insert
        public string Ad_Img_Insert(string img_no = "", string img_file = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"insert into Advertisement(ad_title,ad_no, ad_img) "
                     + "values('img',@img_no ,@img_file)";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_no", img_no);
                cmd.Parameters.AddWithValue("@img_file", img_file);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 首頁廣告圖片刪除 Ad_Img_Delete
        public string Ad_Img_Delete(string img_id = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"delete from Advertisement where ad_id = @img_id ";

                cmd.CommandText = csql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_id", img_id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 首頁 廣告圖片更新 Ad_Img_Update
        public string Ad_Img_Update(string img_no = "", string img_file = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  Advertisement "
                     + "set "
                     + "  ad_img = @img_file "
                     + "where "
                     + "  ad_no = @img_no ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_file", img_file);
                cmd.Parameters.AddWithValue("@img_no", img_no);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion
    }
}
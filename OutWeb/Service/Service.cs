using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Data;

namespace OutWeb.Service
{
    public class Service
    {
        public DataTable Mail(string From,string From_Name,string M_To,string subject,string body)
        {
            string err_msg = "";
            string c_msg = "";
            string status = "";
            string[] c_To;

            DataTable dt = new DataTable();
            DataRow dw;

            dt.Columns.Add("status", System.Type.GetType("System.String"));
            dt.Columns.Add("c_msg", System.Type.GetType("System.String"));
            dt.Columns.Add("err_msg", System.Type.GetType("System.String"));

            status = "N";

            try
            {
                

                System.Net.Mail.MailMessage em = new System.Net.Mail.MailMessage();
                //寄件者
                em.From = new System.Net.Mail.MailAddress(From, From_Name, System.Text.Encoding.UTF8);
                //收件者
                if (M_To.Trim().Length > 0)
                {
                    c_To = M_To.Split(',');
                    for(int i = 0; i < c_To.Length; i++)
                    {
                        //em.To.Add(new System.Net.Mail.MailAddress("收件信箱"));
                        em.To.Add(new System.Net.Mail.MailAddress(c_To[i]));
                    }
                }
                em.Bcc.Add(new System.Net.Mail.MailAddress("gary661116@gmail.com"));
                //主題
                em.Subject = subject;
                em.SubjectEncoding = System.Text.Encoding.UTF8;
                //內容
                em.Body = body; 
                em.BodyEncoding = System.Text.Encoding.UTF8;
                em.IsBodyHtml = true;	  //信件內容是否使用HTML格式
                //----------------------------------------------------------------------------//
                //Mail Server 設定
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                //登入帳號認證
                smtp.Credentials = new System.Net.NetworkCredential("changmorden@gmail.com", "661116701107");
                //使用587 Port - google要設定
                smtp.Port = 587;
                smtp.EnableSsl = true;   //啟動SSL 
                //end of google設定
                smtp.Host = "smtp.gmail.com";   //SMTP伺服器
                //----------------------------------------------------------------------------//
                smtp.Send(em);            //寄出
                status = "Y";
            }
            catch (Exception ex)
            {
                status = "N";
                err_msg = ex.Message;
            }
            finally
            {
                dw = dt.NewRow();
                dw["status"] = status;
                dw["c_msg"] = c_msg;
                dw["err_msg"] = err_msg;
                dt.Rows.Add(dw);
            }
            return dt;
        }
    }
}
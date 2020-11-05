using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ManageController : Controller
    {
        WEBATTENDANCEEntities data = new WEBATTENDANCEEntities();
        // GET: Manage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageAccount()
        {
            if (Session["Login"] == null)

                return RedirectToAction("Login", "Account");
            else
            {
                TAIKHOAN b = (TAIKHOAN)Session["Login"];

                if (b.ROLE1 == 1)
                {
                    var v = data.TAIKHOANs;
                    return View(v);
                }
                else
                {
                    return RedirectToAction("Message", new { tenaction = "quản lý tài khoản" });
                }
            }

        }

        public ActionResult Details(string id)
        {
            var result = data.TAIKHOANs.Where(x => x.USERNAME.Equals(id)).FirstOrDefault();
            return View(result);
        }

        public ActionResult Delete(string id)
        {
            var result = data.TAIKHOANs.Where(x => x.USERNAME.Equals(id)).FirstOrDefault();
            data.TAIKHOANs.Remove(result);
            data.SaveChanges();
            return RedirectToAction("ManageAccount", "Manage");
        }

        public ActionResult Edit(string id)
        {
            var result = data.TAIKHOANs.Where(x => x.USERNAME.Equals(id)).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        public ActionResult Edit(Account a)
        {
            var result = data.TAIKHOANs.Where(x => x.USERNAME.Equals(a.UserName)).FirstOrDefault();
            data.TAIKHOANs.Remove(result);
            data.SaveChanges();

            TAIKHOAN t = new TAIKHOAN();
            t.USERNAME = a.UserName;
            t.PASSWORD = a.PassWord;
            t.Name = a.Name;
            t.ROLE1 = a.Role;
            data.TAIKHOANs.Add(t);
            data.SaveChanges();
            var result1 = data.TAIKHOANs.Where(x => x.USERNAME.Equals(a.UserName)).FirstOrDefault();
            ViewBag.a = "Sửa thông tin thành công !!!";
           return View(result1);

        }

        public ActionResult AddListStudent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddListStudent(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string conString = string.Empty;

                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                conString = @"Data Source=DESKTOP-TB2RUF7;Initial Catalog=WEBATTENDANCE;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.SINHVIEN";

                        // Map the Excel columns with that of the database table, this is optional but good if you do
                        // 
                        sqlBulkCopy.ColumnMappings.Add("ID", "ID");
                        sqlBulkCopy.ColumnMappings.Add("TEN", "TEN");
                        sqlBulkCopy.ColumnMappings.Add("TENLOP", "TENLOP");
                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }
            //if the code reach here means everthing goes fine and excel data is imported into database
            ViewBag.Success = "File Imported and excel data saved into database";

            return View();
        }
        public ActionResult Message(string tenaction)
        {
            ViewBag.tenaction = tenaction;
            return View();
        }

        public ActionResult ManageStudent()
        {
            if (Session["Login"] == null)

                return RedirectToAction("Login", "Account");
            else
            {
                TAIKHOAN b = (TAIKHOAN)Session["Login"];

                if (b.ROLE1 == 1)
                {
                    var v = data.SINHVIENs;
                    return View(v);
                }
                else
                {
                    return RedirectToAction("Message", new { tenaction = "quản lý sinh viên" });
                }
            }
        }

        public ActionResult AddListTeacher()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddListTeacher(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string conString = string.Empty;

                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                conString = @"Data Source=DESKTOP-TB2RUF7;Initial Catalog=WEBATTENDANCE;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.GIANGVIEN";

                        // Map the Excel columns with that of the database table, this is optional but good if you do
                        // 
                        sqlBulkCopy.ColumnMappings.Add("ID", "ID");
                        sqlBulkCopy.ColumnMappings.Add("TEN", "TEN");
                        sqlBulkCopy.ColumnMappings.Add("CHUCVU", "CHUCVU");
                        sqlBulkCopy.ColumnMappings.Add("BANGCAP", "BANGCAP");
                        sqlBulkCopy.ColumnMappings.Add("MADONVI", "MADONVI");
                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }
            //if the code reach here means everthing goes fine and excel data is imported into database
            ViewBag.Success = "File Imported and excel data saved into database";

            return View();
        }

        public ActionResult ManageTeacher()
        {
            if (Session["Login"] == null)

                return RedirectToAction("Login", "Account");
            else
            {
                TAIKHOAN b = (TAIKHOAN)Session["Login"];

                if (b.ROLE1 == 1)
                {
                    var v = data.GIANGVIENs;
                    return View(v);
                }
                else
                {
                    return RedirectToAction("Message", new { tenaction = "quản lý giảng viên" });
                }
            }
        }

    }
}
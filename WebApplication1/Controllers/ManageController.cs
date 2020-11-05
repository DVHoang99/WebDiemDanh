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
        //=======================         Account        =======================//
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
                    return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý tài khoản" });
                }
            }

        }

        public ActionResult Details(string id)
        {
            if (id != null)
            {
                if (Session["Login"] == null)

                    return RedirectToAction("Login", "Account");
                else
                {
                    TAIKHOAN b = (TAIKHOAN)Session["Login"];

                    if (b.ROLE1 == 1)
                    {
                        var result = data.TAIKHOANs.Where(x => x.USERNAME.Equals(id)).FirstOrDefault();
                        return View(result);
                    }
                    else
                    {
                        return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý tài khoản" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Message", new { tenaction = "Bạn phải chọn tài khoản muốn sửa thông tin" });
            }

           
        }

        public ActionResult Delete(string id)
        {
            if (id != null)
            {
                if (Session["Login"] == null)

                    return RedirectToAction("Login", "Account");
                else
                {
                    TAIKHOAN b = (TAIKHOAN)Session["Login"];

                    if (b.ROLE1 == 1)
                    {
                        var result = data.TAIKHOANs.Where(x => x.USERNAME.Equals(id)).FirstOrDefault();
                        data.TAIKHOANs.Remove(result);
                        data.SaveChanges();
                        return RedirectToAction("ManageAccount", "Manage");
                    }
                    else
                    {
                        return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý tài khoản" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Message", new { tenaction = "Bạn phải chọn tài khoản muốn xóa thông tin" });
            }
            
        }

        public ActionResult Edit(string id)
        {
            if (id != null)
            {
                if (Session["Login"] == null)

                    return RedirectToAction("Login", "Account");
                else
                {
                    TAIKHOAN b = (TAIKHOAN)Session["Login"];

                    if (b.ROLE1 == 1)
                    {
                        var result = data.TAIKHOANs.Where(x => x.USERNAME.Equals(id)).FirstOrDefault();
                        return View(result);
                    }
                    else
                    {
                        return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý tài khoản" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Message", new { tenaction = "Bạn phải chọn tài khoản muốn sửa thông tin" });
            }
            
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

        //=======================         Student        =======================//
        
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
                    return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý sinh viên" });
                }
            }
        }
        public ActionResult EditStudent(string id)
        {
            if(id != null)
            {
                if (Session["Login"] == null)

                    return RedirectToAction("Login", "Account");
                else
                {
                    TAIKHOAN b = (TAIKHOAN)Session["Login"];

                    if (b.ROLE1 == 1)
                    {
                        var a = data.SINHVIENs.Where(x => x.ID.Equals(id)).FirstOrDefault();
                        return View(a);
                    }
                    else
                    {
                        return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý sinh viên" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Message", new { tenaction = "Bạn phải chọn sinh viên muốn sửa thông tin" });
            }
  
        }
        [HttpPost]
        public ActionResult EditStudent(Student std)
        {
            var result = data.SINHVIENs.Where(x => x.ID.Equals(std.Id)).FirstOrDefault();
            data.SINHVIENs.Remove(result);
            data.SaveChanges();

            SINHVIEN t = new SINHVIEN();
            t.ID = std.Id;
            t.TEN = std.TEN;
            t.TENLOP = std.TENLOP;
            data.SINHVIENs.Add(t);
            data.SaveChanges();
            var result1 = data.SINHVIENs.Where(x => x.ID.Equals(std.Id)).FirstOrDefault();
            ViewBag.a = "Sửa thông tin thành công !!!";
            return View(result1);
        }

        public ActionResult DetailsStudent(string id)
        {
            if (id != null)
            {
                if (Session["Login"] == null)

                    return RedirectToAction("Login", "Account");
                else
                {
                    TAIKHOAN b = (TAIKHOAN)Session["Login"];

                    if (b.ROLE1 == 1)
                    {
                        var a = data.SINHVIENs.Where(x => x.ID.Equals(id)).FirstOrDefault();
                        return View(a);
                    }
                    else
                    {
                        return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý sinh viên" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Message", new { tenaction = "Bạn phải chọn sinh viên muốn xem thông tin" });
            }
            
        }

        //public ActionResult DeleteStudent(Student std)
        //{
        //    var result = data.SINHVIENs.Where(x => x.USERNAME.Equals(id)).FirstOrDefault();
        //    data.TAIKHOANs.Remove(result);
        //    data.SaveChanges();
        //    return RedirectToAction("ManageAccount", "Manage");
        //}

        //=======================         Teacher        =======================//
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
                    return RedirectToAction("Message", new { tenaction = "Không thể truy cập quản lý giảng viên" });
                }
            }
        }

        public ActionResult EditTeacher(string id)
        {
            if (id != null)
            {
                if (Session["Login"] == null)

                    return RedirectToAction("Login", "Account");
                else
                {
                    TAIKHOAN b = (TAIKHOAN)Session["Login"];

                    if (b.ROLE1 == 1)
                    {
                        var a = data.GIANGVIENs.Where(x => x.ID.Equals(id)).FirstOrDefault();
                        return View(a);
                    }
                    else
                    {
                        return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý giảng viên" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Message", new { tenaction = "Bạn phải chọn giảng viên muốn sửa thông tin" });
            }

        }
        [HttpPost]
        public ActionResult EditTeacher(Teacher tch)
        {
            var result = data.GIANGVIENs.Where(x => x.ID.Equals(tch.ID)).FirstOrDefault();
            data.GIANGVIENs.Remove(result);
            data.SaveChanges();

            GIANGVIEN t = new GIANGVIEN();
            t.ID = tch.ID;
            t.TEN = tch.TEN;
            t.BANGCAP = tch.BANGCAP;
            t.CHUCVU = tch.CHUCVU;

            data.GIANGVIENs.Add(t);
            data.SaveChanges();
            var result1 = data.GIANGVIENs.Where(x => x.ID.Equals(tch.ID)).FirstOrDefault();
            ViewBag.a = "Sửa thông tin thành công !!!";
            return View(result1);
        }

        public ActionResult DeleteTeacher(string id)
        {
            if (id != null)
            {
                if (Session["Login"] == null)

                    return RedirectToAction("Login", "Account");
                else
                {
                    TAIKHOAN b = (TAIKHOAN)Session["Login"];

                    if (b.ROLE1 == 1)
                    {
                        var a = data.GIANGVIENs.Where(x => x.ID.Equals(id)).FirstOrDefault();
                        data.GIANGVIENs.Remove(a);
                        return RedirectToAction("ManageTeacher", "Manage");
                    }
                    else
                    {
                        return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý giảng viên" });
                    }
                }
            }
            else
            {
                return RedirectToAction("Message", new { tenaction = "Bạn phải chọn giảng viên muốn sửa thông tin" });
            }
        }
        //=======================         schedule        =======================//

        //public ActionResult AddSchedule()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult AddSchedule(HttpPostedFileBase postedFile)
        //{
        //    string filePath = string.Empty;
        //    if (postedFile != null)
        //    {
        //        string path = Server.MapPath("~/Uploads/");
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        filePath = path + Path.GetFileName(postedFile.FileName);
        //        string extension = Path.GetExtension(postedFile.FileName);
        //        postedFile.SaveAs(filePath);

        //        string conString = string.Empty;

        //        switch (extension)
        //        {
        //            case ".xls": //Excel 97-03.
        //                conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
        //                break;
        //            case ".xlsx": //Excel 07 and above.
        //                conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
        //                break;
        //        }

        //        DataTable dt = new DataTable();
        //        conString = string.Format(conString, filePath);

        //        using (OleDbConnection connExcel = new OleDbConnection(conString))
        //        {
        //            using (OleDbCommand cmdExcel = new OleDbCommand())
        //            {
        //                using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
        //                {
        //                    cmdExcel.Connection = connExcel;

        //                    //Get the name of First Sheet.
        //                    connExcel.Open();
        //                    DataTable dtExcelSchema;
        //                    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //                    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
        //                    connExcel.Close();

        //                    //Read Data from First Sheet.
        //                    connExcel.Open();
        //                    cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
        //                    odaExcel.SelectCommand = cmdExcel;
        //                    odaExcel.Fill(dt);
        //                    connExcel.Close();
        //                }
        //            }
        //        }

        //        conString = @"Data Source=DESKTOP-TB2RUF7;Initial Catalog=WEBATTENDANCE;Integrated Security=True";
        //        using (SqlConnection con = new SqlConnection(conString))
        //        {
        //            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
        //            {
        //                //Set the database table name.
        //                sqlBulkCopy.DestinationTableName = "dbo.GIANGVIEN";

        //                // Map the Excel columns with that of the database table, this is optional but good if you do
        //                // 
        //                sqlBulkCopy.ColumnMappings.Add("ID", "ID");
        //                sqlBulkCopy.ColumnMappings.Add("TEN", "TEN");
        //                sqlBulkCopy.ColumnMappings.Add("CHUCVU", "CHUCVU");
        //                sqlBulkCopy.ColumnMappings.Add("BANGCAP", "BANGCAP");
        //                sqlBulkCopy.ColumnMappings.Add("MADONVI", "MADONVI");
        //                con.Open();
        //                sqlBulkCopy.WriteToServer(dt);
        //                con.Close();
        //            }
        //        }
        //    }
        //    //if the code reach here means everthing goes fine and excel data is imported into database
        //    ViewBag.Success = "File Imported and excel data saved into database";

        //    return View();
        //}

        //public ActionResult ManageTeacher()
        //{
        //    if (Session["Login"] == null)

        //        return RedirectToAction("Login", "Account");
        //    else
        //    {
        //        TAIKHOAN b = (TAIKHOAN)Session["Login"];

        //        if (b.ROLE1 == 1)
        //        {
        //            var v = data.GIANGVIENs;
        //            return View(v);
        //        }
        //        else
        //        {
        //            return RedirectToAction("Message", new { tenaction = "Không thể truy cập quản lý giảng viên" });
        //        }
        //    }
        //}

        //public ActionResult EditTeacher(string id)
        //{
        //    if (id != null)
        //    {
        //        if (Session["Login"] == null)

        //            return RedirectToAction("Login", "Account");
        //        else
        //        {
        //            TAIKHOAN b = (TAIKHOAN)Session["Login"];

        //            if (b.ROLE1 == 1)
        //            {
        //                var a = data.GIANGVIENs.Where(x => x.ID.Equals(id)).FirstOrDefault();
        //                return View(a);
        //            }
        //            else
        //            {
        //                return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý giảng viên" });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Message", new { tenaction = "Bạn phải chọn giảng viên muốn sửa thông tin" });
        //    }

        //}
        //[HttpPost]
        //public ActionResult EditTeacher(Teacher tch)
        //{
        //    var result = data.GIANGVIENs.Where(x => x.ID.Equals(tch.ID)).FirstOrDefault();
        //    data.GIANGVIENs.Remove(result);
        //    data.SaveChanges();

        //    GIANGVIEN t = new GIANGVIEN();
        //    t.ID = tch.ID;
        //    t.TEN = tch.TEN;
        //    t.BANGCAP = tch.BANGCAP;
        //    t.CHUCVU = tch.CHUCVU;

        //    data.GIANGVIENs.Add(t);
        //    data.SaveChanges();
        //    var result1 = data.GIANGVIENs.Where(x => x.ID.Equals(tch.ID)).FirstOrDefault();
        //    ViewBag.a = "Sửa thông tin thành công !!!";
        //    return View(result1);
        //}

        //public ActionResult DeleteTeacher(string id)
        //{
        //    if (id != null)
        //    {
        //        if (Session["Login"] == null)

        //            return RedirectToAction("Login", "Account");
        //        else
        //        {
        //            TAIKHOAN b = (TAIKHOAN)Session["Login"];

        //            if (b.ROLE1 == 1)
        //            {
        //                var a = data.GIANGVIENs.Where(x => x.ID.Equals(id)).FirstOrDefault();
        //                data.GIANGVIENs.Remove(a);
        //                return RedirectToAction("ManageTeacher", "Manage");
        //            }
        //            else
        //            {
        //                return RedirectToAction("Message", new { tenaction = "Tài khoản của bạn không có quyền truy cập quản lý giảng viên" });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Message", new { tenaction = "Bạn phải chọn giảng viên muốn sửa thông tin" });
        //    }
        //}


        //=======================         Message        =======================//
        public ActionResult Message(string tenaction)
        {
            ViewBag.tenaction = tenaction;
            return View();
        }


    }
}
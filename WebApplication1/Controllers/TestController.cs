using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TestController : Controller
    {
        private WEBATTENDANCEEntities db = new WEBATTENDANCEEntities();
        // GET: User  
        public ActionResult Test()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Test(HttpPostedFileBase postedFile)
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
                        sqlBulkCopy.DestinationTableName = "dbo.test";

                        // Map the Excel columns with that of the database table, this is optional but good if you do
                        // 
                        sqlBulkCopy.ColumnMappings.Add("id", "id");
                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    }
                }
            }
            //if the code reach here means everthing goes fine and excel data is imported into database
            ViewBag.Success = "File Imported and excel data saved into database";

            return View();
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
            //                conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
            //                break;
            //            case ".xlsx": //Excel 07 and above.
            //                conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
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

            //        conString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
            //        using (SqlConnection con = new SqlConnection(conString))
            //        {
            //            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
            //            {
            //                //Set the database table name.
            //                sqlBulkCopy.DestinationTableName = "dbo.test";

            //                //[OPTIONAL]: Map the Excel columns with that of the database table
            //                sqlBulkCopy.ColumnMappings.Add("Id", "Id");
            //                sqlBulkCopy.ColumnMappings.Add("Name", "Name");
            //                sqlBulkCopy.ColumnMappings.Add("Adress", "Adress");
            //                sqlBulkCopy.ColumnMappings.Add("Contact", "Contact");

            //                con.Open();
            //                sqlBulkCopy.WriteToServer(dt);
            //                con.Close();
            //            }
            //        }
            //    }

            //    return View();
            }


        }
}
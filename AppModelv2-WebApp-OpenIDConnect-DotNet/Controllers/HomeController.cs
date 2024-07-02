using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using MySql.Data.MySqlClient;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
    public class HomeController : Controller
    {
        string connStr = ConfigurationManager.ConnectionStrings["mySqlConnection"].ConnectionString;
        public class patientListModel
        {
            public string patient_id { get; set; }
            public string personNbr { get; set; }
            public string patientName { get; set; }
            public string gender { get; set; }
            public string DOB { get; set; }
            public string count { get; set; }
        }
        public class dateListModel
        {
            public string date { get; set; }
            public string seriesdate { get; set; }
        }
        public class imageListModel
        {
            public string id { get; set; }
            public string device { get; set; }
            public string laterality { get; set; }
            public string type { get; set; }
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Send an OpenID Connect sign-in request.
        /// Alternatively, you can just decorate the SignIn method with the [Authorize] attribute
        /// </summary>

        [HttpPost]
        [Authorize]
        public ActionResult patientList()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            var patientItem = new List<patientListModel>();
            //string path = @"c:\users\public\documents\test.txt";
            //System.IO.File.WriteAllText(path, sort);
            string sql = "";
            MySqlCommand cmd;
            MySqlDataReader rdr;
            
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                sql = @"SELECT patient.sex, CONCAT(patient.LASTNAME, ', ', patient.FIRSTNAME) as patientName, DATE_FORMAT(patient.BIRTHDATESTRING, '%b %e, %Y') as BIRTHDATESTRING, study.PATIENT_ID, COUNT(DISTINCT study.ID) as count, patient.PATIENTID FROM archive.sopinstance sopinstance
                        LEFT JOIN archive.series as series ON series.id = sopinstance.SERIES_ID
                        LEFT JOIN archive.study as study on study.id = series.STUDY_ID
                        LEFT JOIN archive.patient as patient on patient.ID = study.PATIENT_ID
                        where acquisitiondate between '" + date + "' and '" + date + " 23:59:59' GROUP BY(study.PATIENT_ID) order by patientName asc; ";
                //string path = @"c:\users\public\documents\test.txt";
                //System.IO.File.WriteAllText(path, sql);
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    patientItem.Add(new patientListModel() { patient_id = rdr["PATIENT_ID"].ToString(), personNbr = rdr["patientid"].ToString(), patientName = rdr["patientName"].ToString(), gender = rdr["sex"].ToString(), DOB = rdr["birthdatestring"].ToString(), count = rdr["count"].ToString()});
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                string path = @"c:\users\public\documents\test.txt";
                System.IO.File.WriteAllText(path, ex.ToString());
                Console.WriteLine(ex.ToString());
            }
            
            conn.Close();
            return Json(new { patientList = patientItem });
        }

        [HttpPost]
        [Authorize]
        public ActionResult dateList(string id)
        {

            var dateItem = new List<dateListModel>();
            //string path = @"c:\users\public\documents\test.txt";
            //System.IO.File.WriteAllText(path, sort);
            string sql = "";
            MySqlCommand cmd;
            MySqlDataReader rdr;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                sql = @"SELECT DISTINCT DATE_FORMAT(series.SERIESDATE, '%b %e, %Y') as `date`, DATE_FORMAT(series.SERIESDATE, '%Y-%m-%d') as `SERIESDATE` FROM archive.study as study
                        LEFT JOIN archive.series as series ON series.STUDY_ID = study.ID
                        where study.PATIENT_ID = '" + id+ "' GROUP BY `date` order by series.SERIESDATE desc;";
                //string path = @"c:\users\public\documents\test.txt";
                //System.IO.File.WriteAllText(path, sql);
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    string date = (rdr["date"].ToString() != "") ? rdr["date"].ToString() : "N/A";
                    dateItem.Add(new dateListModel() { date = date, seriesdate = rdr["SERIESDATE"].ToString() });
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                string path = @"c:\users\public\documents\test.txt";
                System.IO.File.WriteAllText(path, ex.ToString());
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            return Json(new { dateList = dateItem });
        }

        [HttpPost]
        [Authorize]
        public ActionResult imageList(string id, string seriesdate)
        {

            var imageItem = new List<imageListModel>();
            //string path = @"c:\users\public\documents\test.txt";
            //System.IO.File.WriteAllText(path, sort);
            string sql = "";
            MySqlCommand cmd;
            MySqlDataReader rdr;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                sql = @"SELECT sopinstance.ID, IFNULL(series.MANUFACTURERMODELNAME, 'Image') as MANUFACTURERMODELNAME, sopinstance.IMAGELATERALITY, sopinstance.VISITSUBGROUPVALUE FROM archive.series as series
                        LEFT JOIN archive.study as study ON series.STUDY_ID = study.ID
                        LEFT JOIN archive.sopinstance as sopinstance ON series.ID = sopinstance.SERIES_ID
                        where study.PATIENT_ID = '" + id+ "' AND IFNULL(series.SERIESDATE, '') like '" + seriesdate + "%' AND LENGTH(THUMBNAIL) > 0;";
                string path = @"c:\users\public\documents\test.txt";
                System.IO.File.WriteAllText(path, sql);
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    imageItem.Add(new imageListModel() { id = rdr["ID"].ToString(), device = rdr["MANUFACTURERMODELNAME"].ToString(), type = rdr["VISITSUBGROUPVALUE"].ToString(), laterality = rdr["IMAGELATERALITY"].ToString() });
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                string path = @"c:\users\public\documents\test.txt";
                System.IO.File.WriteAllText(path, ex.ToString());
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            return Json(new { iamgeList = imageItem });
        }

        [HttpGet]
        [Authorize]
        public ActionResult loadImage(string id)
        {
            //string path = @"c:\users\public\documents\test.txt";
            //System.IO.File.WriteAllText(path, sort);
            string sql = "";
            string imagepath = "";
            MySqlCommand cmd;
            MySqlDataReader rdr;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                sql = "SELECT PATHINSTORAGE FROM archive.sopinstance where id = '"+id+"';";
                //string path = @"c:\users\public\documents\test.txt";
                //System.IO.File.WriteAllText(path, sql);
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                rdr.Read();
                imagepath = rdr.GetValue(0).ToString();
                rdr.Close();
            }
            catch (Exception ex)
            {
                string path = @"c:\users\public\documents\test.txt";
                System.IO.File.WriteAllText(path, ex.ToString());
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            imagepath = imagepath.Replace("/", "\\");
            byte[] fileBytes = System.IO.File.ReadAllBytes("\\\\uvp-forum-01.mneye.com\\forum\\" + imagepath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "dicom.dcm");
        }

        [HttpPost]
        [Authorize]
        public ActionResult searchList(string term, string date)
        {
            var patientItem = new List<patientListModel>();
            //string path = @"c:\users\public\documents\test.txt";
            //System.IO.File.WriteAllText(path, sort);
            string dateQuery = "";
            if(date != "")
            {
                dateQuery = "acquisitiondate between '" + date + "' and '" + date + " 23:59:59' AND ";
            }
            string sql = "";
            MySqlCommand cmd;
            MySqlDataReader rdr;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                if (term.Contains(","))
                {
                    sql = @"SELECT patient.sex, CONCAT(patient.LASTNAME, ', ', patient.FIRSTNAME) as patientName, DATE_FORMAT(patient.BIRTHDATESTRING, '%b %e, %Y') as BIRTHDATESTRING, study.PATIENT_ID, COUNT(DISTINCT study.ID) as count, patient.PATIENTID FROM archive.sopinstance sopinstance
                        LEFT JOIN archive.series as series ON series.id = sopinstance.SERIES_ID
                        LEFT JOIN archive.study as study on study.id = series.STUDY_ID
                        LEFT JOIN archive.patient as patient on patient.ID = study.PATIENT_ID
                        where " + dateQuery + "CONCAT(patient.LASTNAME, ', ', patient.FIRSTNAME) COLLATE UTF8_GENERAL_CI LIKE '" + term + "%' GROUP BY(study.PATIENT_ID) order by patientName asc; ";
                }
                else if (term == "" && date == "")
                {
                    sql = @"SELECT patient.sex, CONCAT(patient.LASTNAME, ', ', patient.FIRSTNAME) as patientName, DATE_FORMAT(patient.BIRTHDATESTRING, '%b %e, %Y') as BIRTHDATESTRING, study.PATIENT_ID, COUNT(DISTINCT study.ID) as count, patient.PATIENTID FROM archive.sopinstance sopinstance
                        LEFT JOIN archive.series as series ON series.id = sopinstance.SERIES_ID
                        LEFT JOIN archive.study as study on study.id = series.STUDY_ID
                        LEFT JOIN archive.patient as patient on patient.ID = study.PATIENT_ID
                        where acquisitiondate between '" + DateTime.Now.ToString("yyyy-MM-dd") + "' and '" + DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59' GROUP BY(study.PATIENT_ID) order by patientName asc; ";
                }
                else
                {
                    sql = @"SELECT patient.sex, CONCAT(patient.LASTNAME, ', ', patient.FIRSTNAME) as patientName, DATE_FORMAT(patient.BIRTHDATESTRING, '%b %e, %Y') as BIRTHDATESTRING, study.PATIENT_ID, COUNT(DISTINCT study.ID) as count, patient.PATIENTID FROM archive.sopinstance sopinstance
                        LEFT JOIN archive.series as series ON series.id = sopinstance.SERIES_ID
                        LEFT JOIN archive.study as study on study.id = series.STUDY_ID
                        LEFT JOIN archive.patient as patient on patient.ID = study.PATIENT_ID
                        where " + dateQuery + "(patient.LASTNAME COLLATE UTF8_GENERAL_CI LIKE '" + term + "%' OR patient.FIRSTNAME COLLATE UTF8_GENERAL_CI LIKE '" + term + "%' OR patient.PATIENTID LIKE '" + term + "%') GROUP BY(study.PATIENT_ID) order by patientName asc; ";
                }
                //string path = @"c:\users\public\documents\test.txt";
                //System.IO.File.WriteAllText(path, sql);
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    patientItem.Add(new patientListModel() { patient_id = rdr["PATIENT_ID"].ToString(), personNbr = rdr["patientid"].ToString(), patientName = rdr["patientName"].ToString(), gender = rdr["sex"].ToString(), DOB = rdr["birthdatestring"].ToString(), count = rdr["count"].ToString() });
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                string path = @"c:\users\public\documents\test.txt";
                System.IO.File.WriteAllText(path, ex.ToString());
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            return Json(new { patientList = patientItem });
        }

        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        /// <summary>
        /// Send an OpenID Connect sign-out request.
        /// </summary>
        public void SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}
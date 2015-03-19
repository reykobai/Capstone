using ProjectCollaborationSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectCollaborationSystem.Controllers
{
    public class MemberController : Controller
    {
        //
        // GET: /Member/

        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public ActionResult Index()
        {
            List<Information> infos = new List<Information>();
           
            string loginQuery = "SELECT * FROM Information ORDER BY FirstName ASC";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(loginQuery, conn))
                {
                   


                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {

                                Information info = new Information();
                                info.Username = rdr["Username"].ToString();
                                info.FirstName = rdr["FirstName"].ToString();
                                info.LastName = rdr["LastName"].ToString();
                                info.EmailAddress = rdr["EmailAddress"].ToString();
                                info.ImagePath = rdr["ImagePath"].ToString();
                                info.Specialty = rdr["Specialty"].ToString();
                                info.ContactNumber = rdr["ContactNumber"].ToString();
                                info.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                info.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);

                                infos.Add(info);

                                //return RedirectToAction("Index", "Dashboard");
                            }




                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }


            }
            return View(infos);
        }

        [HttpPost]
        public ActionResult SearchMember(string searchMember)
        {
            List<Information> infos = new List<Information>();

            string loginQuery = "SELECT * FROM Information WHERE FirstName LIKE '' + @search + '%' OR LastName LIKE '' + @search + '%' OR Specialty LIKE '' + @search + '%'";
          
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(loginQuery, conn))
                {
                    comm.Parameters.AddWithValue("search", searchMember);


                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {

                                Information info = new Information();
                                info.Username = rdr["Username"].ToString();
                                info.FirstName = rdr["FirstName"].ToString();
                                info.LastName = rdr["LastName"].ToString();
                                info.EmailAddress = rdr["EmailAddress"].ToString();
                                info.ImagePath = rdr["ImagePath"].ToString();
                                info.Specialty = rdr["Specialty"].ToString();
                                info.ContactNumber = rdr["ContactNumber"].ToString();
                                info.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                info.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);

                                infos.Add(info);

                                //return RedirectToAction("Index", "Dashboard");
                            }




                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }


            }
            return View(infos);
        }
	}
}
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
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        Information info = new Information();
        public ActionResult Index(int id)
        {
            Information info = new Information();
            string loginQuery = "SELECT * FROM Information where InformationID = @id";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(loginQuery, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                              

                               info.Username = rdr["Username"].ToString();
                                info.FirstName = rdr["FirstName"].ToString();
                                info.LastName = rdr["LastName"].ToString();
                                info.EmailAddress = rdr["EmailAddress"].ToString();
                               info.ImagePath = rdr["ImagePath"].ToString();
                               info.Specialty = rdr["Specialty"].ToString();
                               info.ContactNumber = rdr["ContactNumber"].ToString();
                               info.InformationID = Convert.ToInt32(rdr["InformationID"]);
                               info.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                             

                      
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
            return View(info);
        }

        public ActionResult editProfile(int id)
        {
            Information info = new Information();
            string loginQuery = "SELECT * FROM Information where InformationID = @id";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(loginQuery, conn))
                {
                    comm.Parameters.AddWithValue("id", id);


                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            if (rdr.Read())
                            {


                                info.Username = rdr["Username"].ToString();
                                info.FirstName = rdr["FirstName"].ToString();
                                info.LastName = rdr["LastName"].ToString();
                                info.EmailAddress = rdr["EmailAddress"].ToString();
                                info.ImagePath = rdr["ImagePath"].ToString();
                                info.Specialty = rdr["Specialty"].ToString();
                                info.ContactNumber = rdr["ContactNumber"].ToString();
                                info.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                info.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);



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
            return View(info);
        }

        [HttpPost]

        public ActionResult editProfile( string fname, string lname, string contactNumber, string specialty)
        {

            string editQuery = "Update Information SET FirstName=@FirstName, LastName=@LastName, ContactNumber=@ContactNumber, Specialty = @Specialty where InformationID = @id";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(editQuery, conn))
                {
                    comm.Parameters.AddWithValue("FirstName", fname);
                    comm.Parameters.AddWithValue("LastName", lname);
                    comm.Parameters.AddWithValue("ContactNumber", contactNumber);
                    comm.Parameters.AddWithValue("Specialty", specialty);
                    comm.Parameters.AddWithValue("id", Session["id"]);

                    try
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                        return RedirectToAction("Index", "Profile", new { id = Session["id"] });
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();

                    }
                }
            }

         

            return View();
        }
        
	}
}
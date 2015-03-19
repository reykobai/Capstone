
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ProjectCollaborationSystem.App_Start;

namespace ProjectCollaborationSystem.Controllers
{
    public class LoginController : Controller
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]

        public ActionResult SignUp(string fname, string lname, string contact, string email, string specialty, string username, string pass, string verifypass)
        {
            int usern;
            int emailad;
            string loginQuery = "SELECT * FROM Information where Username=@usernames";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(loginQuery, conn))
                {
                    comm.Parameters.AddWithValue("usernames", username);


                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                usern = 1;

                                //ModelState.AddModelError("", "Either Username or Email is already used.");
                            }

                            else
                            {
                                usern = 0;

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


            string loginQueryy = "SELECT * FROM Information where EmailAddress=@email";
            using (SqlConnection connn = new SqlConnection(connString))
            {
                using (SqlCommand commm = new SqlCommand(loginQueryy, connn))
                {
                    commm.Parameters.AddWithValue("email", email);


                    try
                    {
                        connn.Open();

                        using (SqlDataReader rdr = commm.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                emailad = 1;
                                //ModelState.AddModelError("", "Username is already used.");
                                //ModelState.AddModelError("", "Either Username or Email is already used.");
                            }

                            else
                            {
                                emailad = 0;

                            }



                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        connn.Close();
                    }
                }


            }

            if ((usern == 1) && (emailad == 0))
            {
              //  ModelState.AddModelError("", "Username is already used.");
                TempData["error"] = "Username is already used.";

            }
            else if ((usern == 0) && (emailad == 1))
            {
                //ModelState.AddModelError("", "Email is already used.");
                TempData["error"] = "Email is already used.";

            }
            else if ((usern == 1) && (emailad == 1))
            {

               // ModelState.AddModelError("", "Email and Username are already used.");
                TempData["error"] = "Email and Username are already used.";
            }
            else
            {


                if (verifypass == pass)
                {
                    var imagePath = "Images/koala.jpg";
                  
                   Insert meth = new Insert();
                    meth.signup(fname, lname, contact, email, username, pass, specialty, imagePath);
                    TempData["success"] = "You can now sign in";
                    return RedirectToAction("Index", "Login");
               


                }
                else
                {

                    //ModelState.AddModelError("", "Passwords does not match.");
                    TempData["error"] = "Passwords does not match.";
                }

            }



            return View();
        }

        [HttpPost]


        public ActionResult Index(string usernames, string pword)
        {
            

            //image




           

            //endImage

            string loginQuery = "SELECT * FROM Information where Username=@usernames AND Pword=@Pword";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(loginQuery, conn))
                {
                    comm.Parameters.AddWithValue("usernames", usernames);
                    comm.Parameters.AddWithValue("Pword", pword);
                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                Session["userid"] = rdr["Username"].ToString();
                                Session["first"] = rdr["FirstName"].ToString();
                                Session["last"] = rdr["LastName"].ToString();
                                Session["email"] = rdr["EmailAddress"].ToString();
                                Session["image"] = rdr["ImagePath"].ToString();
                                Session["id"] = rdr["InformationID"].ToString();
                                return RedirectToAction("Index", "Dashboard");
                             
                             

                            }
                            else
                            {
                                //ModelState.AddModelError("", "Invalid username or password.");
                                TempData["error"] = "Invalid username or password.";
                             
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


            return View();
        }


        public ActionResult logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
	}
}
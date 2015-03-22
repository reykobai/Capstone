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
	public class ChatController : Controller
	{
		//
		// GET: /Chat/
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
		public ActionResult Index(int id = 0)
		{
            if (id == 0)
            {
                TempData["error"] = "Chat now!";
                return View();
            }
            else
            {
                string username = Session["first"].ToString() + " " + Session["last"].ToString();
                string group = "koala";


                List<Chat_Info_Project> cips = new List<Chat_Info_Project>();
                List<Project> projects = new List<Project>();
                Project project = new Project();
                Project proj = new Project();


                string queryProject = "SELECT * FROM Project WHERE ProjectID=@id";

                using (SqlConnection connProject = new SqlConnection(connString))
                {
                    using (SqlCommand commProject = new SqlCommand(queryProject, connProject))
                    {

                        commProject.Parameters.AddWithValue("id", id);

                        try
                        {
                            connProject.Open();

                            using (SqlDataReader rdrProject = commProject.ExecuteReader())
                            {
                                while (rdrProject.Read())
                                {

                                    proj.ProjectID = Convert.ToInt32(rdrProject["ProjectID"]);
                                    proj.ProjectTitle = rdrProject["ProjectTitle"].ToString();

                                }




                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            connProject.Close();
                        }
                    }


                }

                //chat


                string query = "SELECT * FROM Chat chat JOIN Information info on chat.InformationID = info.InformationID JOIN Project project on chat.ProjectID = project.ProjectID WHERE chat.ProjectID = @id ORDER BY chat.DateAdded DESC";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand comm = new SqlCommand(query, conn))
                    {

                        comm.Parameters.AddWithValue("id", id);

                        try
                        {
                            conn.Open();

                            using (SqlDataReader rdr = comm.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    Chat_Info_Project cip = new Chat_Info_Project();

                                    Chat chatters = new Chat();
                                    Information informs = new Information();


                                    project.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                    project.ProjectTitle = rdr["ProjectTitle"].ToString();

                                    informs.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                    informs.FirstName = rdr["FirstName"].ToString();
                                    informs.LastName = rdr["LastName"].ToString();

                                    chatters.ChatID = Convert.ToInt32(rdr["ChatID"]);
                                    chatters.Message = rdr["Message"].ToString();
                                    chatters.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);

                                    cip.info = informs;

                                    cip.chat = chatters;

                                    cips.Add(cip);

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


                if (Request.QueryString["username"] != null)
                {
                    username = Request.QueryString["username"] as string;
                }

                if (Request.QueryString["group"] != null)
                {
                    group = Request.QueryString["group"] as string;
                }

                ViewBag.Username = username;
                ViewBag.Group = group;

                Information_Project ip = new Information_Project();
                ip.cip = cips;
                ip.project = proj;

                return View(ip);
            }
		}

        public ActionResult Chatmembers()
        {
          
            int id = Convert.ToInt32(Session["id"]);
        
            List<Team_Information> infos = new List<Team_Information>();
            List<Project> projects = new List<Project>();
           
            string loginQuery = "SELECT * FROM Team team JOIN Project project ON team.ProjectID = project.ProjectID WHERE team.InformationID = @id ORDER BY project.DateAdded DESC";
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
                            while (rdr.Read())
                            {

                                Project project = new Project();

                                project.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                project.ProjectTitle = rdr["ProjectTitle"].ToString();

                                projects.Add(project);
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

            Team_Information it = new Team_Information();
            it.projectList = projects;

            return View(it);
        }

        public ActionResult ChatMessage()
        {

            return View();
        }
	}
}
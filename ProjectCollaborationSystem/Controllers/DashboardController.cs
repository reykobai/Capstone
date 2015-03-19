using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCollaborationSystem.Models;

namespace ProjectCollaborationSystem.Controllers
{
     
    public class DashboardController : Controller
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //
        // GET: /Dashboard/
        
        public ActionResult Index()
      {

            List<Project> projects = new List<Project>();
            List<Task> tasks = new List<Task>();
            List<Subtask> subtasks = new List<Subtask>();
            string userid = Session["id"].ToString();

            //project
            string iQuery = "SELECT TOP 5 * FROM Project WHERE InformationID = @userid  ORDER by DateAdded DESC";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("userid", userid);
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
                                project.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                project.ProjectStatus = rdr["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                project.ProjectPriority = rdr["ProjectPriority"].ToString();
                                project.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                project.FileUploaded = rdr["FileUploaded"].ToString();


                                projects.Add(project);



                            }


                        }
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

            
            //task
            string taskQuery = "SELECT TOP 5 * FROM Task WHERE InformationID = @userid  ORDER by DateAdded DESC";

            using (SqlConnection taskconn = new SqlConnection(connString))
            {
                using (SqlCommand taskcomm = new SqlCommand(taskQuery, taskconn))
                {
                    taskcomm.Parameters.AddWithValue("userid", userid);
                    try
                    {
                        taskconn.Open();

                        using (SqlDataReader taskrdr = taskcomm.ExecuteReader())
                        {
                            while (taskrdr.Read())
                            {

                                Task task = new Task();

                                task.TaskID = Convert.ToInt32(taskrdr["TaskID"]);
                                task.TaskTitle = taskrdr["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(taskrdr["DueDate"]);
                                task.DateAdded = Convert.ToDateTime(taskrdr["DateAdded"]);
                                task.TaskStatus = taskrdr["TaskStatus"].ToString();
                                task.TaskPriority = taskrdr["TaskPriority"].ToString();
                                task.InformationID = Convert.ToInt32(taskrdr["InformationID"]);
                                task.ProjectID = Convert.ToInt32(taskrdr["ProjectID"]);
                                task.FileUploaded = taskrdr["FileUploaded"].ToString();
                                task.TaskDescription = taskrdr["TaskDescription"].ToString();




                                tasks.Add(task);



                            }


                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        taskconn.Close();
                    }
                }
            }


            //subtask
            string subtaskQuery = "SELECT TOP 5 * FROM Subtask WHERE InformationID = @userid  ORDER by DateAdded DESC";

            using (SqlConnection subtaskconn = new SqlConnection(connString))
            {
                using (SqlCommand subtaskcomm = new SqlCommand(subtaskQuery, subtaskconn))
                {
                    subtaskcomm.Parameters.AddWithValue("userid", userid);
                    try
                    {
                        subtaskconn.Open();

                        using (SqlDataReader subtaskrdr = subtaskcomm.ExecuteReader())
                        {
                            while (subtaskrdr.Read())
                            {

                                Subtask subtasking = new Subtask();


                                subtasking.SubtaskID = Convert.ToInt32(subtaskrdr["SubtaskID"]);
                                subtasking.SubtaskTitle = subtaskrdr["SubtaskTitle"].ToString();
                                subtasking.DueDate = Convert.ToDateTime(subtaskrdr["DueDate"]);
                                subtasking.DateAdded = Convert.ToDateTime(subtaskrdr["DateAdded"]);
                                subtasking.SubtaskPriority = subtaskrdr["SubtaskPriority"].ToString();
                                subtasking.SubtaskStatus = subtaskrdr["SubtaskStatus"].ToString();
                                subtasking.InformationID = Convert.ToInt32(subtaskrdr["InformationID"]);
                                subtasking.ProjectID = Convert.ToInt32(subtaskrdr["ProjectID"]);
                                subtasking.FileUploaded = subtaskrdr["FileUploaded"].ToString();
                                subtasking.SubtaskDescription = subtaskrdr["SubtaskDescription"].ToString();




                                subtasks.Add(subtasking);



                            }


                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        subtaskconn.Close();
                    }
                }
            }

            Project_Task pt = new Project_Task();
            pt.tasks = tasks;
            pt.projects = projects;
            pt.subtasks = subtasks; 

            return View(pt);
        }

       
        public ActionResult searchProj(string searchProj)
        {
            int sid = Convert.ToInt32(Session["id"]);
            List<Project> projects = new List<Project>();
            List<Task> tasks = new List<Task>();
            List<Subtask> subtasks = new List<Subtask>();

            string iQuery = "SELECT * FROM Project WHERE ProjectTitle LIKE '' + @search + '%' AND InformationID=@sid";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("search", searchProj);
                    comm.Parameters.AddWithValue("sid", sid);
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
                                project.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                project.ProjectStatus = rdr["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                project.ProjectPriority = rdr["ProjectPriority"].ToString();
                                project.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                project.FileUploaded = rdr["FileUploaded"].ToString();

                                


                                projects.Add(project);
                              



                            }


                        }
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

           
            //task
             string iQueryTask = "SELECT * FROM Task WHERE TaskTitle LIKE '' + @search + '%' AND InformationID=@id";

            using (SqlConnection connTask = new SqlConnection(connString))
            {
                using (SqlCommand commTask = new SqlCommand(iQueryTask, connTask))
                {
                    commTask.Parameters.AddWithValue("search", searchProj);
                    commTask.Parameters.AddWithValue("id", sid);
                    try
                    {
                        connTask.Open();

                        using (SqlDataReader rdrTask = commTask.ExecuteReader())
                        {
                            while (rdrTask.Read())
                            {

                               Task task = new Task();
                              

                             task.InformationID = Convert.ToInt32(rdrTask["InformationID"]);
                                task.ProjectID = Convert.ToInt32(rdrTask["ProjectID"]);
                                task.TaskID = Convert.ToInt32(rdrTask["TaskID"]);
                                task.TaskTitle = rdrTask["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(rdrTask["DueDate"]);
                                task.TaskDescription = rdrTask["TaskDescription"].ToString();
                                task.TaskStatus = rdrTask["TaskStatus"].ToString();
                                task.DateAdded = Convert.ToDateTime(rdrTask["DateAdded"]);
                                task.TaskPriority = rdrTask["TaskPriority"].ToString();
                                task.FileUploaded = rdrTask["FileUploaded"].ToString();

                                


                              tasks.Add(task);
                              



                            }


                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connTask.Close();
                    }
                }
            }

            //subtask

            string iQuerySub = "SELECT * FROM Subtask WHERE SubtaskTitle LIKE '' + @search + '%' AND InformationID=@id";

            using (SqlConnection connSub = new SqlConnection(connString))
            {
                using (SqlCommand commSub = new SqlCommand(iQuerySub, connSub))
                {
                    commSub.Parameters.AddWithValue("search", searchProj);
                    commSub.Parameters.AddWithValue("id", sid);
             

                    try
                    {
                        connSub.Open();

                        using (SqlDataReader rdrSub = commSub.ExecuteReader())
                        {
                            while (rdrSub.Read())
                            {

                                Subtask subtask = new Subtask();


                                subtask.SubtaskID = Convert.ToInt32(rdrSub["SubtaskID"]);
                                subtask.SubtaskTitle = rdrSub["SubtaskTitle"].ToString();
                                subtask.SubtaskDescription = rdrSub["SubtaskDescription"].ToString();
                                subtask.SubtaskPriority = rdrSub["SubtaskPriority"].ToString();
                                subtask.SubtaskStatus = rdrSub["SubtaskStatus"].ToString();
                                subtask.DueDate = Convert.ToDateTime(rdrSub["DueDate"]);
                                subtask.DateAdded = Convert.ToDateTime(rdrSub["DateAdded"]);
                                subtask.InformationID = Convert.ToInt32(rdrSub["InformationID"]);
                                subtask.ProjectID = Convert.ToInt32(rdrSub["ProjectID"]);
                                subtask.FileUploaded = rdrSub["FileUploaded"].ToString();
                                subtask.TaskID = Convert.ToInt32(rdrSub["TaskID"]);


                                subtasks.Add(subtask);
                              



                            }


                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connSub.Close();
                    }
                }
            }

           

            Information_Project ip = new Information_Project();
            ip.projects = projects;
            ip.task = tasks;
            ip.subtasking = subtasks;

            return View(ip);
        }


     


	}
}

         


                                

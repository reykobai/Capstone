using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectCollaborationSystem.App_Start;
using ProjectCollaborationSystem.Models;

namespace ProjectCollaborationSystem.Controllers
{
    public class SubtaskController : Controller
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //
        // GET: /Subtask/
        public string iPath { get; set; }
        public int subtaskid { get; set; }
        public ActionResult Index(string id, string ProjectID)
        {
            
            List<Information> inform = new List<Information>();
            Team teams = new Team();
            Task task = new Task();

            //task

            string taskQuery = "SELECT * FROM Task WHERE TaskID=@id";

            using (SqlConnection taskConn = new SqlConnection(connString))
            {
                using (SqlCommand taskComm = new SqlCommand(taskQuery, taskConn))
                {
                    taskComm.Parameters.AddWithValue("id", id);
                    try
                    {
                        taskConn.Open();



                        using (SqlDataReader taskrdr = taskComm.ExecuteReader())
                        {

                            while (taskrdr.Read())
                            {

                                task.TaskID = Convert.ToInt32(taskrdr["TaskID"]);

                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        taskConn.Close();
                    }
                }
            }

            //
            string iQueryy = "SELECT * FROM Team WHERE ProjectID=@id";

            using (SqlConnection connn = new SqlConnection(connString))
            {
                using (SqlCommand commm = new SqlCommand(iQueryy, connn))
                {
                    commm.Parameters.AddWithValue("id", ProjectID);
                    try
                    {
                        connn.Open();



                        using (SqlDataReader reder = commm.ExecuteReader())
                        {

                            while (reder.Read())
                            {
                              

                                teams.ProjectID = Convert.ToInt32(reder["ProjectID"]);
                                teams.InformationID = Convert.ToInt32(reder["InformationID"]);

                             
                                //information---------------------------

                                string InfoQuery = "SELECT * FROM Information WHERE InformationID=@id";

                                using (SqlConnection InfoConn = new SqlConnection(connString))
                                {
                                    using (SqlCommand InfoComm = new SqlCommand(InfoQuery, InfoConn))
                                    {
                                        InfoComm.Parameters.AddWithValue("id", teams.InformationID);
                                        try
                                        {
                                            InfoConn.Open();



                                            using (SqlDataReader Infordr = InfoComm.ExecuteReader())
                                            {

                                                while (Infordr.Read())
                                                {
                                                    Information information = new Information();


                                                    information.FirstName = Infordr["FirstName"].ToString();
                                                    information.LastName = Infordr["LastName"].ToString();
                                                    information.EmailAddress = Infordr["EmailAddress"].ToString();
                                                    information.InformationID = Convert.ToInt32(Infordr["InformationID"]);
                                                    

                                                    inform.Add(information);


                                                }


                                            }


                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }
                                        finally
                                        {
                                            InfoConn.Close();
                                        }
                                    }
                                }


                                //end info

                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connn.Close();
                    }
                }
            }


            Team_Information ti = new Team_Information();
            ti.infoList = inform;
            ti.teams = teams;
            ti.tasks = task;
          


            return View(ti);
        }

        [HttpPost]
        public ActionResult Index(int id, string ProjectID, string subtasktitle, string subtaskdesc, string subtaskprio, DateTime subtaskdue, string subtaskassign, HttpPostedFileBase subtaskUploadFile)
        {
            Task task = new Task();
            //project
            string iQueryProject = "SELECT * FROM Task WHERE ProjectID = @ProjectID";

            using (SqlConnection connProj = new SqlConnection(connString))
            {
                using (SqlCommand commProj = new SqlCommand(iQueryProject, connProj))
                {
                    commProj.Parameters.AddWithValue("ProjectID", ProjectID);
                    try
                    {
                        connProj.Open();

                        using (SqlDataReader rdrProj = commProj.ExecuteReader())
                        {
                            while (rdrProj.Read())
                            {


                                task.InformationID = Convert.ToInt32(rdrProj["InformationID"]);
                                task.ProjectID = Convert.ToInt32(rdrProj["ProjectID"]);
                                task.TaskID = Convert.ToInt32(rdrProj["TaskID"]);
                                task.TaskTitle = rdrProj["TaskTitle"].ToString();
                                task.DueDate = Convert.ToDateTime(rdrProj["DueDate"]);
                                task.TaskDescription = rdrProj["TaskDescription"].ToString();
                                task.TaskStatus = rdrProj["TaskStatus"].ToString();
                                task.DateAdded = Convert.ToDateTime(rdrProj["DateAdded"]);
                                task.TaskPriority = rdrProj["TaskPriority"].ToString();
                                task.FileUploaded = rdrProj["FileUploaded"].ToString();


                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connProj.Close();
                    }
                }
            }


            int informID = Convert.ToInt32(Session["id"]);
            int pID = Convert.ToInt32(ProjectID);


            Insert insertTask = new Insert();

            if (subtaskUploadFile != null && subtaskUploadFile.ContentLength > 0)
                try
                {
                    string filename = Path.GetFileName(subtaskUploadFile.FileName);
                    iPath = "Files/" + subtasktitle + "/" + filename;
                    string path = Path.Combine(Server.MapPath("~/Files"), Path.GetFileName(subtaskUploadFile.FileName));
                    subtaskUploadFile.SaveAs(iPath);



                }
                catch (Exception)
                {
                    TempData["error"] = "Wrong Format.";

                }
            else
            {
                TempData["error"] = "No file uploaded.";
            }

            if (task.DueDate < subtaskdue)
            {
                //ModelState.AddModelError("", "");
                TempData["error"] = "The Date you entered is beyond the Task's Due Date.";
            }
            else
            {
             

                insertTask.InsertSubTask(subtasktitle, subtaskdesc, subtaskprio, subtaskdue, subtaskassign, informID, pID, id, iPath);

                string iQueryyTask = "SELECT TOP 1 * FROM Subtask ORDER BY SubtaskID DESC ";

                using (SqlConnection connnTask = new SqlConnection(connString))
                {
                    using (SqlCommand commmTask = new SqlCommand(iQueryyTask, connnTask))
                    {
                        try
                        {
                            connnTask.Open();



                            using (SqlDataReader rederTask = commmTask.ExecuteReader())
                            {

                                while (rederTask.Read())
                                {
                                    subtaskid = Convert.ToInt32(rederTask["SubtaskID"]);

                                }


                            }


                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            connnTask.Close();
                        }
                    }
                }

                string action = "Subtask is created";
                int informedID = Convert.ToInt32(Session["id"]);
                insertTask.SubtaskHistory(subtaskid, informedID, action);


                return RedirectToAction("DisplayTask", "Task", new { id = id });
            }
            //insertProject.InsertProject(projtitle, projdesc, projprio, projdue, projstatus, informID, iPath);

            List<Information> inform = new List<Information>();
            Team teams = new Team();
            Task tasks = new Task();

            //task

            string taskQuery = "SELECT * FROM Task WHERE TaskID=@id";

            using (SqlConnection taskConn = new SqlConnection(connString))
            {
                using (SqlCommand taskComm = new SqlCommand(taskQuery, taskConn))
                {
                    taskComm.Parameters.AddWithValue("id", id);
                    try
                    {
                        taskConn.Open();



                        using (SqlDataReader taskrdr = taskComm.ExecuteReader())
                        {

                            while (taskrdr.Read())
                            {

                                tasks.TaskID = Convert.ToInt32(taskrdr["TaskID"]);

                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        taskConn.Close();
                    }
                }
            }

            //
            string iQueryy = "SELECT * FROM Team WHERE ProjectID=@id";

            using (SqlConnection connn = new SqlConnection(connString))
            {
                using (SqlCommand commm = new SqlCommand(iQueryy, connn))
                {
                    commm.Parameters.AddWithValue("id", ProjectID);
                    try
                    {
                        connn.Open();



                        using (SqlDataReader reder = commm.ExecuteReader())
                        {

                            while (reder.Read())
                            {


                                teams.ProjectID = Convert.ToInt32(reder["ProjectID"]);
                                teams.InformationID = Convert.ToInt32(reder["InformationID"]);


                                //information---------------------------

                                string InfoQuery = "SELECT * FROM Information WHERE InformationID=@id";

                                using (SqlConnection InfoConn = new SqlConnection(connString))
                                {
                                    using (SqlCommand InfoComm = new SqlCommand(InfoQuery, InfoConn))
                                    {
                                        InfoComm.Parameters.AddWithValue("id", teams.InformationID);
                                        try
                                        {
                                            InfoConn.Open();



                                            using (SqlDataReader Infordr = InfoComm.ExecuteReader())
                                            {

                                                while (Infordr.Read())
                                                {
                                                    Information information = new Information();


                                                    information.FirstName = Infordr["FirstName"].ToString();
                                                    information.LastName = Infordr["LastName"].ToString();
                                                    information.EmailAddress = Infordr["EmailAddress"].ToString();
                                                    information.InformationID = Convert.ToInt32(Infordr["InformationID"]);


                                                    inform.Add(information);


                                                }


                                            }


                                        }
                                        catch (Exception)
                                        {
                                            throw;
                                        }
                                        finally
                                        {
                                            InfoConn.Close();
                                        }
                                    }
                                }


                                //end info

                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connn.Close();
                    }
                }
            }


            Team_Information ti = new Team_Information();
            ti.infoList = inform;
            ti.teams = teams;
            ti.tasks = tasks;



            return View(ti);
  
        }
        public ActionResult DisplaySubtask(string id, string ProjectID )
        {
            Subtask subtask = new Subtask();
            Project project = new Project();
            Task task = new Task();

            Information info = new Information();
            List<Information> infoing = new List<Information>();
            List<Subtask> subtasking = new List<Subtask>();
            List<Task> tasks = new List<Task>();

            string iQuery = "SELECT * FROM Subtask WHERE SubtaskID = @SubtaskID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("SubtaskID", id);
                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                subtask.SubtaskID = Convert.ToInt32(rdr["SubtaskID"]);
                                subtask.SubtaskTitle = rdr["SubtaskTitle"].ToString();
                                subtask.SubtaskDescription = rdr["SubtaskDescription"].ToString();
                                subtask.SubtaskPriority = rdr["SubtaskPriority"].ToString();
                                subtask.SubtaskStatus = rdr["SubtaskStatus"].ToString();
                                subtask.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                subtask.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                subtask.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                subtask.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                subtask.FileUploaded = rdr["FileUploaded"].ToString();
                                subtask.TaskID = Convert.ToInt32(rdr["TaskID"]);

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

            string iQueryTask = "SELECT * FROM Task WHERE ProjectID=@ProjectID ";

            using (SqlConnection connTask = new SqlConnection(connString))
            {
                using (SqlCommand commTask = new SqlCommand(iQueryTask, connTask))
                {
                    commTask.Parameters.AddWithValue("ProjectID", subtask.ProjectID);
                    try
                    {
                        connTask.Open();

                        using (SqlDataReader rdrTask = commTask.ExecuteReader())
                        {
                            while (rdrTask.Read())
                            {

                            


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

            //project
            string iQueryProject = "SELECT * FROM Project WHERE ProjectID = @ProjectID";

            using (SqlConnection connProj = new SqlConnection(connString))
            {
                using (SqlCommand commProj = new SqlCommand(iQueryProject, connProj))
                {
                    commProj.Parameters.AddWithValue("ProjectID", subtask.ProjectID);
                    try
                    {
                        connProj.Open();

                        using (SqlDataReader rdrProj = commProj.ExecuteReader())
                        {
                            while (rdrProj.Read())
                            {

                                project.ProjectID = Convert.ToInt32(rdrProj["ProjectID"]);
                                project.InformationID = Convert.ToInt32(rdrProj["InformationID"]);
                                project.ProjectTitle = rdrProj["ProjectTitle"].ToString();


                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connProj.Close();
                    }
                }
            }

            //teaminfo


            List<Team_Information> teaminfos = new List<Team_Information>();
            string iQueryTeam = "SELECT * FROM Team team INNER JOIN Information info ON team.InformationID = info.InformationID WHERE team.ProjectID = @ProjectID";

            using (SqlConnection connTeam= new SqlConnection(connString))
            {
                using (SqlCommand commTeam = new SqlCommand(iQueryTeam, connTeam))
                {
                    commTeam.Parameters.AddWithValue("ProjectID", subtask.ProjectID);
                    try
                    {
                        connTeam.Open();

                        using (SqlDataReader rdrTeam = commTeam.ExecuteReader())
                        {
                            while (rdrTeam.Read())
                            {
                                Team_Information infoteam = new Team_Information();

                                Team teams = new Team();
                                teams.ProjectID = Convert.ToInt32(rdrTeam["ProjectID"]);
                                teams.InformationID = Convert.ToInt32(rdrTeam["InformationID"]);

                               

                                Information infos = new Information();
                                infos.InformationID = Convert.ToInt32(rdrTeam["InformationID"]);

                                infoteam.inform = infos;
                                infoteam.teams = teams;

                                teaminfos.Add(infoteam);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connTeam.Close();
                    }
                }
            }

            //info

            string iQueryInfo = "SELECT * FROM Information WHERE InformationID = @InformationID";

            using (SqlConnection conns = new SqlConnection(connString))
            {
                using (SqlCommand comms = new SqlCommand(iQueryInfo, conns))
                {
                    comms.Parameters.AddWithValue("InformationID", subtask.InformationID);
                    try
                    {
                        conns.Open();

                        using (SqlDataReader rdrs = comms.ExecuteReader())
                        {
                            while (rdrs.Read())
                            {

                                info.FirstName = rdrs["FirstName"].ToString();
                                info.LastName = rdrs["LastName"].ToString();
                                info.InformationID = Convert.ToInt32(rdrs["InformationID"]);

                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        conns.Close();
                    }
                }
            }

            //comment

            List<Comment_Information> cis = new List<Comment_Information>();
            string commentQuery = "SELECT * FROM Comment c JOIN Information i ON c.InformationID = i.InformationID WHERE c.SubtaskID = @id AND c.CommentStatus = 'Active' ORDER BY c.DateAdded DESC";

            using (SqlConnection commentConn = new SqlConnection(connString))
            {
                using (SqlCommand commentComm = new SqlCommand(commentQuery, commentConn))
                {
                    commentComm.Parameters.AddWithValue("id", id);
                    try
                    {
                        commentConn.Open();



                        using (SqlDataReader commentrdr = commentComm.ExecuteReader())
                        {

                            while (commentrdr.Read())
                            {
                                Comment_Information ci = new Comment_Information();

                                Comment comments = new Comment();

                                comments.CommentID = Convert.ToInt32(commentrdr["CommentID"]);
                                comments.CommentMessage = commentrdr["CommentMessage"].ToString();
                                comments.CommentStatus = commentrdr["CommentStatus"].ToString();
                                comments.DateAdded = Convert.ToDateTime(commentrdr["DateAdded"]);


                                Information commentInfo = new Information();
                                commentInfo.InformationID = Convert.ToInt32(commentrdr["InformationID"]);
                                commentInfo.FirstName = commentrdr["FirstName"].ToString();
                                commentInfo.LastName = commentrdr["LastName"].ToString();
                                commentInfo.Username = commentrdr["Username"].ToString();
                                commentInfo.DateAdded = Convert.ToDateTime(commentrdr["DateAdded"]);
                                commentInfo.ImagePath = commentrdr["ImagePath"].ToString();
             

                                ci.info = commentInfo;
                                ci.comments = comments;

                                cis.Add(ci);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        commentConn.Close();
                    }
                }
            }



            //History


            List<History_Information> his = new List<History_Information>();
            string historyQuery = "SELECT * FROM History c JOIN Information i ON c.InformationID = i.InformationID WHERE c.SubtaskID = @id ORDER BY c.DateAdded DESC";

            using (SqlConnection historyConn = new SqlConnection(connString))
            {
                using (SqlCommand historyComm = new SqlCommand(historyQuery, historyConn))
                {
                    historyComm.Parameters.AddWithValue("id", id);
                    try
                    {
                        historyConn.Open();



                        using (SqlDataReader historyrdr = historyComm.ExecuteReader())
                        {

                            while (historyrdr.Read())
                            {
                                History_Information hi = new History_Information();

                                History histowry = new History();

                               
                                histowry.SubtaskID = Convert.ToInt32(historyrdr["SubtaskID"]);
                                histowry.InformationID = Convert.ToInt32(historyrdr["InformationID"]);
                                histowry.Action = historyrdr["Action"].ToString();
                                histowry.DateAdded = Convert.ToDateTime(historyrdr["DateAdded"]);

                                Information commentInfo = new Information();
                                commentInfo.InformationID = Convert.ToInt32(historyrdr["InformationID"]);
                                commentInfo.FirstName = historyrdr["FirstName"].ToString();
                                commentInfo.LastName = historyrdr["LastName"].ToString();
                                commentInfo.Username = historyrdr["Username"].ToString();
                                commentInfo.DateAdded = Convert.ToDateTime(historyrdr["DateAdded"]);
                                commentInfo.ImagePath = historyrdr["ImagePath"].ToString();


                                hi.history = histowry;
                                hi.information = commentInfo;

                                his.Add(hi);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        historyConn.Close();
                    }
                }
            }






           

            Information_Project ip = new Information_Project();
            ip.info = info;
            ip.subtask = subtask;
            ip.cinfo = cis;
            ip.project = project;
            ip.tasking = task;
            ip.teaminfo = teaminfos;
            ip.Hist_inf = his;
            return View(ip);
        }

        [HttpPost]
        public ActionResult DisplaySubtask(int id, string CommentMessage)
        {
            int commentor = Convert.ToInt32(Session["id"]);
            Insert Addcomment = new Insert();
            Addcomment.AddCommentSubtask(CommentMessage, id, commentor);

            return RedirectToAction("DisplaySubtask", "Subtask", new { id = id });

        }

        public ActionResult Subtasks()
        {

            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Subtask> subtask = new List<Subtask>();
            string subtasksQuery = "SELECT TOP 5 * FROM Subtask WHERE InformationID = @id AND SubtaskStatus = 'Open'  AND DueDate >= @time ORDER BY DateAdded DESC";

            using (SqlConnection subtasksConn = new SqlConnection(connString))
            {
                using (SqlCommand subtasksComm = new SqlCommand(subtasksQuery, subtasksConn))
                {
                    subtasksComm.Parameters.AddWithValue("id", infoID);
                    subtasksComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        subtasksConn.Open();



                        using (SqlDataReader subtasksrdr = subtasksComm.ExecuteReader())
                        {

                            while (subtasksrdr.Read())
                            {
                                Subtask subtasks = new Subtask();

                                subtasks.InformationID = Convert.ToInt32(subtasksrdr["InformationID"]);
                                subtasks.ProjectID = Convert.ToInt32(subtasksrdr["ProjectID"]);
                                subtasks.SubtaskID = Convert.ToInt32(subtasksrdr["SubtaskID"]);
                                subtasks.SubtaskTitle = subtasksrdr["SubtaskTitle"].ToString();
                                subtasks.DueDate = Convert.ToDateTime(subtasksrdr["DueDate"]);
                                subtasks.SubtaskDescription = subtasksrdr["SubtaskDescription"].ToString();
                                subtasks.SubtaskStatus = subtasksrdr["SubtaskStatus"].ToString();
                                subtasks.DateAdded = Convert.ToDateTime(subtasksrdr["DateAdded"]);
                                subtasks.SubtaskPriority = subtasksrdr["SubtaskPriority"].ToString();
                                subtasks.FileUploaded = subtasksrdr["FileUploaded"].ToString();

                                subtask.Add(subtasks);


                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        subtasksConn.Close();
                    }
                }
            }


            //current




            List<Subtask> subtaskCurrent = new List<Subtask>();
            string subtaskCurrentsQuery = "SELECT TOP 5 * FROM Subtask WHERE InformationID = @id AND SubtaskStatus = 'In Progress' ORDER BY DateAdded DESC";

            using (SqlConnection subtaskCurrentsConn = new SqlConnection(connString))
            {
                using (SqlCommand subtaskCurrentsComm = new SqlCommand(subtaskCurrentsQuery, subtaskCurrentsConn))
                {
                    subtaskCurrentsComm.Parameters.AddWithValue("id", infoID);
                    try
                    {
                        subtaskCurrentsConn.Open();



                        using (SqlDataReader subtaskrdr = subtaskCurrentsComm.ExecuteReader())
                        {

                            while (subtaskrdr.Read())
                            {
                                Subtask subtaskCurrents = new Subtask();



                                subtaskCurrents.InformationID = Convert.ToInt32(subtaskrdr["InformationID"]);
                                subtaskCurrents.ProjectID = Convert.ToInt32(subtaskrdr["ProjectID"]);
                                subtaskCurrents.SubtaskID = Convert.ToInt32(subtaskrdr["SubtaskID"]);
                                subtaskCurrents.SubtaskTitle = subtaskrdr["SubtaskTitle"].ToString();
                                subtaskCurrents.DueDate = Convert.ToDateTime(subtaskrdr["DueDate"]);
                                subtaskCurrents.SubtaskDescription = subtaskrdr["SubtaskDescription"].ToString();
                                subtaskCurrents.SubtaskStatus = subtaskrdr["SubtaskStatus"].ToString();
                                subtaskCurrents.DateAdded = Convert.ToDateTime(subtaskrdr["DateAdded"]);
                                subtaskCurrents.SubtaskPriority = subtaskrdr["SubtaskPriority"].ToString();
                                subtaskCurrents.FileUploaded = subtaskrdr["FileUploaded"].ToString();

                                subtaskCurrent.Add(subtaskCurrents);




                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        subtaskCurrentsConn.Close();
                    }
                }
            }

            //delay

            DateTime delayedTime = System.DateTime.Now;
            List<Subtask> delayedsubtask = new List<Subtask>();
            string delayedsubtaskQuery = "SELECT TOP 5 * FROM Subtask WHERE InformationID = @id AND DueDate < @delayedTime ORDER BY DateAdded DESC";

            using (SqlConnection delayedsubtaskConn = new SqlConnection(connString))
            {
                using (SqlCommand delayedsubtaskComm = new SqlCommand(delayedsubtaskQuery, delayedsubtaskConn))
                {
                    delayedsubtaskComm.Parameters.AddWithValue("id", infoID);
                    delayedsubtaskComm.Parameters.AddWithValue("delayedTime", delayedTime);

                    try
                    {
                        delayedsubtaskConn.Open();



                        using (SqlDataReader delayedsubtaskrdr = delayedsubtaskComm.ExecuteReader())
                        {

                            while (delayedsubtaskrdr.Read())
                            {
                                Subtask delayedsubtasks = new Subtask();



                                delayedsubtasks.InformationID = Convert.ToInt32(delayedsubtaskrdr["InformationID"]);
                                delayedsubtasks.ProjectID = Convert.ToInt32(delayedsubtaskrdr["ProjectID"]);
                                delayedsubtasks.SubtaskID = Convert.ToInt32(delayedsubtaskrdr["SubtaskID"]);
                                delayedsubtasks.SubtaskTitle = delayedsubtaskrdr["SubtaskTitle"].ToString();
                                delayedsubtasks.DueDate = Convert.ToDateTime(delayedsubtaskrdr["DueDate"]);
                                delayedsubtasks.SubtaskDescription = delayedsubtaskrdr["SubtaskDescription"].ToString();
                                delayedsubtasks.SubtaskStatus = delayedsubtaskrdr["SubtaskStatus"].ToString();
                                delayedsubtasks.DateAdded = Convert.ToDateTime(delayedsubtaskrdr["DateAdded"]);
                                delayedsubtasks.SubtaskPriority = delayedsubtaskrdr["SubtaskPriority"].ToString();
                                delayedsubtasks.FileUploaded = delayedsubtaskrdr["FileUploaded"].ToString();

                                delayedsubtask.Add(delayedsubtasks);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        delayedsubtaskConn.Close();
                    }
                }
            }

            Task_information subtaskStat = new Task_information();
            subtaskStat.subtas = subtask;
            subtaskStat.subta = subtaskCurrent;
            subtaskStat.subt = delayedsubtask;

            return View(subtaskStat);

          
        }

        public ActionResult DeleteComment(int id, int SubtaskID)
        {
            Insert delComment = new Insert();
            delComment.DeleteComment(id);

            return RedirectToAction("DisplaySubtask", "Subtask", new { id = SubtaskID });
        }

        public ActionResult Resolve(int id)
        {
            Insert resolve = new Insert();
            int infoID = Convert.ToInt32(Session["id"]);
            string action = "Status changed to Resolved.";

            resolve.SubtaskHistory(id,infoID,action);
            resolve.ResolveSubtask(id);
            return RedirectToAction("DisplaySubtask", "Subtask", new { id = id });
        }

        public ActionResult Reopen(int id)
        {
            Insert resolve = new Insert();
            int infoID = Convert.ToInt32(Session["id"]);
            string action = "Status changed to Re-Open.";

            resolve.SubtaskHistory(id, infoID, action);
            resolve.ReOpenSubtask(id);
            return RedirectToAction("DisplaySubtask", "Subtask", new { id = id });
        }

        public ActionResult InProgress(int id)
        {
            Insert resolve = new Insert();
            int infoID = Convert.ToInt32(Session["id"]);
            string action = "Status changed to In-Progress.";

            resolve.SubtaskHistory(id, infoID, action);
            resolve.ProgressSubtask(id);
            return RedirectToAction("DisplaySubtask", "Subtask", new { id = id });
        }

        public ActionResult editSubtask(int id, int TaskID, int ProjectID)
        {
            Project project = new Project();
            Subtask subtask = new Subtask();

            //subtask
            string iQuery = "SELECT * FROM Subtask WHERE SubtaskID = @SubtaskID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("SubtaskID", id);
                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                subtask.SubtaskID = Convert.ToInt32(rdr["SubtaskID"]);
                                subtask.SubtaskTitle = rdr["SubtaskTitle"].ToString();
                                subtask.SubtaskDescription = rdr["SubtaskDescription"].ToString();
                                subtask.SubtaskPriority = rdr["SubtaskPriority"].ToString();
                                subtask.SubtaskStatus = rdr["SubtaskStatus"].ToString();
                                subtask.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                subtask.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                subtask.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                subtask.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                subtask.FileUploaded = rdr["FileUploaded"].ToString();
                                subtask.TaskID = Convert.ToInt32(rdr["TaskID"]);

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
            //project
            string projQuery = "SELECT * FROM Project WHERE ProjectID = @ProjectID";

            using (SqlConnection connproj = new SqlConnection(connString))
            {
                using (SqlCommand commproj = new SqlCommand(projQuery, connproj))
                {
                    commproj.Parameters.AddWithValue("ProjectID", subtask.ProjectID);
                    try
                    {
                        connproj.Open();

                        using (SqlDataReader rdrproj = commproj.ExecuteReader())
                        {
                            while (rdrproj.Read())
                            {
                                project.ProjectID = Convert.ToInt32(rdrproj["ProjectID"]);
                                project.ProjectTitle = rdrproj["ProjectTitle"].ToString();
                                project.DueDate = Convert.ToDateTime(rdrproj["DueDate"]);
                                project.ProjectStatus = rdrproj["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdrproj["DateAdded"]);
                                project.InformationID = Convert.ToInt32(rdrproj["InformationID"]);
                                project.FileUploaded = rdrproj["FileUploaded"].ToString();
                                project.ProjectPriority = rdrproj["ProjectPriority"].ToString();
                                project.ProjectDescription = rdrproj["ProjectDescription"].ToString();

                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connproj.Close();
                    }
                }
            }

            //team
            List<Team_Information> teaminfoList = new List<Team_Information>();
          
            string iQueryInfo = "SELECT * FROM Team T INNER JOIN Information I on T.InformationID = I.InformationID WHERE T.ProjectID = @ProjectID";

            using (SqlConnection conns = new SqlConnection(connString))
            {
                using (SqlCommand comms = new SqlCommand(iQueryInfo, conns))
                {
                    comms.Parameters.AddWithValue("ProjectID", ProjectID);
                    try
                    {
                        conns.Open();

                        using (SqlDataReader rdrs = comms.ExecuteReader())
                        {
                            while (rdrs.Read())
                            {


                                Team_Information bulok = new Team_Information();
                                Information info = new Information();


                                info.InformationID = Convert.ToInt32(rdrs["InformationID"]); ;
                                info.FirstName = rdrs["FirstName"].ToString();
                                info.LastName = rdrs["LastName"].ToString();
                                info.EmailAddress = rdrs["EmailAddress"].ToString();

                                bulok.inform = info;



                                teaminfoList.Add(bulok);




                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        conns.Close();
                    }
                }
            }

            

            Information_Project ip = new Information_Project();
            ip.project = project;
            ip.teaminfo = teaminfoList;
            ip.subtask = subtask;



            return View(ip);
        }

        [HttpPost]

        public ActionResult editSubtask(string subtasktitle, string subtaskdesc, string subtaskprio, DateTime subtaskdue, int id, int subtaskassign)
        {
            //if error

            Project project = new Project();
            Subtask subtask = new Subtask();

            //subtask
            string iQuery = "SELECT * FROM Subtask WHERE SubtaskID = @SubtaskID";

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(iQuery, conn))
                {
                    comm.Parameters.AddWithValue("SubtaskID", id);
                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                subtask.SubtaskID = Convert.ToInt32(rdr["SubtaskID"]);
                                subtask.SubtaskTitle = rdr["SubtaskTitle"].ToString();
                                subtask.SubtaskDescription = rdr["SubtaskDescription"].ToString();
                                subtask.SubtaskPriority = rdr["SubtaskPriority"].ToString();
                                subtask.SubtaskStatus = rdr["SubtaskStatus"].ToString();
                                subtask.DueDate = Convert.ToDateTime(rdr["DueDate"]);
                                subtask.DateAdded = Convert.ToDateTime(rdr["DateAdded"]);
                                subtask.InformationID = Convert.ToInt32(rdr["InformationID"]);
                                subtask.ProjectID = Convert.ToInt32(rdr["ProjectID"]);
                                subtask.FileUploaded = rdr["FileUploaded"].ToString();
                                subtask.TaskID = Convert.ToInt32(rdr["TaskID"]);

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
            //project
            string projQuery = "SELECT * FROM Project WHERE ProjectID = @ProjectID";

            using (SqlConnection connproj = new SqlConnection(connString))
            {
                using (SqlCommand commproj = new SqlCommand(projQuery, connproj))
                {
                    commproj.Parameters.AddWithValue("ProjectID", subtask.ProjectID);
                    try
                    {
                        connproj.Open();

                        using (SqlDataReader rdrproj = commproj.ExecuteReader())
                        {
                            while (rdrproj.Read())
                            {
                                project.ProjectID = Convert.ToInt32(rdrproj["ProjectID"]);
                                project.ProjectTitle = rdrproj["ProjectTitle"].ToString();
                                project.DueDate = Convert.ToDateTime(rdrproj["DueDate"]);
                                project.ProjectStatus = rdrproj["ProjectStatus"].ToString();
                                project.DateAdded = Convert.ToDateTime(rdrproj["DateAdded"]);
                                project.InformationID = Convert.ToInt32(rdrproj["InformationID"]);
                                project.FileUploaded = rdrproj["FileUploaded"].ToString();
                                project.ProjectPriority = rdrproj["ProjectPriority"].ToString();
                                project.ProjectDescription = rdrproj["ProjectDescription"].ToString();

                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connproj.Close();
                    }
                }
            }

            //team
            List<Team_Information> teaminfoList = new List<Team_Information>();

            string iQueryInfo = "SELECT * FROM Team T INNER JOIN Information I on T.InformationID = I.InformationID WHERE T.ProjectID = @ProjectID";

            using (SqlConnection conns = new SqlConnection(connString))
            {
                using (SqlCommand comms = new SqlCommand(iQueryInfo, conns))
                {
                    comms.Parameters.AddWithValue("ProjectID", project.ProjectID);
                    try
                    {
                        conns.Open();

                        using (SqlDataReader rdrs = comms.ExecuteReader())
                        {
                            while (rdrs.Read())
                            {


                                Team_Information bulok = new Team_Information();
                                Information info = new Information();


                                info.InformationID = Convert.ToInt32(rdrs["InformationID"]); ;
                                info.FirstName = rdrs["FirstName"].ToString();
                                info.LastName = rdrs["LastName"].ToString();
                                info.EmailAddress = rdrs["EmailAddress"].ToString();

                                bulok.inform = info;



                                teaminfoList.Add(bulok);




                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        conns.Close();
                    }
                }
            }



            Information_Project ip = new Information_Project();
            ip.project = project;
            ip.teaminfo = teaminfoList;
            ip.subtask = subtask;

            if (subtask.DueDate < subtaskdue)
            {
                TempData["error"] = "The Date you entered is beyond the Task's Due Date.";
                return View(ip);
            }
            else
            {
                //end if error
                DateTime due = Convert.ToDateTime(subtaskdue);
                Insert insert = new Insert();
                int informedID = Convert.ToInt32(Session["id"]);
                //check assignee
                Information info = new Information();
                Information infox = new Information();
                string loginQuery = "SELECT * FROM Information info JOIN Subtask sub ON info.InformationID = sub.InformationID WHERE SubtaskID = @id";
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


                                    info.InformationID = Convert.ToInt32(rdr["InformationID"]);

                                    if (info.InformationID == subtaskassign)
                                    {
                                        string action = "The subtask was updated.";
                                        insert.SubtaskHistory(id, informedID, action);

                                    }
                                    else
                                    {
                                        string loginQueryx = "SELECT * FROM Information WHERE InformationID = @infoid";
                                        using (SqlConnection connx = new SqlConnection(connString))
                                        {
                                            using (SqlCommand commx = new SqlCommand(loginQueryx, connx))
                                            {

                                                commx.Parameters.AddWithValue("infoid", subtaskassign);

                                                try
                                                {
                                                    connx.Open();

                                                    using (SqlDataReader rdrx = commx.ExecuteReader())
                                                    {
                                                        while (rdrx.Read())
                                                        {


                                                            infox.Username = rdrx["Username"].ToString();
                                                            infox.FirstName = rdrx["FirstName"].ToString();
                                                            infox.LastName = rdrx["LastName"].ToString();
                                                            infox.EmailAddress = rdrx["EmailAddress"].ToString();
                                                            infox.ImagePath = rdrx["ImagePath"].ToString();
                                                            infox.Specialty = rdrx["Specialty"].ToString();
                                                            infox.ContactNumber = rdrx["ContactNumber"].ToString();
                                                            infox.InformationID = Convert.ToInt32(rdrx["InformationID"]);
                                                            infox.DateAdded = Convert.ToDateTime(rdrx["DateAdded"]);



                                                        }




                                                    }
                                                }
                                                catch
                                                {
                                                    throw;
                                                }
                                                finally
                                                {
                                                    connx.Close();
                                                }
                                            }


                                        }

                                        string action = "The subtask was updated. Re-assign the subtask to " + infox.FirstName + " " + infox.LastName;
                                        insert.SubtaskHistory(id, informedID, action);
                                    }
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
                //


                insert.UpdateSubtask(subtasktitle, subtaskdesc, subtaskprio, due, id, subtaskassign);


                return RedirectToAction("DisplaySubtask", "Subtask", new { id = id });
            }
        }
        [HttpPost]
        public ActionResult editSubtaskAssignee(int id, int subtaskassign)
        {

            Information info = new Information();
            string loginQuery = "SELECT * FROM Information WHERE InformationID=@taskassign";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand comm = new SqlCommand(loginQuery, conn))
                {
                    comm.Parameters.AddWithValue("taskassign", subtaskassign);


                    try
                    {
                        conn.Open();

                        using (SqlDataReader rdr = comm.ExecuteReader())
                        {
                            while (rdr.Read())
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
            //updateAssignee

            Insert insert = new Insert();
            int informedID = Convert.ToInt32(Session["id"]);
            string action = "Re-assign subtask to" + " " + @info.FirstName + " " + @info.LastName;
            insert.SubtaskHistory(id, informedID, action);
           
            insert.editSubtaskAssignee(id, subtaskassign);
            return RedirectToAction("DisplaySubtask", "Subtask", new { id = id });
        }

        public ActionResult CurrentSubtask()
        {

            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Subtask> subtask = new List<Subtask>();
            string subtasksQuery = "SELECT TOP 5 * FROM Subtask WHERE InformationID = @id AND SubtaskStatus = 'In Progress'  AND DueDate >= @time ORDER BY DateAdded DESC";

            using (SqlConnection subtasksConn = new SqlConnection(connString))
            {
                using (SqlCommand subtasksComm = new SqlCommand(subtasksQuery, subtasksConn))
                {
                    subtasksComm.Parameters.AddWithValue("id", infoID);
                    subtasksComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        subtasksConn.Open();



                        using (SqlDataReader subtasksrdr = subtasksComm.ExecuteReader())
                        {

                            while (subtasksrdr.Read())
                            {
                                Subtask subtasks = new Subtask();

                                subtasks.InformationID = Convert.ToInt32(subtasksrdr["InformationID"]);
                                subtasks.ProjectID = Convert.ToInt32(subtasksrdr["ProjectID"]);
                                subtasks.SubtaskID = Convert.ToInt32(subtasksrdr["SubtaskID"]);
                                subtasks.SubtaskTitle = subtasksrdr["SubtaskTitle"].ToString();
                                subtasks.DueDate = Convert.ToDateTime(subtasksrdr["DueDate"]);
                                subtasks.SubtaskDescription = subtasksrdr["SubtaskDescription"].ToString();
                                subtasks.SubtaskStatus = subtasksrdr["SubtaskStatus"].ToString();
                                subtasks.DateAdded = Convert.ToDateTime(subtasksrdr["DateAdded"]);
                                subtasks.SubtaskPriority = subtasksrdr["SubtaskPriority"].ToString();
                                subtasks.FileUploaded = subtasksrdr["FileUploaded"].ToString();

                                subtask.Add(subtasks);


                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        subtasksConn.Close();
                    }
                }
            }

            return View(subtask);

        }

        public ActionResult PendingSubtask()
        {

            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Subtask> subtask = new List<Subtask>();
            string subtasksQuery = "SELECT TOP 5 * FROM Subtask WHERE InformationID = @id AND SubtaskStatus = 'Open'  AND DueDate >= @time ORDER BY DateAdded DESC";

            using (SqlConnection subtasksConn = new SqlConnection(connString))
            {
                using (SqlCommand subtasksComm = new SqlCommand(subtasksQuery, subtasksConn))
                {
                    subtasksComm.Parameters.AddWithValue("id", infoID);
                    subtasksComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        subtasksConn.Open();



                        using (SqlDataReader subtasksrdr = subtasksComm.ExecuteReader())
                        {

                            while (subtasksrdr.Read())
                            {
                                Subtask subtasks = new Subtask();

                                subtasks.InformationID = Convert.ToInt32(subtasksrdr["InformationID"]);
                                subtasks.ProjectID = Convert.ToInt32(subtasksrdr["ProjectID"]);
                                subtasks.SubtaskID = Convert.ToInt32(subtasksrdr["SubtaskID"]);
                                subtasks.SubtaskTitle = subtasksrdr["SubtaskTitle"].ToString();
                                subtasks.DueDate = Convert.ToDateTime(subtasksrdr["DueDate"]);
                                subtasks.SubtaskDescription = subtasksrdr["SubtaskDescription"].ToString();
                                subtasks.SubtaskStatus = subtasksrdr["SubtaskStatus"].ToString();
                                subtasks.DateAdded = Convert.ToDateTime(subtasksrdr["DateAdded"]);
                                subtasks.SubtaskPriority = subtasksrdr["SubtaskPriority"].ToString();
                                subtasks.FileUploaded = subtasksrdr["FileUploaded"].ToString();

                                subtask.Add(subtasks);


                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        subtasksConn.Close();
                    }
                }
            }

            return View(subtask);

        }

        public ActionResult DelayedSubtask()
        {

            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Subtask> subtask = new List<Subtask>();
            string subtasksQuery = "SELECT TOP 5 * FROM Subtask WHERE InformationID = @id AND SubtaskStatus != 'Resolved'  AND DueDate >= @time ORDER BY DateAdded DESC";

            using (SqlConnection subtasksConn = new SqlConnection(connString))
            {
                using (SqlCommand subtasksComm = new SqlCommand(subtasksQuery, subtasksConn))
                {
                    subtasksComm.Parameters.AddWithValue("id", infoID);
                    subtasksComm.Parameters.AddWithValue("time", time);
                    try
                    {
                        subtasksConn.Open();



                        using (SqlDataReader subtasksrdr = subtasksComm.ExecuteReader())
                        {

                            while (subtasksrdr.Read())
                            {
                                Subtask subtasks = new Subtask();

                                subtasks.InformationID = Convert.ToInt32(subtasksrdr["InformationID"]);
                                subtasks.ProjectID = Convert.ToInt32(subtasksrdr["ProjectID"]);
                                subtasks.SubtaskID = Convert.ToInt32(subtasksrdr["SubtaskID"]);
                                subtasks.SubtaskTitle = subtasksrdr["SubtaskTitle"].ToString();
                                subtasks.DueDate = Convert.ToDateTime(subtasksrdr["DueDate"]);
                                subtasks.SubtaskDescription = subtasksrdr["SubtaskDescription"].ToString();
                                subtasks.SubtaskStatus = subtasksrdr["SubtaskStatus"].ToString();
                                subtasks.DateAdded = Convert.ToDateTime(subtasksrdr["DateAdded"]);
                                subtasks.SubtaskPriority = subtasksrdr["SubtaskPriority"].ToString();
                                subtasks.FileUploaded = subtasksrdr["FileUploaded"].ToString();

                                subtask.Add(subtasks);


                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        subtasksConn.Close();
                    }
                }
            }

            return View(subtask);
        
        }

        public ActionResult ViewAllSubtasks()
        {
            int infoID = Convert.ToInt32(Session["id"]);
            DateTime time = System.DateTime.Now;
            List<Subtask> proj = new List<Subtask>();
            string projCurrentQuery = "SELECT  * FROM Subtask WHERE InformationID = @id ORDER BY DueDate DESC";

            using (SqlConnection projConn = new SqlConnection(connString))
            {
                using (SqlCommand projComm = new SqlCommand(projCurrentQuery, projConn))
                {
                    projComm.Parameters.AddWithValue("id", infoID);

                    try
                    {
                        projConn.Open();



                        using (SqlDataReader projrdr = projComm.ExecuteReader())
                        {

                            while (projrdr.Read())
                            {
                                Subtask projs = new Subtask();

                                projs.SubtaskID = Convert.ToInt32(projrdr["SubtaskID"]);
                                projs.SubtaskTitle = projrdr["SubtaskTitle"].ToString();
                                projs.SubtaskStatus = projrdr["SubtaskStatus"].ToString();
                                projs.SubtaskPriority = projrdr["SubtaskPriority"].ToString();
                                projs.SubtaskDescription = projrdr["SubtaskDescription"].ToString();
                                projs.TaskID = Convert.ToInt32(projrdr["TaskID"]);                        
                                projs.DueDate = Convert.ToDateTime(projrdr["DueDate"]);
                                projs.ProjectID = Convert.ToInt32(projrdr["ProjectID"]);
                                projs.DateAdded = Convert.ToDateTime(projrdr["DateAdded"]);
                                projs.InformationID = Convert.ToInt32(projrdr["InformationID"]);
                                projs.FileUploaded = projrdr["FileUploaded"].ToString();


                                proj.Add(projs);



                            }


                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        projConn.Close();
                    }
                }
            }







            return View(proj);

        }

	}
}